using System.Collections;
using UnityEngine;

/// <summary>
/// Obstacle class
/// </summary>
public class GameObstacle : Obstacle, IDestroyEffect
{
    //USING CUSTOM EDITOR
    public float minDensity;
    public float maxDensity;
    public GameObject deathEffect;
    public float density;
    public float mass;

    public new Rigidbody rigidbody;

    public ObstacleGravity gravityComponent;

    private new void Awake()
    {
        base.Awake();
        gravityComponent = GetComponentInChildren<ObstacleGravity>();
        rigidbody = GetComponent<Rigidbody>();
    }

    //Collision and triggers

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.collider.tag == "Player")
        {
            Destroy(true);
        }
    }

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
    //
    public override void SetupObstacle()
    {
        density = Random.Range(minDensity, maxDensity);
        mass = GameplayMath.GetInstance().GetObstacleMass(this);
        gravityComponent.fieldID = (int)((mass * density) / 1000000000);

        SphereCollider dangerZone = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<SphereCollider>(gameObject, "DangerZone");
        SphereCollider gravityField = SharedUtilities.GetInstance().GetFirstComponentInChildrenWithTag<SphereCollider>(gameObject, "GravityField");
        gravityField.radius = (0.75E-2f * Mathf.Sqrt(mass)) / targetScale;

        if (gravityField.radius <= 0.8f)
        {
            gravityField.radius = 2f;
        }

        dangerZone.radius = gravityField.radius / 12.5f;
        if (dangerZone.radius <= 0.55f)
        {
            dangerZone.radius = 0.6f;
        }

        base.SetupObstacle();

    }

    public void Destroy(bool instantiateEffect)
    {
        if (instantiateEffect)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
            switch (gameMode.GetType().Name)
            {
                case "LinearMode":
                    effect.AddComponent<LinearMovementComponent>();
                    break;
            }
        }
        base.DeactivateObstacle();
    }
}

