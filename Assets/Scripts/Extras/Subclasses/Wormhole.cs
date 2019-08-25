using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wormhole : Extra
{
    [Header("Warp Drive")]
    public float duration;
    public float viewDistortion;
    public float scoreMultiplier;
    [Range(0f, 1f)]
    [Tooltip("Speed in respect of the speed of light")]
    public float speed;

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
