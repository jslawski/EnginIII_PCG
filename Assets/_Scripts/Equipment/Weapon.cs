using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Equipment/Weapon")]
public class Weapon : Equipment
{
    public int attackPower;
    [Range(0.0f, 1.0f)]
    public float critChance;
    public Vector2 attackZoneDimensions;

    public float GetUtilityTotalScore()
    {
        float utilityScore = 0.0f;
        float attackArea = this.attackZoneDimensions.x * this.attackZoneDimensions.y;

        utilityScore += (float)this.rarity / 4.0f;
        utilityScore += (float)attackPower / 2.0f;
        utilityScore += critChance * 10.0f;
        utilityScore += attackArea;

        for (int i = 0; i < this.enchantmentSlots.Length; i++)
        {
            if (this.enchantmentSlots[i] != null)
            {
                utilityScore += this.enchantmentSlots[i].utilityScore;
            }
        }

        return utilityScore;
    }
}
