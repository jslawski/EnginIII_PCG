using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLifeSteal : TimedStatusEffect
{
    private int sapDamage = 1;

    private float delayBetweenSapInSeconds = 0.3f;

    public override void ApplyStatusEffect()
    {
        this.duration = 4.0f;

        StartCoroutine(this.ApplySap());

        base.ApplyStatusEffect();
    }

    private IEnumerator ApplySap()
    {
        while (true)
        {
            this.nonOwnerTarget.TakeDamage(this.ownerTarget, this.sapDamage, true);
            this.ownerTarget.currentHitPoints += this.sapDamage;
            yield return new WaitForSeconds(this.delayBetweenSapInSeconds);
        }
    }
}
