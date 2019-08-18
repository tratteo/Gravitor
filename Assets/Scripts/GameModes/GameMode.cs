using System;
using UnityEngine;


public abstract class GameMode : MonoBehaviour
{
    protected const string GAMEOBSTACLES_CAT_TAG = "Obstacles";
    protected const string PICKUPS_CAT_TAG = "Extras";

    #region Variables
    //Editor variables
    [Header("Obstacle spawn parameters")]
    public Vector2 obstacleRandXSpawn;
    public Vector2 obstacleRandYSpawn;
    public Vector2 obstacleRandZSpawn;
    [Header("Pickups spawn parameters")]
    [SerializeField] protected Vector2 pickUpSpawnRateRange;
    public Vector2 pickupsRandXSpawn;
    public Vector2 pickupsRandYSpawn;
    public Vector2 pickupsRandZSpawn;

    //Bools
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isGameOver = false;
    private bool highScoreReached = false;

    //References
    protected PoolManager poolManager;
    [HideInInspector] private GameObject player;
    [HideInInspector] public PlayerManager playerManager = null;

    //Local Variables
    [HideInInspector] public int sessionGravityPoints = 0;
    private int currentGravityPoints;
    [HideInInspector] public float sessionScore;
    [HideInInspector] public bool attemptUsed = false;

    private int currentHighScore;

    protected Spawner obstacleSpawner, pickupsSpawner;
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
        player = FindObjectOfType<PlayerManager>().gameObject;
        playerManager = player.GetComponent<PlayerManager>();
    }

    void OnDisable()
    {
        playerManager.UnsubscribeToPlayerDeathEvent(EndSession);
        GoogleAdsManager.GetInstance().UnsubscribeToRewardClaimed(EarnReward);
    }

    void Start()
    {
        poolManager = PoolManager.GetInstance();

        SaveObject objectData;
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.HIGHSCORE_PATH);
        currentHighScore = objectData != null ? currentHighScore = objectData.GetData<int>() : 0;
        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.GRAVITYPOINTS_PATH);
        currentGravityPoints = objectData != null ? currentGravityPoints = objectData.GetData<int>() : 0;

        //Obstacle Spawner
        obstacleSpawner = new Spawner(this, obstacleRandXSpawn, obstacleRandYSpawn, obstacleRandZSpawn);
        obstacleSpawner.CreateSpawnTimer(GameplayMath.GetInstance().GetSpawnRateFromTime, true);
        obstacleSpawner.SubscribeToSpawnEvent(InstantiateObstacle);
        //Pick-Up Spawner
        pickupsSpawner = new Spawner(this, pickupsRandXSpawn, pickupsRandYSpawn, pickupsRandZSpawn);
        pickupsSpawner.CreateSpawnTimer(pickUpSpawnRateRange, true);
        pickupsSpawner.SubscribeToSpawnEvent(InstantiatePickUp);

        //Events subscriptions
        playerManager.SubscribeToPlayerDeathEvent(EndSession);
        GoogleAdsManager.GetInstance().SubscribeToRewardClaimed(EarnReward);

        playerManager.movementManager.SpeedEffect(1.25f, 8f);
        CameraManager.GetInstance().SmoothInAndOutFOV(null, 175f, 10f, 0.15f, 1.15f);
    }

    private void FixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1f && !isGameOver)
        {
            sessionScore += Time.fixedDeltaTime * 20f * playerManager.movementManager.currentSlingMultiplier * playerManager.timeDistortion;
        }
        if (!highScoreReached && sessionScore > currentHighScore)
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
            pickupsSpawner.PauseSpawnTimer(false);
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
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        obstacleSpawner.PauseSpawnTimer(true);
        pickupsSpawner.PauseSpawnTimer(true);

        sessionGravityPoints += (int)(0.2f * (0.8f * sessionScore * (playerManager.properTime / 150f)));

        SaveManager.GetInstance().SavePersistentData<int>(sessionGravityPoints + currentGravityPoints, SaveManager.GRAVITYPOINTS_PATH);
        if (currentHighScore != default)
        {
            currentHighScore = sessionScore > currentHighScore ? (int)sessionScore : currentHighScore;
        }
        else
        {
            currentHighScore = (int)sessionScore;
        }

        SaveManager.GetInstance().SavePersistentData<int>(currentHighScore, SaveManager.HIGHSCORE_PATH);
        isGameOver = true;
        HUDManager.GetInstance().DisplayGameOver();
        HUDManager.GetInstance().ShowDangerZoneUI(false);
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
            poolManager.DeactivateObject(collider.gameObject);
        }
    }

}
