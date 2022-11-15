using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private GameObject deathScreen;

    [Header("Creature Attributes")]
    [SerializeField]
    private Weapon startingWeapon;
    [SerializeField]
    private Armor startingArmor;
    
    public Weapon equippedWeapon;    
    public Armor equippedArmor;

    public int totalHitPoints = 100;
    public int currentHitPoints = 100;

    public delegate void EquipTrigger();
    public EquipTrigger onEquipTriggered;

    public delegate void AttackTrigger();
    public AttackTrigger onAttackTriggered;

    public delegate void DamageEnemyTrigger(Creature targetCreature, int damageDealt);
    public DamageEnemyTrigger onDamageEnemyTriggered;

    public delegate void DamageSelfTrigger(Creature sourceCreature, int damageDealt);
    public DamageSelfTrigger onDamageSelfTriggered;

    public float moveSpeed = 5f;

    [HideInInspector]
    public Weapon unarmedWeapon;
    [HideInInspector]
    public Armor nakedArmor;

    private Animator creatureAnimator;

    [HideInInspector]
    public GrabbableWeapon weaponPickupCandidate;

    [HideInInspector]
    public LootChest lootChestCandidate;

    [HideInInspector]
    public Rigidbody creatureRb;

    [HideInInspector]
    public Coroutine currentAttack = null;

    public float attackDuration = 0.5f;

    protected AudioSource creatureAudio;

    protected AudioSource damageAudio;

    protected virtual void Start()
    {
        this.unarmedWeapon = Resources.Load<Weapon>("Equipment/Weapons/Unarmed");
        this.nakedArmor = Resources.Load<Armor>("Equipment/Armor/Naked");

        this.creatureAnimator = GetComponent<Animator>();

        this.currentHitPoints = this.totalHitPoints;

        this.creatureRb = GetComponent<Rigidbody>();

        AudioSource[] allAudio = GetComponents<AudioSource>();

        this.creatureAudio = allAudio[0];
        this.damageAudio = allAudio[1];

        if (this.startingWeapon != null)
        {
            this.Equip(this.startingWeapon);
        }
        else
        {
            this.Equip(this.unarmedWeapon);
        }
        if (this.startingArmor != null)
        {
            this.Equip(this.startingArmor);
        }
        else
        {
            this.Equip(this.nakedArmor);
        }
    }

    protected virtual void Update()
    {
        if (this.currentHitPoints <= 0)
        {
            this.Die();
        }
    }

    public void Interact()
    {        
        if (this.lootChestCandidate != null)
        {
            this.OpenLootChest();
        }
        else if (this.weaponPickupCandidate != null)
        {
            this.PickUpWeapon();
        }
    }

    public void PickUpWeapon()
    {
        this.Equip(this.weaponPickupCandidate.weaponDetails);
        Destroy(this.weaponPickupCandidate.gameObject);        
    }

    public void OpenLootChest()
    {
        this.lootChestCandidate.GenerateLoot();        
    }

    public void DropWeapon()
    {
        if (this.equippedWeapon == null || this.equippedWeapon == this.unarmedWeapon)
        {
            return;
        }

        GameObject grabbableWeaponPrefab = Resources.Load<GameObject>("Prefabs/GrabbableWeapon");
        GameObject grabbableWeaponObject = Instantiate(grabbableWeaponPrefab, this.gameObject.transform.position, new Quaternion());
        grabbableWeaponObject.GetComponent<GrabbableWeapon>().Setup(this.equippedWeapon);
        this.equippedWeapon.Unequip();

        this.equippedWeapon = this.unarmedWeapon;
        this.equippedWeapon.Equip(this);
    }

    public void LoseWeapon()
    {
        if (this.equippedWeapon == null || this.equippedWeapon == this.unarmedWeapon)
        {
            return;
        }
        
        this.equippedWeapon.Unequip();
        this.equippedWeapon = null;
        this.Equip(this.unarmedWeapon);
    }

    public void DropArmor()
    {
        if (this.equippedArmor == null || this.equippedArmor == this.nakedArmor)
        {
            return;
        }

        GameObject grabbableArmorPrefab = Resources.Load<GameObject>("Prefabs/GrabbableArmor");
        GameObject grabbableArmorObject = Instantiate(grabbableArmorPrefab, this.gameObject.transform.position, new Quaternion());
        grabbableArmorObject.GetComponent<GrabbableArmor>().Setup(this.equippedArmor);
        this.equippedArmor.Unequip();

        this.equippedArmor = this.nakedArmor;
        this.equippedArmor.Equip(this);
    }

    public void Equip(Weapon newWeapon)
    {
        this.DropWeapon();
        this.equippedWeapon = newWeapon;
        this.equippedWeapon.Equip(this);

        if (this.equippedWeapon != this.unarmedWeapon)
        {
            this.creatureAudio.clip = Resources.Load<AudioClip>("Audio/Equip");
            this.creatureAudio.Play();
        }

        this.TriggerEquip();
    }

    private void Equip(Armor newArmor)
    {
        this.DropArmor();
        this.equippedArmor = newArmor;
        this.equippedArmor.Equip(this);
        this.TriggerEquip();
    }

    public void TriggerEquip()
    {
        if (this.onEquipTriggered != null)
        {
            this.onEquipTriggered();
        }
    }

    public void TriggerAttack()
    {
        if (this.onAttackTriggered != null)
        {
            this.onAttackTriggered();
        }
    }

    public void TriggerDamageEnemy(Creature targetCreature, int damageDealt)
    {
        if (this.onDamageEnemyTriggered != null)
        {
            this.onDamageEnemyTriggered(targetCreature, damageDealt);
        }
    }

    public void TriggerDamageSelf(Creature sourceCreature, int damageDealt)
    {
        if (this.onDamageSelfTriggered != null)
        {
            this.onDamageSelfTriggered(sourceCreature, damageDealt);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        this.damageAudio.clip = Resources.Load<AudioClip>("Audio/damaged");
        this.damageAudio.Play();

        this.creatureAnimator.SetTrigger("DamagedTrigger");

        this.currentHitPoints -= damage;
    }

    protected virtual void Die()
    {
        this.creatureAudio.clip = Resources.Load<AudioClip>("Audio/die");
        this.creatureAudio.Play();

        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<Collider>().enabled = false;
        this.enabled = false;

        this.deathScreen.SetActive(true);
    }    
}
