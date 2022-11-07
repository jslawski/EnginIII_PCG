using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOffLifeSteal : StatusEffect
{
    private float stealPercentage = 0.5f;

    public override void ApplyStatusEffect()
    {
        int stealAmount = Mathf.RoundToInt(this.stealPercentage * (float)this.ownerTarget.equippedWeapon.attackPower);
        this.ownerTarget.currentHitPoints += stealAmount;

        Destroy(this);
    }
}
