using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{ 
    protected Creature ownerTarget;
    protected Creature nonOwnerTarget;

    public virtual void ApplyStatusEffect() { }
    protected virtual void CleanupStatusEffect() { }

    public void SetupStatusEffect(Creature owner, Creature nonOwner)
    {
        this.ownerTarget = owner;
        this.nonOwnerTarget = nonOwner;
    }

    protected void OnDestroy()
    {
        this.CleanupStatusEffect();
    }
}
