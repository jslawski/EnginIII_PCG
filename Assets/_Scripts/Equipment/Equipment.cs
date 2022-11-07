using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType { Weapon, Armor, None };
public enum Rarity { Common, Uncommon, Rare, Legendary };

public abstract class Equipment : ScriptableObject
{
    [HideInInspector]
    public Creature owner;

    public Rarity rarity;
    public EquipmentType equipmentType;

    public Enchantment[] enchantmentSlots;

    private List<Enchantment> equipEnchantments;
    private List<Enchantment> attackEnchantments;
    private List<Enchantment> damageEnemyEnchantments;
    private List<Enchantment> damageSelfEnchantments;

    private GameObject statusEffectManager;

    private List<StatusEffect> equippedStatusEffects;

    private void OrganizeEnchantments()
    {
        this.equipEnchantments = new List<Enchantment>();
        this.attackEnchantments = new List<Enchantment>();
        this.damageEnemyEnchantments = new List<Enchantment>();
        this.damageSelfEnchantments = new List<Enchantment>();

        for (int i = 0; i < this.enchantmentSlots.Length; i++)
        {
            if (this.enchantmentSlots[i] != null)
            {
                for (int j = 0; j < this.enchantmentSlots[i].triggers.Length; j++)
                {
                    switch (this.enchantmentSlots[i].triggers[j])
                    {
                        case EnchantmentTrigger.ON_EQUIP:
                            this.equipEnchantments.Add(this.enchantmentSlots[i]);
                            break;
                        case EnchantmentTrigger.ON_ATTACK:
                            this.attackEnchantments.Add(this.enchantmentSlots[i]);
                            break;
                        case EnchantmentTrigger.ON_DAMAGE_ENEMY:
                            this.damageEnemyEnchantments.Add(this.enchantmentSlots[i]);
                            break;
                        case EnchantmentTrigger.ON_DAMAGE_SELF:
                            this.damageSelfEnchantments.Add(this.enchantmentSlots[i]);
                            break;
                        default:
                            Debug.LogError("Unknown EnchantmentTrigger: " + this.enchantmentSlots[i].triggers[j] + " Unable to setup enchantment.");
                            break;
                    }
                }
            }
        }
    }

    private void SetupEnchantmentTriggers()
    {
        if (this.equipEnchantments.Count > 0)
        {
            this.owner.onEquipTriggered += this.TriggerEquipEnchantments;
        }
        if (this.attackEnchantments.Count > 0)
        {
            this.owner.onAttackTriggered += this.TriggerAttackEnchantments;
        }
        if (this.damageEnemyEnchantments.Count > 0)
        {
            this.owner.onDamageEnemyTriggered += this.TriggerDamageEnemyEnchantments;
        }
        if (this.damageSelfEnchantments.Count > 0)
        {
            this.owner.onDamageSelfTriggered += this.TriggerDamageSelfEnchantments;
        }        
    }

    private void CleanUpEnchantments()
    {
        if (this.equipEnchantments.Count > 0)
        {
            this.owner.onEquipTriggered -= this.TriggerEquipEnchantments;
        }
        if (this.attackEnchantments.Count > 0)
        {
            this.owner.onAttackTriggered -= this.TriggerAttackEnchantments;
        }
        if (this.damageEnemyEnchantments.Count > 0)
        {
            this.owner.onDamageEnemyTriggered -= this.TriggerDamageEnemyEnchantments;
        }
        if (this.damageSelfEnchantments.Count > 0)
        {
            this.owner.onDamageSelfTriggered -= this.TriggerDamageSelfEnchantments;
        }

        //Destroy all equipped status effects
        for (int i = 0; i < this.equippedStatusEffects.Count; i++)
        {
            if (this.equippedStatusEffects[i] != null)
            {
                Destroy(this.equippedStatusEffects[i]);
            }
        }
    }

    public void Equip(Creature equippingCreature)
    {
        this.equippedStatusEffects = new List<StatusEffect>();

        this.statusEffectManager = GameObject.Find("StatusEffectManager");

        this.OrganizeEnchantments();

        this.owner = equippingCreature;

        this.SetupEnchantmentTriggers();
    }

    public void Unequip()
    {
        this.CleanUpEnchantments();
        this.owner = null;
    }

    private void TriggerEquipEnchantments()
    {
        for (int i = 0; i < this.equipEnchantments.Count; i++)
        {
            StatusEffect effect = this.owner.gameObject.AddComponent(Type.GetType(this.equipEnchantments[i].statusEffectName)) as StatusEffect;
            effect.SetupStatusEffect(this.owner, this.owner);
            effect.ApplyStatusEffect();

            this.equippedStatusEffects.Add(effect);

            this.owner.onEquipTriggered -= this.TriggerEquipEnchantments;
        }
    }

    private void TriggerAttackEnchantments()
    {
        for (int i = 0; i < this.attackEnchantments.Count; i++)
        {
            StatusEffect effect = this.statusEffectManager.AddComponent(Type.GetType(this.attackEnchantments[i].statusEffectName)) as StatusEffect;            
            effect.SetupStatusEffect(this.owner, this.owner);
            effect.ApplyStatusEffect();
        }
    }

    private void TriggerDamageEnemyEnchantments(Creature targetCreature, int damageDealt)
    {
        for (int i = 0; i < this.damageEnemyEnchantments.Count; i++)
        {
            StatusEffect effect = this.statusEffectManager.AddComponent(Type.GetType(this.damageEnemyEnchantments[i].statusEffectName)) as StatusEffect;
            effect.SetupStatusEffect(this.owner, targetCreature);
            effect.ApplyStatusEffect();
        }
    }

    private void TriggerDamageSelfEnchantments(Creature sourceCreature, int damageDealt)
    {
        for (int i = 0; i < this.damageEnemyEnchantments.Count; i++)
        {
            StatusEffect effect = this.statusEffectManager.AddComponent(Type.GetType(this.damageEnemyEnchantments[i].statusEffectName)) as StatusEffect;
            effect.SetupStatusEffect(this.owner, sourceCreature);
            effect.ApplyStatusEffect();
        }
    }    
}
