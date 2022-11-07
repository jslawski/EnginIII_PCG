using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentMinimize : StatusEffect
{
    private float minimizedScale = 0.3f;

    public override void ApplyStatusEffect()
    {
        base.ApplyStatusEffect();

        this.nonOwnerTarget.gameObject.transform.localScale = new Vector3(this.minimizedScale, this.minimizedScale, this.minimizedScale);
    }

    protected override void CleanupStatusEffect()
    {
        base.CleanupStatusEffect();

        this.nonOwnerTarget.gameObject.transform.localScale = Vector3.one;
    }
}
