using System;
using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [SerializeField]
    public EquipSlot[] equipSlotUI; 

    [SerializeField]
    private List<EquipItemSO> equipSlots = new List<EquipItemSO>(); 

    public int TotalSlots => equipSlotUI.Length; 

    public event Action OnEquipmentChanged;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool EquipItem(ItemSO item, int quantity = 1)
    {
        foreach (EquipSlot slotUI in equipSlotUI)
        {
            if (slotUI.itemTypeEnum == item.ItemType)
            {
                if (slotUI.GetItemSO() == null)
                {
                    slotUI.EquipItem(item, quantity);  
                    Debug.Log($"{item.Name} telah di-equip ke slot {slotUI.itemTypeEnum} dengan quantity {quantity}");
                    OnEquipmentChanged?.Invoke();
                    return true;
                }
                else if (slotUI.AllowsStacking && slotUI.GetItemSO().ID == item.ID && item.IsStackable)
                {
                    slotUI.EquipItem(item, quantity);
                    Debug.Log($"Stacked {quantity} of {item.Name} to slot {slotUI.itemTypeEnum}. New quantity: {slotUI.EquipItemSO.Quantity}");
                    OnEquipmentChanged?.Invoke();
                    return true;
                }
            }
        }

        Debug.LogWarning($"Tidak ada slot yang sesuai atau slot sudah terisi untuk item {item.Name}");
        return false;
    }


    public void UnequipItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < equipSlotUI.Length)
        {
            EquipSlot slotUI = equipSlotUI[slotIndex];
            if (slotUI.GetItemSO() != null)
            {
                ItemSO equippedItem = slotUI.GetItemSO();  
                slotUI.UnequipItem();  
                Debug.Log($"{equippedItem.Name} telah di-unequip dari slot {slotIndex}");
                OnEquipmentChanged?.Invoke();
            }
            else
            {
                Debug.LogWarning($"Tidak ada item yang di-equip di slot {slotIndex}");
                slotUI.UpdateUI();
            }
        }
        else
        {
            Debug.LogWarning($"Slot index {slotIndex} di luar jangkauan.");
        }
    }


    public ItemSO GetEquippedItemByIndex(int index)
    {
        if (index < 0 || index >= equipSlotUI.Length)
        {
            Debug.LogWarning($"Index {index} di luar jangkauan slot yang ada.");
            return null;
        }

        EquipSlot slotUI = equipSlotUI[index];
        return slotUI.GetItemSO();
    }

    public ItemSO GetEquippedItem(ItemTypeEnum.SlotItemType slotType)
    {
        foreach (EquipSlot slotUI in equipSlotUI)
        {
            if (slotUI.itemTypeEnum == slotType && slotUI.GetItemSO() != null)
            {
                return slotUI.GetItemSO(); 
            }
        }

        Debug.LogWarning($"Tidak ada item yang di-equip di slot {slotType}");
        return null;
    }

    public int GetEquippedItemQuantity(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equipSlotUI.Length)
        {
            Debug.LogWarning($"Index {slotIndex} di luar jangkauan slot yang ada.");
            return 0;
        }

        EquipSlot slotUI = equipSlotUI[slotIndex];
        if (slotUI.EquipItemSO != null && !slotUI.EquipItemSO.IsEmpty)
        {
            return slotUI.EquipItemSO.Quantity;
        }

        return 0;
    }

    public bool DecreaseItemQuantity(int slotIndex, int amount)
    {
        if (slotIndex < 0 || slotIndex >= equipSlotUI.Length)
        {
            Debug.LogWarning($"DecreaseItemQuantity: SlotIndex {slotIndex} out of range.");
            return false;
        }

        EquipSlot slot = equipSlotUI[slotIndex];
        if (slot.EquipItemSO != null && !slot.EquipItemSO.IsEmpty)
        {
            slot.EquipItemSO.Unequip(amount);
            slot.UpdateUI();
            Debug.Log($"DecreaseItemQuantity: Decreased item in slot {slotIndex} by {amount}. New quantity: {slot.EquipItemSO.Quantity}");
            OnEquipmentChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.LogWarning($"DecreaseItemQuantity: No item to decrease in slot {slotIndex}.");
            return false;
        }
    }


     /// <summary>
    /// Mengambil EquipSlot berdasarkan SlotType.
    /// </summary>
    /// <param name="slotType">Jenis slot yang ingin diambil.</param>
    /// <returns>EquipSlot yang sesuai atau null jika tidak ditemukan.</returns>
    public EquipSlot GetEquipSlotByType(ItemTypeEnum.SlotItemType slotType)
    {
        foreach (EquipSlot slot in equipSlotUI)
        {
            if (slot.itemTypeEnum == slotType)
            {
                return slot;
            }
        }

        Debug.LogWarning($"EquipmentManager: EquipSlot untuk {slotType} tidak ditemukan!");
        return null;
    }

}
