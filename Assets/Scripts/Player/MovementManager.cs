using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerManager))]
public class MovementManager : MonoBehaviour
{
    public const float MAX_SPEED = 800;

    [Header("Movement settings")]
    public Joystick joystick;
    public float initialMovementSpeed = 110f;
    [SerializeField] private float gravitySlingSpeedMultiplier = 1.15f;

    [HideInInspector] public float currentSlingMultiplier;
    private new Rigidbody rigidbody;
    private readonly float zThreshHold = 10f;
    private float thrustForce = 120f;
    private float maxSpeedReached = 0f;

    [HideInInspector] public float movementSpeed;
    private bool canMove = true;
    private float forceIntensity;
    private Vector3 zAdjust;

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
    }

    public void GravitySling()
    {
        if (movementSpeed >= MAX_SPEED)
        {
            maxSpeedReached = MAX_SPEED;
            return;
        }

        movementSpeed *= gravitySlingSpeedMultiplier;
        maxSpeedReached = movementSpeed;
        currentSlingMultiplier += gravitySlingSpeedMultiplier;
    }
    public float GetMaxSpeedReached()
    {
        return maxSpeedReached;
    }

    public void ResetMovementSpeed()
    {
        movementSpeed = initialMovementSpeed;
        currentSlingMultiplier = 1f;
    }

    public void DisableMovement() { canMove = false; }
    public void EnableMovement() { canMove = true; }

    public void SpeedEffect(float duration, float multiplier)
    {
        StartCoroutine(SpeedEffectCoroutine(duration, multiplier));
    }

    private IEnumerator SpeedEffectCoroutine(float duration, float multiplier)
    {
        canMove = false;
        movementSpeed *= multiplier;
        yield return new WaitForSeconds(duration);
        movementSpeed /= multiplier;
        canMove = true;
    }
}
