using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOffDamage : StatusEffect
{
    private int selfDamage = 5;

    public override void ApplyStatusEffect()
    {
        this.ownerTarget.TakeDamage(this.ownerTarget, this.selfDamage, false);

        Destroy(this);
    }
}
