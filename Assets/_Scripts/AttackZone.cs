using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    private Creature owningCreature;
    [HideInInspector]
    public BoxCollider attackCollider;
    [HideInInspector]
    public MeshRenderer attackRenderer;

    public bool attacking = false;

    public void Setup()
    {
        this.owningCreature = GetComponentInParent<Creature>();
        this.attackCollider = GetComponent<BoxCollider>();
        this.attackRenderer = GetComponent<MeshRenderer>();
    }

    public void EnableAttack()
    {
        this.attackCollider.enabled = true;
        this.attackRenderer.enabled = true;
        this.attacking = true;
    }

    public void DisableAttack()
    {
        this.attackCollider.enabled = false;
        this.attackRenderer.enabled = false;
        this.attacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hittable")
        {
            Creature hitCreature = other.gameObject.GetComponent<Creature>();
            if (hitCreature != this.owningCreature)
            {
                this.ProcessHit(hitCreature);
            }            
        }
    }

    private int CalculateDamage()
    {
        float critRoll = Random.Range(0.0f, 1.0f);

        if (this.owningCreature.equippedWeapon.critChance <= critRoll)
        {
            return this.owningCreature.equippedWeapon.attackPower * 2;
        }
        else
        {
            return this.owningCreature.equippedWeapon.attackPower;
        }
    }

    public void ProcessHit(Creature hitCreature)
    {
        if (hitCreature != null)
        {
            int damageDealt = this.CalculateDamage();
            hitCreature.TakeDamage(this.owningCreature, damageDealt, false);
            this.owningCreature.TriggerDamageEnemy(hitCreature, this.owningCreature.equippedWeapon.attackPower);
        }
    }
}
