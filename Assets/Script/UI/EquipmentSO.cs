using Inventory.Model;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipSlot", menuName = "Inventory/EquipSlot")]
public class EquipItemSO : ScriptableObject
{
    [field: SerializeField] public ItemSO EquippedItem { get; set; } 

    [field: SerializeField] public ItemTypeEnum.SlotItemType SlotType { get; set; } 
    
    public bool IsEmpty => EquippedItem == null;
    [field: SerializeField] public int Quantity { get; set; } = 1;  


    public void Equip(ItemSO item, int quantity = 1){
        if (item.ItemType != SlotType)
        {
            Debug.LogWarning($"Item {item.Name} tidak cocok dengan slot {SlotType}");
            return;
        }

        if (IsEmpty)
        {
            EquippedItem = item;
            Quantity = quantity;
            Debug.Log($"Equipped {quantity} of {item.Name} to slot {SlotType}");
        }
        else if (EquippedItem.ID == item.ID && item.IsStackable)
        {
            Quantity += quantity;
            if (Quantity > item.MaxStackSize)
            {
                Debug.LogWarning($"Quantity exceeds MaxStackSize for {item.Name}. Set to MaxStackSize.");
                Quantity = item.MaxStackSize;
            }
            else
            {
                Debug.Log($"Stacked {quantity} of {item.Name} to slot {SlotType}. New quantity: {Quantity}");
            }
        }
        else
        {
            Debug.LogWarning($"Slot {SlotType} sudah terisi dengan item {EquippedItem.Name}");
        }
    }
    

    public void Unequip(int quantity = 1)
    {
        if (IsEmpty)
        {
            Debug.LogWarning($"Slot {SlotType} sudah kosong.");
            return;
        }

        Quantity -= quantity;
        Debug.Log($"Unequip {quantity} of {EquippedItem.Name} from slot {SlotType}. New quantity: {Quantity}");

        if (Quantity <= 0)
        {
            EquippedItem = null;
            Quantity = 0;
            Debug.Log($"Slot {SlotType} sekarang kosong.");
        }
    }
}
