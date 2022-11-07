using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Loot Chest class.  Make this abstract in the future when chest rarities are a thing.
/// </summary>
public class LootChest : MonoBehaviour
{
    private const string WeaponsResourcePath = "Equipment/Weapons/";
    private const string CommonWeaponsResourcePath = WeaponsResourcePath + "0_Common";
    private const string UncommonWeaponsResourcePath = WeaponsResourcePath + "1_Uncommon";
    private const string RareWeaponsResourcePath = WeaponsResourcePath + "2_Rare";
    private const string LegendaryWeaponsResourcePath = WeaponsResourcePath + "3_Legendary";

    [SerializeField]
    private GameObject grabbableWeaponPrefab;

    [SerializeField]
    private GameObject infoCanvas;

    private string GetResourcePath(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return CommonWeaponsResourcePath;
            case Rarity.Uncommon:
                return UncommonWeaponsResourcePath;
            case Rarity.Rare:
                return RareWeaponsResourcePath;
            case Rarity.Legendary:
                return LegendaryWeaponsResourcePath;
            default:
                Debug.LogError("LootChest.GenerateResourcePathName: Unknown Rarity: " + rarity + ". Unable to generate loot");
                return string.Empty;
        }
    }

    protected virtual Rarity GetEquipmentRarity()
    {
        float rarityRoll = Random.Range(0.0f, 1.0f);

        if (rarityRoll <= 0.20f)
        {
            return Rarity.Legendary;
        }
        else if (rarityRoll <= 0.40f)
        {
            return Rarity.Rare;
        }
        else if (rarityRoll <= 0.70f)
        {
            return Rarity.Uncommon;
        }

        return Rarity.Common;
    }

    public virtual void GenerateLoot()
    {
        //Roll for rarity
        Rarity rarity = this.GetEquipmentRarity();

        string resourcePath = this.GetResourcePath(rarity);
        if (resourcePath == string.Empty)
        {
            return;
        }

        Weapon[] weapons = Resources.LoadAll<Weapon>(resourcePath);
        Weapon chosenWeapon = weapons[Random.Range(0, weapons.Length)];
        this.GenerateGrabbableWeapon(chosenWeapon);

        Destroy(this.gameObject);
    }

    private void GenerateGrabbableWeapon(Weapon newWeapon)
    {
        GameObject instance = GameObject.Instantiate(grabbableWeaponPrefab, this.gameObject.transform.position, new Quaternion()) as GameObject;
        GrabbableWeapon grabbableWeaponComponent = instance.GetComponent<GrabbableWeapon>();
        grabbableWeaponComponent.weaponDetails = newWeapon;
        grabbableWeaponComponent.PlayChestOpenSound();
    }

    private void OnTriggerEnter(Collider other)
    {
        this.infoCanvas.SetActive(true);
        other.gameObject.GetComponent<Creature>().lootChestCandidate = this;
    }

    private void OnTriggerExit(Collider other)
    {
        this.infoCanvas.SetActive(false);
        other.gameObject.GetComponent<Creature>().lootChestCandidate = null;
    }
}