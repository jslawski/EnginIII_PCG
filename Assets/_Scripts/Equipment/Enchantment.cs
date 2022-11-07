using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnchantmentTrigger { ON_EQUIP, ON_ATTACK, ON_DAMAGE_ENEMY, ON_DAMAGE_SELF }

[CreateAssetMenu(fileName = "New Enchantment", menuName = "Enchantment")]
public class Enchantment : ScriptableObject
{
    public EnchantmentTrigger[] triggers;
    public string abilityDisplayName;
    public string statusEffectName;
    public float utilityScore;
}
