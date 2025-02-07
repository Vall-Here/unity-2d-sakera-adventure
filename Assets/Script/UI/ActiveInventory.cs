using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class ActiveInventory : Singleton<ActiveInventory>
{
    private int activeSlotIndexNum = 0;
    private PlayerControls playerControls;

    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private List<InventorySlot> inventorySlots;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
    }

    private void Start()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.Initialize(equipmentManager);
        }

        equipmentManager.OnEquipmentChanged += UpdateInventorySlots;

        playerControls.Inventory.Keyboard.performed += ctx =>
        {
            int slotIndex = (int)ctx.ReadValue<float>();
            ToggleActiveSlot(slotIndex); 
        };

        ToggleActiveSlot(0); 
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        equipmentManager.OnEquipmentChanged -= UpdateInventorySlots;
    }

    private void UpdateInventorySlots()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.SyncWithEquipment();
        }
    }



    private void ToggleActiveSlot(int slotIndex)
    {
        ToggleActiveHighlight(slotIndex - 1);
        ChangeActiveWeapon();
    }


    private void ToggleActiveHighlight(int indexNum)
    {

        if (indexNum < 5)
        {
            activeSlotIndexNum = indexNum;
        }
        else
        {
            Debug.Log("Index out of range");
            return;
        }
        
        foreach (Transform inventorySlot in transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }
        if (activeSlotIndexNum < 0)
        {
            activeSlotIndexNum = 0;
        }
        transform.GetChild(activeSlotIndexNum).GetChild(0).gameObject.SetActive(true);
        
    }

    private void ChangeActiveWeapon()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        InventorySlot activeSlot = inventorySlots[activeSlotIndexNum];
        ItemSO activeItem = activeSlot.GetItemSO();

        if (activeItem is EquippableItemSO equippableItem)
        {
            GameObject weaponToSpawn = equippableItem.ItemPrefab;

            if (weaponToSpawn != null)
            {
                GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);
                SetWeaponPosition(newWeapon);

                var weaponComponent = newWeapon.GetComponent<MonoBehaviour>();
                ActiveWeapon.Instance.NewWeapon(weaponComponent, activeSlotIndexNum);
                Debug.Log("Equipped weapon: " + equippableItem.name);
            }
        }
        else if (activeItem is EdibleItemSO edibleItem)
        {
            GameObject itemToSpawn = edibleItem.ItemPrefab;

            if (itemToSpawn != null)
            {
                GameObject newItem = Instantiate(itemToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);
                SetWeaponPosition(newItem);

                var usableComponent = newItem.GetComponent<IUsable>();  
                if (usableComponent != null)
                {
                    ActiveWeapon.Instance.NewWeapon((MonoBehaviour)usableComponent, activeSlotIndexNum);
                    Debug.Log("Equipped edible item: " + edibleItem.name);
                }
                else
                {
                    Debug.LogError("The spawned item does not implement IUsable.");
                }
            }
        }
        else
        {
            ActiveWeapon.Instance.WeaponNull();
        }
    }

    private void SetWeaponPosition(GameObject item)
    {
        ActiveWeapon.Instance.transform.rotation = Quaternion.identity;
        ActiveWeapon.Instance.transform.position = PlayerController.Instance.transform.position;
        item.transform.parent = ActiveWeapon.Instance.transform;
    }

}
