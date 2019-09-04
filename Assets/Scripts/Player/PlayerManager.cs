using EZCameraShake;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementManager))]
[RequireComponent(typeof(SkillManager))]
[RequireComponent(typeof(ExtraManager))]
[RequireComponent(typeof(AchievementsManager))]
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
    public Dictionary<int, float> gravityFieldsSet;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public short dangerZoneCount = 0;
    [HideInInspector] public short gravityFieldCount = 0;
    [HideInInspector] public HUDManager hudManagerInstance;
    [HideInInspector] public bool isDead = false;
    private int sessionObstaclesHit = 0;

    [HideInInspector] public float scoreMultiplier = 1f;
    [HideInInspector] public float timeDistortion = 1f;

    [HideInInspector] public float properTime = 0f;
    [HideInInspector] public float relativeExTime = 0f;
    [HideInInspector] public float velocityRelativeTime = 0f;

    [HideInInspector] public Level level;

    //Events

    private event Action OnPlayerDeath;
    public void SubscribeToPlayerDeathEvent(Action funcToSub) { OnPlayerDeath += funcToSub; }
    public void UnsubscribeToPlayerDeathEvent(Action funcToUnsub) { OnPlayerDeath -= funcToUnsub; }

    public struct SessionStats
    {
        public float maxSpeedReached;
        public float timePlayed;
        public float distortedTime;
        public float score;
        public int obstaclesHit;
    }
    private event Action<SessionStats> FireSessionStats;
    public void SubscribeToSessionStatsEvent(Action<SessionStats> funcToSub) { FireSessionStats += funcToSub; }
    public void UnsubscribeToSessionStatsEvent(Action<SessionStats> funcToUnsub) { FireSessionStats += funcToUnsub; }

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
            initialResilience = playerData.health;
        }
    }

    private void Start()
    {
        resilience = initialResilience;
        gravityFieldsSet = new Dictionary<int, float>();
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
    }

    void Update()
    {
        timeDistortion = timeDistortion < 1f ? 1f : timeDistortion;
        //Debug.Log("Td: " + timeDistortion + "Sm: " + scoreMultiplier);
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
            gravityFieldCount--;
            if (dangerZoneCount == 0)
            {
                hudManagerInstance.EnableHighGravityFieldPanel(false);
            }

            ObstacleGravity obstacleGravity = collision.gameObject.GetComponentInChildren<ObstacleGravity>();
            gravityFieldsSet.Remove(obstacleGravity.fieldID);
            timeDistortion = 1f;
            CameraShaker.Instance.ShakeOnce(shakeMagnitude, shakeRoughness, shakeFadeInTime, shakeFadeOutTime);
            Time.timeScale = 1 / collisionTimerMultiplier;
            GameObstacle obstacle = null;
            obstacle = collision.gameObject.GetComponent<GameObstacle>();
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
            }
            sessionObstaclesHit++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameMode.isGameOver)
        {
            return;
        }

        if (other.gameObject.tag.Equals("GravityField"))
        {
            ObstacleGravity gravityComponent = other.gameObject.GetComponent<ObstacleGravity>();

            float timeDistortion = GameplayMath.GetInstance().GetGravityTd(gameObject, other.transform.parent.gameObject);
            if (!gravityFieldsSet.ContainsKey(gravityComponent.fieldID))
            {
                gravityFieldsSet.Add(gravityComponent.fieldID, timeDistortion);
            }

            gravityFieldCount++;
        }
        else if (other.gameObject.tag.Equals("DangerZone"))
        {
            dangerZoneCount++;
            hudManagerInstance.EnableHighGravityFieldPanel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameMode.isGameOver)
        {
            return;
        }

        if (other.gameObject.tag.Equals("GravityField"))
        {
            ObstacleGravity gravityComponent = other.gameObject.GetComponent<ObstacleGravity>();
            gravityFieldsSet.Remove(gravityComponent.fieldID);

            if (gravityFieldCount > 0)
            {
                gravityFieldCount--;
            }
            if (gravityFieldCount == 0)
            {
                timeDistortion = 1f;
            }
        }
        else if (other.gameObject.tag.Equals("DangerZone"))
        {
            if (dangerZoneCount == 1 && !skillManager.isAntiGravityActive)
            {
                movementManager.GravitySling();
            }
            if (dangerZoneCount > 0)
            {
                dangerZoneCount--;
            }
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
        if (gameMode.isGameOver)
        {
            return;
        }

        if (other.gameObject.tag.Equals("GravityField"))
        {
            float newTimeDistortion = GameplayMath.GetInstance().GetGravityTd(gameObject, other.transform.parent.gameObject);
            ObstacleGravity gravityComponent = other.gameObject.GetComponent<ObstacleGravity>();
            //Subtract current obstacle Td
            if (gravityFieldsSet.ContainsKey(gravityComponent.fieldID))
            {
                timeDistortion -= gravityFieldsSet[gravityComponent.fieldID];
                //Update current obstacle Td
                gravityFieldsSet[gravityComponent.fieldID] = newTimeDistortion;
                //Add back correct Td
                timeDistortion += newTimeDistortion;
                scoreMultiplier = timeDistortion;
            }
        }
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
    //

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
        isDead = true;
        directionArrow.gameObject.SetActive(false);
        //FIRE EVENT
        OnPlayerDeath();

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
        gravityFieldCount = 0;
        if (level.category == Level.LevelCategory.ENDLESS)
        {
            BroadcastStats();
        }
    }

    public void LevelCompleted()
    {
        movementManager.DisableMovement();

        timeDistortion = 1f;
        directionArrow.gameObject.SetActive(false);
        //gameObject.GetComponent<SphereCollider>().enabled = false;
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
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
        gravityFieldCount = 0;
        gameMode.LevelCompleted();
    }

    private void BroadcastStats()
    {
        //Send broadcast stats
        SessionStats stats = new SessionStats();
        stats.maxSpeedReached = movementManager.GetMaxSpeedReached();
        stats.timePlayed = properTime;
        stats.distortedTime = relativeExTime - properTime;
        stats.score = gameMode.sessionScore;
        stats.obstaclesHit = sessionObstaclesHit;
        FireSessionStats(stats);
    }
}

