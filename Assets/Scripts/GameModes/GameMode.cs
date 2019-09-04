using System;
using UnityEngine;


public abstract class GameMode : MonoBehaviour
{
    protected const string GAMEOBSTACLES_CAT_TAG = "Obstacles";
    protected const string PICKUPS_CAT_TAG = "Extras";

    #region Variables
    //Editor variables
    [Header("Spawn area parameters")]
    public Vector2 randXSpawn;
    public Vector2 randYSpawn;
    public Vector2 randZSpawn;

    protected Vector2 extrasSpawnRateRange;

    //Bools
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isGameOver = false;
    private bool highScoreReached = false;

    //References
    protected PoolManager poolManager;
    [HideInInspector] public PlayerManager playerManager = null;

    //Local Variables
    [HideInInspector] public int sessionGravityPoints = 0;
    private int currentGravityPoints;
    [HideInInspector] public float sessionScore;
    [HideInInspector] public bool attemptUsed = false;
    private float alpha, beta, gamma, delta;

    private Level currentLevel = null;
    private int currentHighscore;

    protected Spawner obstacleSpawner, extraSpawner;
    //Events
    private event Action OnHighScoreReached;
    public void SubrscribeToHighScoreReachedEvent(Action funcToSub) { OnHighScoreReached += funcToSub; }
    public void UnsubrscribeToHighScoreReachedEvent(Action funcToUnsub) { OnHighScoreReached -= funcToUnsub; }

    private event Action OnAttempt;
    public void SubscribeToOnAttemptEvent(Action funcToSub) { OnAttempt += funcToSub; }
    public void UnsubscribeToOnAttemptEvent(Action funcToUnsub) { OnAttempt -= funcToUnsub; }

    #endregion


    protected virtual void InstantiateObstacle() { }

    protected virtual void InstantiatePickUp() { }


    private void Awake()
    {
        currentLevel = LevelLoader.GetInstance().GetCurrentLevel();
    }

    void OnDisable()
    {
        playerManager.UnsubscribeToPlayerDeathEvent(EndSession);
        playerManager.UnsubscribeToOnLevelCompletedEvent(LevelCompleted);
        GoogleAdsManager.GetInstance().UnsubscribeToRewardClaimed(EarnReward);
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        InitializeLevelParameters();

        SaveObject objectData;     
        
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        currentGravityPoints = objectData != null ? currentGravityPoints = objectData.GetData<int>() : 0;

        LevelsData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
        currentHighscore = data.GetLevelHighScore(currentLevel.id);

        //Events subscriptions
        playerManager.SubscribeToPlayerDeathEvent(EndSession);
        playerManager.SubscribeToOnLevelCompletedEvent(LevelCompleted);
        GoogleAdsManager.GetInstance().SubscribeToRewardClaimed(EarnReward);

        HUDManager.GetInstance().SetHUDBasedOnLevel(currentLevel);
        playerManager.movementManager.SpeedEffect(2f, playerManager.movementManager.maxSpeed / 2, false, false);
        CameraManager.GetInstance().SmoothInAndOutFOV(null, 175f, 0.25f, 1.5f);
    }

    private void InitializeLevelParameters()
    {
        poolManager = currentLevel.poolManager;
        poolManager.Initialize();

        extrasSpawnRateRange = currentLevel.extrasSpawnRateRange;

        if(currentLevel.overrideSpawnArea)
        {
            randXSpawn = currentLevel.randXSpawn;
            randYSpawn = currentLevel.randYSpawn;
            randZSpawn = currentLevel.randZSpawn;
        }

        //Obstacle Spawner
        obstacleSpawner = new Spawner(this, randXSpawn, randYSpawn, randZSpawn);
        if (currentLevel.isSpawnRateConst)
        {
            obstacleSpawner.CreateSpawnTimer(currentLevel.constSpawnRate, true);
        }
        else
        {
            alpha = currentLevel.a;
            beta = currentLevel.b;
            gamma = currentLevel.g;
            delta = currentLevel.d;

            obstacleSpawner.CreateSpawnTimer(GetSpawnRateFromTime, true);
        }
        obstacleSpawner.SubscribeToSpawnEvent(InstantiateObstacle);


        //Extra Spawner
        extraSpawner = new Spawner(this, randXSpawn, randYSpawn, randZSpawn);
        extraSpawner.CreateSpawnTimer(extrasSpawnRateRange, true);
        extraSpawner.SubscribeToSpawnEvent(InstantiatePickUp);

        playerManager.level = currentLevel;
    }

    private void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1f && !isGameOver)
        {
            sessionScore += Time.fixedDeltaTime * (260f * playerManager.movementManager.currentSlingMultiplier + 750f * (playerManager.scoreMultiplier - 0.975f));
        }
        if (currentLevel.category == Level.LevelCategory.ENDLESS && !highScoreReached && sessionScore > currentHighscore)
        {
            highScoreReached = true;
            //FIRE EVENT
            OnHighScoreReached();
        }
    }

    public void BonusScore(float bonusScore)
    {
        sessionScore += bonusScore;
    }

    private void EarnReward(string reward)
    {
        if (reward == "Attempt" || reward == "coins")
        {
            Attempt();
        }
    }

    private void Attempt()
    {
        if (!attemptUsed)
        {
            isGameOver = false;
            attemptUsed = true;
            obstacleSpawner.PauseSpawnTimer(false);
            extraSpawner.PauseSpawnTimer(false);
            OnAttempt();
            Executer.GetInstance().AddJob(() =>
            {
               ClearMap();
            });

        }
    }

    public void EndSession()
    {
        Time.timeScale = 1f;

        obstacleSpawner.PauseSpawnTimer(true);
        extraSpawner.PauseSpawnTimer(true);

        sessionGravityPoints = (int)(0.185f * (0.6f * sessionScore * (playerManager.properTime / 225f)));

        SaveManager.GetInstance().SavePersistentData<int>(sessionGravityPoints + currentGravityPoints, SaveManager.GRAVITYPOINTS_PATH);
        if (currentLevel.category == Level.LevelCategory.ENDLESS)
        {
            LevelsData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
            data.UpdateLevelScore(LevelsData.ENDLESS_ID, (int)sessionScore);
            SaveManager.GetInstance().SavePersistentData<LevelsData>(data, SaveManager.LEVELSDATA_PATH);
        }
        isGameOver = true;
        HUDManager.GetInstance().DisplayGameOver();
        HUDManager.GetInstance().ShowHighGravityPanel(false);
    }

    private void LevelCompleted()
    {
        sessionGravityPoints = (int)(0.185f * (0.6f * sessionScore * (playerManager.properTime / 225f)));
        SaveManager.GetInstance().SavePersistentData<int>(sessionGravityPoints + currentGravityPoints, SaveManager.GRAVITYPOINTS_PATH);
        LevelsData levelsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
        levelsData.UnlockLevel(currentLevel.id + 1);
        levelsData.UpdateLevelScore(currentLevel.id, (int)sessionScore);
        SaveManager.GetInstance().SavePersistentData<LevelsData>(levelsData, SaveManager.LEVELSDATA_PATH);

        Time.timeScale = 1f;
        isGameOver = true;
        sessionGravityPoints = (int)(0.185f * (0.6f * sessionScore * (playerManager.properTime / 300f)));
        obstacleSpawner.PauseSpawnTimer(true);
        extraSpawner.PauseSpawnTimer(true);
        HUDManager.GetInstance().ShowHighGravityPanel(false);
        HUDManager.GetInstance().DisplayLevelCompleted();
    }

    public void NotifySpawnException(Vector3 centre, float width, float height, float depth, float duration)
    {
        obstacleSpawner.CreateSpawnException(centre, width, height, depth, duration, true);
    }

    public void ClearMap()
    {
        LayerMask mask = LayerMask.GetMask("Obstacles", "Extras");
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(0, 0, 1500), new Vector3(300, 300, 1500), Quaternion.identity, mask);
        foreach(Collider collider in hitColliders)
        {
            collider.gameObject.SetActive(false);
        }
    }

    public float GetSpawnRateFromTime(int seconds)
    {
        return (-alpha / ((seconds * beta) + gamma)) + delta;
        //return (-865f / ((seconds * 0.0625f) + 45)) + 20.5f;
    }
}
