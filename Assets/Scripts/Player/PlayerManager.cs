using EZCameraShake;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementManager))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(ExtraManager))]
public class PlayerManager : MonoBehaviour
{
    #region Variables
    public enum PlayerState { ASTEROID, COMET };

    //Editor variables
    [Header("References")]
    public Camera mainCamera;
    [SerializeField] private Canvas directionArrow = null;
    [Header("Effects")]
    public GameObject asteroidDE;
    public GameObject cometDE;
    private GameObject deathEffect;
    [SerializeField] private GameObject damageNebula;
    private ParticleSystem latDamageNebulaEffect;
    private ParticleSystem verticalDamageNebulaEffect;
    [SerializeField] private GameObject rightNebulaSpawner;
    [SerializeField] private GameObject leftNebulaSpawner;
    [SerializeField] private GameObject topNebulaSpawner;
    [SerializeField] private GameObject bottomNebulaSpawner;
    [Header("Camera shaker")]
    [SerializeField] private float shakeMagnitude = 5f;
    [SerializeField] private float shakeRoughness = 8f;
    [SerializeField] private float shakeFadeInTime = 0.25f;
    [SerializeField] private float shakeFadeOutTime = 0.8f;
    [Header("Collision parameters")]
    [SerializeField] private float collisionTimerMultiplier = 2f;
    [SerializeField] private float collisionSlowMotionDuration = 2f;
    [Header("Aspect")]
    [SerializeField] private Material asteroidMaterial = null;
    [SerializeField] private Material cometMaterial = null;

    //Player references
    [HideInInspector] public MovementManager movementManager;
    [HideInInspector] public SkillManager skillManager;
    [HideInInspector] public ExtraManager extraManager;

    //References
    [HideInInspector] public GameMode gameMode;

    //Local variables
    [HideInInspector] public float resilience;
    private float initialResilience;
    private PlayerData playerData;
    [HideInInspector] public List<GameObstacle> gravityFieldsGens;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public int dangerZoneCount = 0;
    [HideInInspector] public HUDManager hudManagerInstance;
    [HideInInspector] public bool isDead = false;
    private int sessionObstaclesHit = 0;

    [HideInInspector] public float scoreMultiplier = 1f;
    [HideInInspector] public float timeDistortion = 1f;
    [HideInInspector] public bool isGravityTdActive = true;

    [HideInInspector] public float properTime = 0f;
    [HideInInspector] public float relativeExTime = 0f;
    [HideInInspector] public float velocityRelativeTime = 0f;

    [HideInInspector] public Level level;


    public struct SessionStats
    {
        public float maxSpeedReached;
        public float maxSpeed;
        public float timePlayed;
        public float distortedTime;
        public float score;
        public int obstaclesHit;
    }

    #endregion

    private void Awake()
    {
        gameMode = FindObjectOfType<GameMode>();
        movementManager = GetComponent<MovementManager>();
        skillManager = GetComponent<SkillManager>();
        extraManager = GetComponent<ExtraManager>();

        playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        if (playerData != null)
        {
            playerState = playerData.playerState;
            initialResilience = playerData.resilience;
        }
        dangerZoneCount = 0;
    }

    private void Start()
    {
        gravityFieldsGens = new List<GameObstacle>();

        resilience = initialResilience;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        switch (playerState)
        {
            case PlayerState.ASTEROID:
                renderer.material = asteroidMaterial;
                deathEffect = asteroidDE;
                break;

            case PlayerState.COMET:
                renderer.material = cometMaterial;
                deathEffect = cometDE;
                break;
        }

        hudManagerInstance = HUDManager.GetInstance();

        hudManagerInstance.UpdatePlayerHealthUI(resilience, initialResilience);

        StartCoroutine(UpdateTimeDistortion());
    }

    void Update()
    {
        if (Time.timeScale < 1 && gameMode != null && !gameMode.isPaused)
        {
            Time.timeScale += (1 / collisionSlowMotionDuration) * Time.unscaledDeltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!gameMode.isGameOver)
        {
            relativeExTime += Time.fixedDeltaTime * timeDistortion * movementManager.velocityTimeDistrotion;
            properTime += Time.fixedDeltaTime;
            if (level.category == Level.LevelCategory.TIME && properTime >= level.targetTime)
            {
                LevelCompleted();
            }
            else if (level.category == Level.LevelCategory.TIME_DILATED && (relativeExTime - properTime) >= level.targetTimeDilated)
            {
                LevelCompleted();
            }
        }
    }

    //Collision and triggers
    private void OnCollisionEnter(Collision collision)
    {
        if (gameMode.isGameOver)
        {
            return;
        }

        if (collision.collider.tag == "Obstacle")
        {
            movementManager.ResetMovementSpeed();
            dangerZoneCount--;
            GameObstacle obstacle = collision.gameObject.GetComponent<GameObstacle>();
            gravityFieldsGens.Remove(obstacle);

            if (dangerZoneCount == 0)
            {
                hudManagerInstance.EnableHighGravityFieldPanel(false);
            }

            CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);
            Time.timeScale = 1 / collisionTimerMultiplier;

            if (!extraManager.isShielded)
            {
                float damage = 0;
                switch (obstacle.type)
                {
                    case Obstacle.ObstacleType.PLANET:
                        damage = 60f;
                        break;
                    case Obstacle.ObstacleType.STAR:
                        damage = 80f;
                        break;
                    case Obstacle.ObstacleType.WHITE_DWARF:
                        damage = 120f;
                        break;
                    case Obstacle.ObstacleType.NEUTRON_STAR:
                        damage = 160f;
                        break;
                }
                TakeDamage(damage);
            }
            else
            {
                extraManager.DestroyShield();
                gameMode.BonusScore(GameplayMath.GetInstance().GetBonusPointsFromObstacleMass(obstacle.mass));
                skillManager.sessionObstaclesDestroyed++;
            }
            sessionObstaclesHit++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameMode.isGameOver) return;

        if (other.gameObject.tag.Equals("GravityField"))
        {
            GameObstacle obstacleObj = SharedUtilities.GetInstance().GetFirstComponentInParentWithTag<GameObstacle>(other.gameObject, "Obstacle");
            if(!gravityFieldsGens.Contains(obstacleObj))
            {
                gravityFieldsGens.Add(obstacleObj);
            }  
        }

        else if (other.gameObject.tag.Equals("DangerZone"))
        {
            dangerZoneCount++;
            hudManagerInstance.EnableHighGravityFieldPanel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameMode.isGameOver) return;

        if (other.gameObject.tag.Equals("GravityField"))
        {
            GameObstacle obstacleObj = SharedUtilities.GetInstance().GetFirstComponentInParentWithTag<GameObstacle>(other.gameObject, "Obstacle");
            gravityFieldsGens.Remove(obstacleObj);
        }
        else if (other.gameObject.tag.Equals("DangerZone"))
        {
            if (dangerZoneCount == 1 && !skillManager.isAntiGravityActive)
            {
                movementManager.GravitySling();
            }
            dangerZoneCount = dangerZoneCount > 0 ? dangerZoneCount - 1 : 0;

            if (dangerZoneCount == 0)
            {
                hudManagerInstance.EnableHighGravityFieldPanel(false);
            }
        }
        if (other.gameObject.tag.Equals("Margins"))
        {
            directionArrow.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameMode.isGameOver) return;

        if (other.gameObject.tag.Equals("Margins") && !isDead)
        {
            directionArrow.gameObject.SetActive(true);
            float x = transform.position.x;
            float y = transform.position.y;
            Vector3 moveDir = new Vector3(-x, -y, 0);
            moveDir.Normalize();
            directionArrow.transform.localPosition = moveDir * 3.5f;
            float degrees = GameplayMath.GetInstance().arctan(x, y);
            directionArrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, degrees));
        }
    }

    public void EnteredDamageNebula(Margin.MarginLocation location)
    {
        GameObject damageNebulaRef = null;
        if (location == Margin.MarginLocation.TOP)
        {
            damageNebulaRef = Instantiate(damageNebula, topNebulaSpawner.transform.position, Quaternion.identity);
            damageNebulaRef.transform.SetParent(topNebulaSpawner.transform);
            damageNebulaRef.transform.localScale = new Vector3(1f, 1f, 1f);
            damageNebulaRef.transform.localRotation = Quaternion.identity;
            verticalDamageNebulaEffect = damageNebulaRef.GetComponent<ParticleSystem>();
        }
        else if (location == Margin.MarginLocation.BOTTOM)
        {
            damageNebulaRef = Instantiate(damageNebula, bottomNebulaSpawner.transform.position, Quaternion.identity);
            damageNebulaRef.transform.SetParent(bottomNebulaSpawner.transform);
            damageNebulaRef.transform.localScale = new Vector3(1f, 1f, 1f);
            damageNebulaRef.transform.localRotation = Quaternion.identity;
            verticalDamageNebulaEffect = damageNebulaRef.GetComponent<ParticleSystem>();
        }


        if (location == Margin.MarginLocation.LEFT)
        {
            damageNebulaRef = Instantiate(damageNebula, leftNebulaSpawner.transform.position, Quaternion.identity);
            damageNebulaRef.transform.SetParent(leftNebulaSpawner.transform);
            damageNebulaRef.transform.localScale = new Vector3(1f, 1f, 1f);
            damageNebulaRef.transform.localRotation = Quaternion.identity;
            latDamageNebulaEffect = damageNebulaRef.GetComponent<ParticleSystem>();
        }
        else if (location == Margin.MarginLocation.RIGHT)
        {
            damageNebulaRef = Instantiate(damageNebula, rightNebulaSpawner.transform.position, Quaternion.identity);
            damageNebulaRef.transform.SetParent(rightNebulaSpawner.transform);
            damageNebulaRef.transform.localScale = new Vector3(1f, 1f, 1f);
            damageNebulaRef.transform.localRotation = Quaternion.identity;
            latDamageNebulaEffect = damageNebulaRef.GetComponent<ParticleSystem>();
        }


        directionArrow.gameObject.SetActive(true);
    }

    public void ExitedDamageNebula(Margin.MarginLocation location)
    {
        if ((location == Margin.MarginLocation.TOP || location == Margin.MarginLocation.BOTTOM) && verticalDamageNebulaEffect != null)
        {
            verticalDamageNebulaEffect.Stop();
        }

        if ((location == Margin.MarginLocation.LEFT || location == Margin.MarginLocation.RIGHT) && latDamageNebulaEffect != null)
        {
            latDamageNebulaEffect.Stop();
        }
        directionArrow.gameObject.SetActive(false);
    }

    public void Attempt()
    {
        Executer.GetInstance().AddJob(() =>
        {
            ParticleSystem system = null;
            system = rightNebulaSpawner.GetComponentInChildren<ParticleSystem>();
            if (system != null)
            {
                Destroy(system.gameObject);
            }

            system = leftNebulaSpawner.GetComponentInChildren<ParticleSystem>();
            if (system != null)
            {
                Destroy(system.gameObject);
            }

            system = topNebulaSpawner.GetComponentInChildren<ParticleSystem>();
            if (system != null)
            {
                Destroy(system.gameObject);
            }

            system = bottomNebulaSpawner.GetComponentInChildren<ParticleSystem>();
            if (system != null)
            {
                Destroy(system.gameObject);
            }

            gameObject.GetComponent<SphereCollider>().enabled = true;
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            transform.position = new Vector3(0f, 0f, 0f);
        });
        StartCoroutine(UpdateTimeDistortion());
        isDead = false;
        resilience = initialResilience / 2f;
        hudManagerInstance.UpdatePlayerHealthUI(resilience, initialResilience);
        movementManager.EnableMovement();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        resilience -= amount;
        resilience = resilience < 0 ? 0f : resilience;

        hudManagerInstance.UpdatePlayerHealthUI(resilience, initialResilience);

        if (resilience <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        timeDistortion = 1f;
        scoreMultiplier = 1f;
        isDead = true;
        directionArrow.gameObject.SetActive(false);

        movementManager.DisableMovement();

        Instantiate(deathEffect, transform.position, transform.rotation);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (level.category == Level.LevelCategory.ENDLESS)
        {
            LevelsData levelsData = SaveManager.GetInstance().LoadPersistentData(SaveManager.LEVELSDATA_PATH).GetData<LevelsData>();
            levelsData.UpdateLevelScore(level.id, (int)gameMode.sessionScore);
            SaveManager.GetInstance().SavePersistentData<LevelsData>(levelsData, SaveManager.LEVELSDATA_PATH);
        }

        ParticleSystem[] effects = skillManager.skillSpawn.GetComponentsInChildren<ParticleSystem>();
        if (effects != null)
        {
            int length = effects.Length;
            for (int i = 0; i < length; i++)
            {
                Destroy(effects[i].gameObject);
            }
        }

        dangerZoneCount = 0;
        gravityFieldsGens.Clear();

        if (level.category == Level.LevelCategory.ENDLESS)
        {
            BroadcastStats();
        }

        gameMode.EndSession();
    }

    public void LevelCompleted()
    {
        movementManager.DisableMovement();

        timeDistortion = 1f;
        directionArrow.gameObject.SetActive(false);
        ParticleSystem[] effects = skillManager.skillSpawn.GetComponentsInChildren<ParticleSystem>();
        if (effects != null)
        {
            int length = effects.Length;
            for (int i = 0; i < length; i++)
            {
                Destroy(effects[i].gameObject);
            }
        }
        dangerZoneCount = 0;
        gravityFieldsGens.Clear();

        gameMode.LevelCompleted();
    }

    public int CalculateLevel(int gravityPoints, GameMode.GradeObtained obt)
    {
        PlayerData data = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        int exp = GameplayMath.GetInstance().GetExp(gravityPoints, obt);
        int res = data.CalculateLevel(exp);
        SaveManager.GetInstance().SavePersistentData<PlayerData>(data, SaveManager.PLAYER_DATA);
        return res;
    }

    private void BroadcastStats()
    {
        //Send broadcast stats
        SessionStats stats = new SessionStats();
        stats.maxSpeedReached = movementManager.GetMaxSpeedReached();
        stats.maxSpeed = movementManager.maxSpeed;
        stats.timePlayed = properTime;
        stats.distortedTime = relativeExTime - properTime;
        stats.score = gameMode.sessionScore;
        stats.obstaclesHit = sessionObstaclesHit;
        PersistentPlayerPrefs.GetInstance().CheckAchievements(stats);
    }

    private IEnumerator UpdateTimeDistortion()
    {
        while (!gameMode.isGameOver)
        {
            if (isGravityTdActive)
            {
                float totTd = 1f;
                int length = gravityFieldsGens.Count;
                for (int i = 0; i < length; i++)
                {
                    totTd *= GameplayMath.GetInstance().GetGravityTd(gameObject, gravityFieldsGens[i]);
                }
                timeDistortion = totTd > 1 ? totTd : 1f;
                scoreMultiplier = timeDistortion;
            }
            else
            {
                timeDistortion = 1f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}

