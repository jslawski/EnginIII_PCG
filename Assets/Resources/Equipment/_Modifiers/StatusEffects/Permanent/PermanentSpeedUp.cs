using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentSpeedUp : StatusEffect
{
    private float speedModifier = 3.0f;

    public override void ApplyStatusEffect()
    {
        this.ownerTarget.moveSpeed += this.speedModifier;
    }

    protected override void CleanupStatusEffect()
    {
        this.ownerTarget.moveSpeed -= this.speedModifier;
    }
}
