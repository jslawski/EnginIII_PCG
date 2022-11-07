using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrabbableWeapon : MonoBehaviour
{
    public bool grabbable = true;

    public Weapon weaponDetails;
    private MeshRenderer objectMesh;

    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private TextMeshProUGUI weaponName;
    [SerializeField]
    private TextMeshProUGUI weaponAtk;
    [SerializeField]
    private TextMeshProUGUI weaponSkills;
    [SerializeField]
    private AudioSource chestOpenSound;

    public void Start()
    {
        if (this.weaponDetails != null)
        {
            this.Setup(this.weaponDetails);
        }
    }

    public void PlayChestOpenSound()
    {
        this.chestOpenSound.pitch = Random.Range(0.8f, 1.2f);
        this.chestOpenSound.Play();
    }

    // Start is called before the first frame update
    public void Setup(Weapon weapon)
    {
        this.objectMesh = GetComponent<MeshRenderer>();
        this.weaponDetails = weapon;

        this.SetupWeaponInfo();
        
        switch (weapon.rarity)
        {
            case Rarity.Common:
                this.objectMesh.material = Resources.Load<Material>("Materials/Common");
                break;
            case Rarity.Uncommon:
                this.objectMesh.material = Resources.Load<Material>("Materials/Uncommon");
                break;
            case Rarity.Rare:
                this.objectMesh.material = Resources.Load<Material>("Materials/Rare");
                break;
            case Rarity.Legendary:
                this.objectMesh.material = Resources.Load<Material>("Materials/Legendary");
                break;
            default:
                Debug.LogError("Unknown Rarity: " + weapon.rarity + " Unable to set object mesh");
                break;
        }
    }

    private void SetupWeaponInfo()
    {
        this.weaponName.text = this.weaponDetails.name;

        this.weaponAtk.text = "Atk: " + this.weaponDetails.attackPower;

        if (this.weaponDetails.enchantmentSlots.Length > 0)
        {
            string skillList = this.weaponDetails.enchantmentSlots[0].abilityDisplayName;

            for (int i = 1; i < this.weaponDetails.enchantmentSlots.Length; i++)
            {
                skillList += "\n";
                skillList += this.weaponDetails.enchantmentSlots[i].abilityDisplayName;
            }

            this.weaponSkills.text = skillList;
        }
        else
        {
            this.weaponSkills.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.grabbable == false)
        {
            return;
        }

        this.infoCanvas.SetActive(true);

        other.gameObject.GetComponent<Creature>().weaponPickupCandidate = this;
    }

    private void OnTriggerExit(Collider other)
    {
        this.infoCanvas.SetActive(false);

        other.gameObject.GetComponent<Creature>().weaponPickupCandidate = null;
    }
}
