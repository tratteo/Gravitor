using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusGravityPoints : Extra
{
    [Header("Bonus Gravity Points")]
    public Vector2 bonusGpRange;

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}
