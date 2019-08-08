using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDrive : Extra
{
    [Header("Warp Drive")]
    public float duration;
    public float viewDistortion;
    public float scoreMultiplier;

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
