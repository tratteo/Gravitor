using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Obstacle gravity implementation
/// </summary>
public class ObstacleGravity : MonoBehaviour
{
    private GameplayMath gameplayMathInstance;
    private Rigidbody otherRigidbody;
    private SkillManager playerSkillManager;

    private void Start()
    {
        gameplayMathInstance = GameplayMath.GetInstance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
            playerSkillManager = other.gameObject.GetComponent<SkillManager>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (playerSkillManager.isGravitable && otherRigidbody != null)
            {
                float gravityMagnitude = gameplayMathInstance.GetGravityIntensity(otherRigidbody.gameObject, transform.parent.gameObject);
                otherRigidbody.AddForce(Vector3.Normalize(transform.position - otherRigidbody.transform.position) * gravityMagnitude, ForceMode.Acceleration);
            }
        }
    }
}
