using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedBurn : TimedStatusEffect
{
    private int fireDamage = 5;

    private float delayBetweenBurnsInSeconds = 0.5f;

    public override void ApplyStatusEffect()
    {
        this.duration = 10.0f;

        StartCoroutine(this.ApplyBurn());

        base.ApplyStatusEffect();
    }

    private IEnumerator ApplyBurn()
    {
        while (true)
        {
            this.nonOwnerTarget.TakeDamage(this.ownerTarget, this.fireDamage, true);
            yield return new WaitForSeconds(this.delayBetweenBurnsInSeconds);
        }
    }    
}
