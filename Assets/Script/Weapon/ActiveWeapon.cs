using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

// public class ActiveWeapon : Singleton<ActiveWeapon>
// {
//     public MonoBehaviour CurrentActiveWeapon { get; private set; }

//     private PlayerControls playerControls;
//     public bool hasWeapon => CurrentActiveWeapon != null;
//     private bool attackButtonDown, isAttacking = false;
//     private float timeBetweenAttacks;

//     // Tambahkan variabel untuk menyimpan sprite cursor
//     public Texture2D rangeCursor;
//     public Texture2D meleeCursor;
//     public Texture2D defaultCursor;

//     protected override void Awake()
//     {
//         base.Awake();
//         playerControls = new PlayerControls();
//     }

//     private void OnEnable(){
//         playerControls.Enable();
//     }

//     private void Start(){
//         playerControls.Combat.Attack.started += _ => StartAttack();
//         playerControls.Combat.Attack.canceled += _ => StopAttack();
//         AttackCooldown();

//         UpdateCursor();  // Update cursor saat game mulai
//     }

//     private void Update(){
//         Attack();
//     }

//     public void NewWeapon(MonoBehaviour weapon){
//         CurrentActiveWeapon = weapon;

//         AttackCooldown();
//         timeBetweenAttacks = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;

//         UpdateCursor();  // Update cursor saat mengganti senjata
//     }

//     public void WeaponNull(){
//         CurrentActiveWeapon = null;
//         UpdateCursor(); 
//     }

//     public string GetWeaponType()
//     {
//         if (CurrentActiveWeapon is IWeapon weapon)
//         {
//             return weapon.WeaponType;
//         }

//         return "No Weapon";
//     }

//     public void EquipWeaponFromSlot(InventorySlot slot)
//     {
//         ItemSO itemSO = slot.GetWeaponInfo();

//         if (itemSO != null)
//         {
//             NewWeapon(slot.GetComponent<MonoBehaviour>());
//             Debug.Log("Weapon equipped from slot: " + itemSO.Name);
//         }
//         else
//         {
//             WeaponNull();
//             Debug.Log("No weapon found in slot.");
//         }
//     }

//     private void AttackCooldown(){
//         isAttacking = true;
//         StopAllCoroutines();
//         StartCoroutine(TimeBetweenAttacksRoutine());
//     }

//     private IEnumerator TimeBetweenAttacksRoutine(){
//         yield return new WaitForSeconds(timeBetweenAttacks);
//         isAttacking = false;
//     }

//     private void StartAttack(){
//         attackButtonDown = true;
//     }

//     private void StopAttack(){
//         attackButtonDown = false;
//     }

//     private void Attack(){
//         if(attackButtonDown && !isAttacking){
//             AttackCooldown();
//             (CurrentActiveWeapon as IWeapon).Attack();
//         }
//     }

//     private void UpdateCursor(){
//         if (CurrentActiveWeapon is IWeapon weapon)
//         {
//             if (weapon.WeaponType == "Range")
//             {
//                 Cursor.SetCursor(rangeCursor, Vector2.zero, CursorMode.Auto);
//             }
//             else if (weapon.WeaponType == "Melee")
//             {
//                 Cursor.SetCursor(meleeCursor, Vector2.zero, CursorMode.Auto);
//             }
//             else
//             {
//                 Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
//             }
//         }
//         else
//         {
//             Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
//         }
//     }
// }



public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon { get; private set; }

    private PlayerControls playerControls;
    public bool hasWeapon => CurrentActiveWeapon != null;
    private bool attackButtonDown, isAttacking = false;
    private float timeBetweenAttacks;
    private int activeSlotIndex = -1;

    public Texture2D rangeCursor;
    public Texture2D meleeCursor;
    public AudioClip attackMelee;
    public AudioClip attackRange;
    public Texture2D defaultCursor;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
        audioSource = GetComponent<AudioSource>();

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.OnEquipmentChanged += HandleEquipmentChanged;
        }
        else
        {
            Debug.LogError("ActiveWeapon: EquipmentManager.Instance is null!");
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    public void DisableControls()
    {
        playerControls.Disable();
    }

    public void EnableControls()
    {
        playerControls.Enable();
    }


    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttack();
        playerControls.Combat.Attack.canceled += _ => StopAttack();

        playerControls.Item.Use.performed += _ => UseItem(); 
        AttackCooldown();

        UpdateCursor();
    }

    private void Update()
    {
        Attack();
    }

    public string GetWeaponType()
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            return weapon.WeaponType;
        }

        return "No Weapon";
    }
    public void NewWeapon(MonoBehaviour weapon ,int slotIndex)
    {
        CurrentActiveWeapon = weapon;
        activeSlotIndex = slotIndex;
        AttackCooldown();
        timeBetweenAttacks = 1f;

        UpdateCursor();
        Debug.Log(CurrentActiveWeapon);
        
    }

    public void WeaponNull(){
        if (CurrentActiveWeapon != null)
        {
            Destroy(CurrentActiveWeapon.gameObject); 
            CurrentActiveWeapon = null; 
            
        }
        activeSlotIndex = -1; 
        UpdateCursor(); 
    }

    private void AttackCooldown()
    {
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    private void StartAttack()
    {
        attackButtonDown = true;
    }

    private void StopAttack()
    {
        attackButtonDown = false;
    }

    private void Attack()
    {
        if (attackButtonDown && !isAttacking && CurrentActiveWeapon is IWeapon weapon)
        {
            AttackCooldown();
            weapon.Attack();

            if (weapon.WeaponType == "Melee")
            {
                audioSource.PlayOneShot(attackMelee);
            }
            else if (weapon.WeaponType == "Range")
            {
                audioSource.PlayOneShot(attackRange);
            }
        }
    }

    private void UseItem()
    {
        if (CurrentActiveWeapon is IUsable usableItem)
        {
            Debug.Log("Using item...");
            usableItem.Use();

            if (activeSlotIndex >= 0 && EquipmentManager.Instance != null)
            {
                bool success = EquipmentManager.Instance.DecreaseItemQuantity(activeSlotIndex, 1);
                if (success)
                {
                    Debug.Log($"Used 1 unit from slot {activeSlotIndex}.");
                }
                else
                {
                    Debug.LogError($"Failed to use item from slot {activeSlotIndex}.");
                }
            }
        }
        else
        {
            Debug.LogError($"CurrentActiveWeapon is not usable! Type: {CurrentActiveWeapon?.GetType().Name}");
        }
    }

    private void HandleEquipmentChanged()
    {
        if (activeSlotIndex >= 0)
        {
            ItemSO item = EquipmentManager.Instance.GetEquippedItemByIndex(activeSlotIndex);
            if (item == null)
            {
                // Jika item di slot telah dihapus, set ActiveWeapon ke null
                WeaponNull();
                Debug.Log($"Item di slot {activeSlotIndex} telah dihapus. ActiveWeapon direset.");
            }
        }
    }


    private void UpdateCursor()
    {
        if (CurrentActiveWeapon is IWeapon weapon)
        {
            if (weapon.WeaponType == "Range")
            {
                Cursor.SetCursor(rangeCursor, Vector2.zero, CursorMode.Auto);
            }
            else if (weapon.WeaponType == "Melee")
            {
                Cursor.SetCursor(meleeCursor, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    
}
