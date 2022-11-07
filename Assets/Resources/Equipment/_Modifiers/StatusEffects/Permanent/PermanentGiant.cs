using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentGiant : StatusEffect
{
    private float newScale = 3.0f;

    public override void ApplyStatusEffect()
    {
        base.ApplyStatusEffect();

        this.nonOwnerTarget.gameObject.transform.localScale = new Vector3(this.newScale, this.newScale, this.newScale);
    }

    protected override void CleanupStatusEffect()
    {
        base.CleanupStatusEffect();

        this.nonOwnerTarget.gameObject.transform.localScale = Vector3.one;
    }
}
