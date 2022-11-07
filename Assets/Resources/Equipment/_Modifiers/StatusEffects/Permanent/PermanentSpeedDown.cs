using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentSpeedDown : StatusEffect
{
    private float speedModifier = 2.0f;

    public override void ApplyStatusEffect()
    {
        this.ownerTarget.moveSpeed -= this.speedModifier;
    }

    protected override void CleanupStatusEffect()
    {
        this.ownerTarget.moveSpeed += this.speedModifier;
    }
}
