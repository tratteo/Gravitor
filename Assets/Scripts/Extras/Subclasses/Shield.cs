using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Extra
{
    [Header("Shield")]
    public GameObject shieldEffect = null;
    public GameObject destroyShieldEffect = null;

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    public void DestroyShield(Vector3 position)
    {
        Instantiate(destroyShieldEffect, position, Quaternion.identity);
    }
}
