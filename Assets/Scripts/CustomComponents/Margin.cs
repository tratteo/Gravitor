using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Margin : MonoBehaviour
{
    public enum MarginLocation { LEFT, RIGHT, TOP, BOTTOM }
    [SerializeField] private MarginLocation type;
    [SerializeField] private float dealRate = 4;

    private float currentTime;
    private float dealTime;

    private PlayerManager playerManager;

    void Start()
    {
        currentTime = Time.timeSinceLevelLoad;
        dealTime = 1 / dealRate;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            playerManager = other.gameObject.GetComponent<PlayerManager>();
            playerManager.EnteredDamageNebula(type);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            playerManager.ExitedDamageNebula(type);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (Time.timeSinceLevelLoad >= currentTime + dealTime)
            {
                float distanceFromCentre = Vector3.Magnitude(other.transform.position);
                float damage = GameplayMath.GetInstance().GetDamageWithDistance(distanceFromCentre);
                playerManager.TakeDamage(damage);
                currentTime = Time.timeSinceLevelLoad;
            }
        }
    }
}
