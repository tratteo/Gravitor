using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingSysObstacle : MonoBehaviour, IPooledObject
{
    public GameObstacle planet1, planet2;
    public Vector2 orbitingSpeedRange;
    private float orbitingSpeed;


    public void OnObjectSpawn()
    {
        planet1.gameObject.SetActive(true);
        planet2.gameObject.SetActive(true);
        orbitingSpeed = Random.Range(orbitingSpeedRange.x, orbitingSpeedRange.y);
    }


    private void FixedUpdate()
    {
        planet1.transform.RotateAround(transform.position, transform.up, orbitingSpeed * Time.fixedDeltaTime);
        planet2.transform.RotateAround(transform.position, transform.up, orbitingSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KillVolume")
        {
            planet1.Destroy(false);
            planet2.Destroy(false);
            gameObject.SetActive(false);
        }
    }
}
