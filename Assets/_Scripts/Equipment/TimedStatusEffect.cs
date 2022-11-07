using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedStatusEffect : StatusEffect
{
    protected float duration = 0.0f;

    public override void ApplyStatusEffect()
    {
        StartCoroutine(this.DepleteTimer());
    }

    protected override void CleanupStatusEffect()
    {
        StopAllCoroutines();
    }

    protected IEnumerator DepleteTimer()
    {
        while (this.duration > 0.0f)
        {
            yield return new WaitForFixedUpdate();
            this.duration -= Time.fixedDeltaTime;
        }

        Destroy(this);
    }
}
