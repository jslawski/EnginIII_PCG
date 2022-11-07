using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableArmor : MonoBehaviour
{
    [HideInInspector]
    public Armor armorDetails;
    private MeshRenderer objectMesh;

    // Start is called before the first frame update
    public void Setup(Armor armor)
    {
        this.objectMesh = GetComponent<MeshRenderer>();
        this.armorDetails = armor;

        switch (armor.rarity)
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
                Debug.LogError("Unknown Rarity: " + armor.rarity + " Unable to set object mesh");
                break;
        }
    }
}
