using UnityEngine;
using UnityEngine.Profiling;

public class LinearMovementComponent : MonoBehaviour
{
    private Vector3 movementDirection;
    private LinearMode linearMode;
    private Vector3 rotationVelocity;

    private void Start()
    {
        linearMode = FindObjectOfType<LinearMode>();
        movementDirection = -Vector3.forward;
    }

    private void FixedUpdate()
    {

        transform.position += movementDirection * linearMode.playerManager.movementManager.movementSpeed * Time.fixedDeltaTime;
    }
}
