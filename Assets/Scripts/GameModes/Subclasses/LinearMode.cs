using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMode : GameMode
{
    protected override void InstantiateObstacle()
    {
        Quaternion rot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        string poolTag = poolManager.GetRandomCategoryPoolTag(GAMEOBSTACLES_CAT_TAG);
        if(poolTag.Equals("OrbNeutronStar"))
        {
            HUDManager.GetInstance().Toast(HUDManager.ToastType.GAME_TOAST, "Orbital NeutronStars incoming", null, 1.5f, 0f, false);
        }
        GameObject instantiatedObstacleRef = poolManager.Spawn(GAMEOBSTACLES_CAT_TAG, poolTag, obstacleSpawner.GetSpawnPosition(), rot);

        LinearMovementComponent linearMovementComponent = instantiatedObstacleRef.GetComponent<LinearMovementComponent>();
        if (linearMovementComponent == null)
        {
            instantiatedObstacleRef.AddComponent<LinearMovementComponent>();
        }
    }

    protected override void InstantiatePickUp()
    {
        GameObject instantiatedPickUpRef = poolManager.Spawn(PICKUPS_CAT_TAG, extraSpawner.GetSpawnPosition(), Quaternion.identity);
        LinearMovementComponent linearMovementComponent = instantiatedPickUpRef.GetComponent<LinearMovementComponent>();
        if (linearMovementComponent == null)
        {
            instantiatedPickUpRef.AddComponent<LinearMovementComponent>();
        }
    }
}
