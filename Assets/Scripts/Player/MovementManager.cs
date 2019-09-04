using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerManager))]
public class MovementManager : MonoBehaviour
{
    [Header("Movement settings")]
    public Joystick joystick;
    public float initialMovementSpeed = 110f;
    [SerializeField] private float gravitySlingSpeedMultiplier = 1.15f;
    public float maxSpeed = 1500f;
    public float maxRelativeSpeed = 0.9f;

    /*[HideInInspector] */public float velocityTimeDistrotion = 1f;
    [HideInInspector] public float currentSlingMultiplier;
    private new Rigidbody rigidbody;
    private readonly float zThreshHold = 10f;
    private float thrustForce = 120f;
    private float maxSpeedReached = 0f;

    /*[HideInInspector]*/ public float movementSpeed;
   /* [HideInInspector]*/ public float relativeSpeed;
    private bool canMove = true;
    private float forceIntensity;
    private Vector3 zAdjust;
    [HideInInspector] public float distance = 0f;

    private PlayerManager playerManager;

    private Vector3 horizontal = new Vector3(1f, 0f, 0f), vertical = new Vector3(0f, 1f, 0f);

    void OnDisable()
    {
        playerManager.UnsubscribeToPlayerDeathEvent(DisableMovement);
    }

    private void Awake()
    {
        movementSpeed = initialMovementSpeed;
        currentSlingMultiplier = 1;
        maxSpeedReached = movementSpeed;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        playerManager.SubscribeToPlayerDeathEvent(DisableMovement);

        PlayerData playerData = SaveManager.GetInstance().LoadPersistentData(SaveManager.PLAYER_DATA).GetData<PlayerData>();
        if (playerData != null)
        {
            thrustForce = GameplayMath.GetInstance().GetPlayerThrustForceFromPoints(playerData.thrustForcePoints);
        }

        zAdjust = Vector3.zero;
        relativeSpeed = (initialMovementSpeed * 0.9f) / maxSpeed;
    }

    private void FixedUpdate()
    {
        forceIntensity = Mathf.Abs(transform.position.z) / 1.5f;
        if (forceIntensity > zThreshHold)
        {
            zAdjust.z = -transform.position.z;
            rigidbody.AddForce(zAdjust * forceIntensity, ForceMode.Acceleration);
        }

        if (canMove)
        {
            rigidbody.AddForce(horizontal * joystick.Horizontal * thrustForce, ForceMode.Acceleration);
            rigidbody.AddForce(vertical * joystick.Vertical * thrustForce, ForceMode.Acceleration);
        }
        if (!playerManager.gameMode.isGameOver)
        {
            distance += relativeSpeed * 3E+5f * Time.fixedDeltaTime;
        }

        if(playerManager.level.category == Level.LevelCategory.DISTANCE && distance >= playerManager.level.targetDistance)
        {
            playerManager.LevelCompleted();
        }
    }

    public void GravitySling()
    {
        if (movementSpeed * gravitySlingSpeedMultiplier >= maxSpeed)
        {
            VelocityChange(maxSpeed);
            maxSpeedReached = maxSpeed;
            if(playerManager.level.category == Level.LevelCategory.MAX_SPEED)
            {
                playerManager.LevelCompleted();
            }
            return;
        }

        movementSpeed *= gravitySlingSpeedMultiplier;
        VelocityChange(movementSpeed);
        currentSlingMultiplier += gravitySlingSpeedMultiplier;
    }

    public void VelocityChange(float speed)
    {
        movementSpeed = speed;
        relativeSpeed = (movementSpeed * maxRelativeSpeed) / maxSpeed;
        velocityTimeDistrotion = GameplayMath.GetInstance().GetSpeedTd(relativeSpeed);
    }

    public void RelativeVelocityChange(float relativeSpeed)
    {
        this.relativeSpeed = relativeSpeed;
        movementSpeed = (relativeSpeed * maxSpeed) / maxRelativeSpeed;
        velocityTimeDistrotion = GameplayMath.GetInstance().GetSpeedTd(relativeSpeed);
    }

    public float GetMaxSpeedReached()
    {
        return maxSpeedReached;
    }

    public void ResetMovementSpeed()
    {
        movementSpeed = initialMovementSpeed;
        relativeSpeed = (movementSpeed * maxRelativeSpeed) / maxSpeed;
        currentSlingMultiplier = 1f;
        velocityTimeDistrotion = 1f;
    }

    public void DisableMovement() { canMove = false; }
    public void EnableMovement() { canMove = true; }

    public void SpeedEffect(float duration, float speed, bool velocityDistort, bool relativeSpeed)
    {
        if (relativeSpeed)
        {
            StartCoroutine(RelativeSpeedEffect_C(duration, speed, velocityDistort));
        }
        else
        {
            StartCoroutine(SpeedEffect_C(duration, speed, velocityDistort));
        }     
    }

    private IEnumerator SpeedEffect_C(float duration, float speed, bool velocityDistort)
    {
        speed = speed > maxSpeed ? maxSpeed : speed;

        canMove = false;
        float currentSpeed = movementSpeed;
        if (velocityDistort)
        {
            VelocityChange(speed);
            yield return new WaitForSeconds(duration);
            VelocityChange(currentSpeed);
        }
        else
        {
            movementSpeed = speed;
            yield return new WaitForSeconds(duration);
            movementSpeed = currentSpeed;
        }
        canMove = true;
    }

    private IEnumerator RelativeSpeedEffect_C(float duration, float relativeSpeed, bool velocityDistort)
    {
        relativeSpeed = relativeSpeed > 1 ? 1 : relativeSpeed;

        canMove = false;
        float currentSpeed = this.relativeSpeed;
        if (velocityDistort)
        {
            RelativeVelocityChange(relativeSpeed);
            yield return new WaitForSeconds(duration);
            RelativeVelocityChange(currentSpeed);
        }
        else
        {
            movementSpeed = (relativeSpeed * maxSpeed) / maxRelativeSpeed;
            yield return new WaitForSeconds(duration);
            movementSpeed = (currentSpeed * maxSpeed) / maxRelativeSpeed;
        }
        canMove = true;
    }
}
