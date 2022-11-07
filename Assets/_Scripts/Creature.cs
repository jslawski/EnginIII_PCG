using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Creature : MonoBehaviour
{
    [Header("Creature Attributes")]
    [SerializeField]
    private Weapon startingWeapon;
    [SerializeField]
    private Armor startingArmor;
    
    public Weapon equippedWeapon;    
    public Armor equippedArmor;

    public int totalHitPoints = 100;
    public int currentHitPoints = 100;

    public AttackZone attackZone;

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

    private TextMeshProUGUI hpText;

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

        this.attackZone = GetComponentInChildren<AttackZone>(true);
        this.attackZone.Setup();

        this.creatureAnimator = GetComponent<Animator>();

        this.currentHitPoints = this.totalHitPoints;

        this.hpText = GetComponentInChildren<TextMeshProUGUI>(true);

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
        this.hpText.text = ("HP: " + this.currentHitPoints);

        Vector3 adjustedPosition =
            new Vector3(this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + 1.5f,
            -0.5f);

        this.hpText.transform.parent.transform.parent.position = adjustedPosition;
        this.hpText.transform.parent.transform.parent.rotation = Quaternion.identity;

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

        this.SetupAttackZone();

        if (this.equippedWeapon != this.unarmedWeapon)
        {
            this.creatureAudio.clip = Resources.Load<AudioClip>("Audio/Equip");
            this.creatureAudio.Play();
        }

        this.TriggerEquip();
    }

    private void SetupAttackZone()
    {
        this.attackZone.gameObject.transform.localScale =
            new Vector3(this.equippedWeapon.attackZoneDimensions.y,
            this.equippedWeapon.attackZoneDimensions.x, 1.0f);

        float adjustedXPosition = (this.attackZone.gameObject.transform.localScale.x / 2.0f) + 0.5f;

        Vector3 adjustedPosition = new Vector3(adjustedXPosition,
            this.attackZone.gameObject.transform.localPosition.y,
            this.attackZone.gameObject.transform.localPosition.z);
        
        this.attackZone.gameObject.transform.localPosition = adjustedPosition;           
    }

    private void Equip(Armor newArmor)
    {
        this.DropArmor();
        this.equippedArmor = newArmor;
        this.equippedArmor.Equip(this);
        this.TriggerEquip();
    }

    public virtual void Attack()
    {
        if (this.currentAttack != null)
        {
            return;
        }

        this.creatureAudio.clip = Resources.Load<AudioClip>("Audio/Attack");
        this.creatureAudio.Play();

        this.currentAttack = StartCoroutine(this.AttackCoroutine());
        this.TriggerAttack();
    }

    private IEnumerator AttackCoroutine()
    {
        float elapsedAttackTime = 0.0f;

        this.attackZone.EnableAttack();

        while (elapsedAttackTime < this.attackDuration)
        {
            elapsedAttackTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        this.attackZone.DisableAttack();
        this.currentAttack = null;
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

    public virtual void TakeDamage(Creature attackingCreature, int damage, bool isDOTS)
    {
        if (isDOTS == true)
        {
            this.damageAudio.clip = Resources.Load<AudioClip>("Audio/DOT");
            this.damageAudio.Play();
        }
        else
        {
            this.damageAudio.clip = Resources.Load<AudioClip>("Audio/Damage");
            this.damageAudio.Play();
        }

        this.currentHitPoints -= damage;
        this.creatureAnimator.SetTrigger("DamagedTrigger");

        this.TriggerDamageSelf(attackingCreature, damage);        
    }

    protected virtual void Die()
    {
        this.creatureAudio.clip = Resources.Load<AudioClip>("Audio/Die");
        this.creatureAudio.Play();

        Destroy(this.gameObject);
    }    
}
