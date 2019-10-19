using UnityEngine;


public abstract class GameMode : MonoBehaviour
{
    public enum GradeObtained { UNRANKED, BRONZE, SILVER, GOLD }

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
    [HideInInspector] public int sessionGravitons = 0;
    [HideInInspector] public bool attemptUsed = false;
    private float alpha, beta, gamma, delta;

    [HideInInspector] public Level currentLevel = null;
    private int currentHighscore;

    protected Spawner obstacleSpawner, extraSpawner;

    protected CurrencyData currencyData;

    #endregion

    protected virtual void InstantiateObstacle() { }

    protected virtual void InstantiatePickUp() { }


    private void Awake()
    {
        currentLevel = LevelLoader.GetInstance().GetCurrentLevel();
    }

    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        InitializeLevelParameters();

        SaveObject objectData;

        objectData = SaveManager.GetInstance().LoadPersistentData(SaveManager.CURRENCY_PATH);
        currencyData = objectData.GetData<CurrencyData>();
        currentGravityPoints = objectData != null ? currencyData.gravityPoints : 0;

        LevelsData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
        currentHighscore = data.GetLevelHighScore(currentLevel.id);

        HUDManager.GetInstance().DisplayLevelObjectivePanel();

        playerManager.movementManager.SpeedEffect(1.25f, playerManager.movementManager.maxSpeed / 1.4f, false, false);
        CameraManager.GetInstance().SmoothInAndOutFOV(null, 175f, 0.25f, 0.75f);
    }

    private void InitializeLevelParameters()
    {
        poolManager = currentLevel.poolManager;

        poolManager.Initialize();

        extrasSpawnRateRange = currentLevel.extrasSpawnRateRange;

        if (currentLevel.overrideSpawnArea)
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
            sessionScore += 1.25f * Time.fixedDeltaTime * (600f * playerManager.movementManager.currentSlingMultiplier + 100f * (playerManager.scoreMultiplier - 1f));
        }
        if (currentLevel.category == Level.LevelCategory.ENDLESS && !highScoreReached && sessionScore > currentHighscore)
        {
            highScoreReached = true;
            HUDManager.GetInstance().DisplayHighscorePanel();
        }
    }

    public void BonusScore(float bonusScore)
    {
        sessionScore += bonusScore;
    }

    public void Attempt()
    {
        if (!attemptUsed)
        {
            isGameOver = false;
            attemptUsed = true;
            obstacleSpawner.PauseSpawnTimer(false);
            extraSpawner.PauseSpawnTimer(false);

            playerManager.Attempt();
            HUDManager.GetInstance().Attempt();

            Executer.GetInstance().AddJob(() => { ClearMap(); });
        }
    }

    public void EndSession()
    {
        GradeObtained obt = GradeObtained.UNRANKED;
        isGameOver = true;
        highScoreReached = false;

        obstacleSpawner.PauseSpawnTimer(true);
        extraSpawner.PauseSpawnTimer(true);

        sessionGravityPoints = GameplayMath.GetInstance().GetGravityPointsFromSession(sessionScore, playerManager.properTime, currentLevel);

        if (currentLevel.category == Level.LevelCategory.ENDLESS)
        {
            LevelsData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
            data.UpdateLevelScore(LevelsData.ENDLESS_ID, (int)sessionScore);
            SaveManager.GetInstance().SavePersistentData<LevelsData>(data, SaveManager.LEVELSDATA_PATH);

            obt = CheckForGradeBonusGP();

            if (attemptUsed)
            {
                sessionGravitons = 0;
            }
            else
            {
                sessionGravitons = GameplayMath.GetInstance().GetGravitonsFromGame(playerManager.properTime, sessionScore);
                currencyData.gravitons += sessionGravitons;
            }
        }
        currencyData.gravityPoints = sessionGravityPoints + currentGravityPoints;
        SaveManager.GetInstance().SavePersistentData<CurrencyData>(currencyData, SaveManager.CURRENCY_PATH);

        CheckForPlayerLevelUp(obt);

        if (currentLevel.category == Level.LevelCategory.ENDLESS)
        {
            HUDManager.GetInstance().DisplayGameOverPanel(true, true);
        }
        else
        {
            HUDManager.GetInstance().DisplayGameOverPanel(false, false);
        }
        HUDManager.GetInstance().EnableHighGravityFieldPanel(false);
    }

    public void LevelCompleted()
    {
        Time.timeScale = 1f;
        isGameOver = true;
        attemptUsed = true;

        obstacleSpawner.PauseSpawnTimer(true);
        extraSpawner.PauseSpawnTimer(true);

        LevelsData levelsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
        levelsData.UnlockLevel(currentLevel.id + 1);
        levelsData.UpdateLevelScore(currentLevel.id, (int)sessionScore);
        SaveManager.GetInstance().SavePersistentData<LevelsData>(levelsData, SaveManager.LEVELSDATA_PATH);

        sessionGravityPoints = GameplayMath.GetInstance().GetGravityPointsFromSession(sessionScore, playerManager.properTime, currentLevel);

        GradeObtained obt = CheckForGradeBonusGP();
        currencyData.gravityPoints = sessionGravityPoints + currentGravityPoints;
        SaveManager.GetInstance().SavePersistentData<CurrencyData>(currencyData, SaveManager.CURRENCY_PATH);

        CheckForPlayerLevelUp(obt);

        HUDManager.GetInstance().EnableHighGravityFieldPanel(false);
        HUDManager.GetInstance().DisplayLevelCompletedPanel();
    }

    public void NotifySpawnException(Vector3 centre, float width, float height, float depth, float duration)
    {
        obstacleSpawner.CreateSpawnException(centre, width, height, depth, duration, true);
    }

    public void ClearMap()
    {
        LayerMask mask = LayerMask.GetMask("Obstacles", "Extras");
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(0, 0, 1500), new Vector3(300, 300, 1500), Quaternion.identity, mask);
        foreach (Collider collider in hitColliders)
        {
            collider.gameObject.SetActive(false);
        }
    }

    public float GetSpawnRateFromTime(int seconds)
    {
        return (-alpha / ((seconds * beta) + gamma)) + delta;
    }

    private void CheckForPlayerLevelUp(GradeObtained obt)
    {
        int newLevel = playerManager.CalculateLevel(sessionGravityPoints, obt);
        if (newLevel != 0)
        {
            HUDManager.GetInstance().DisplayPlayerLevelUp(newLevel);
        }
    }

    private GradeObtained CheckForGradeBonusGP()
    {
        GradeObtained obtained = GradeObtained.UNRANKED;
        int levelGP = 0;
        if (sessionScore >= currentLevel.goldScore)
        {
            levelGP = currentLevel.goldGP;
            obtained = GradeObtained.GOLD;
        }
        else if (sessionScore >= currentLevel.silverScore)
        {
            levelGP = currentLevel.silverGP;
            obtained = GradeObtained.SILVER;
        }
        else if (sessionScore >= currentLevel.bronzeScore)
        {
            levelGP = currentLevel.bronzeGP;
            obtained = GradeObtained.BRONZE;
        }
        if (levelGP != 0)
        {
            sessionGravityPoints += levelGP;
        }
        return obtained;
    }
}
