using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(fileName = "ChestInventory", menuName = "Inventory/ChestInventorySO")]
    public class ChestInventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> chestItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnChestUpdated;

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            chestItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                chestItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            Debug.Log($"ChestInventorySO: AddItem called for {item.Name} with quantity {quantity}");
            if (!item.IsStackable)
            {
                while (quantity > 0 && !IsChestFull())
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                }
                InformAboutChange();
                return quantity;
            }

            quantity = AddStackableItem(item, quantity);
            InformAboutChange();
            return quantity;
        }

        private int AddItemToFirstFreeSlot(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            InventoryItem newItem = new InventoryItem
            {
                item = item,
                quantity = quantity,
                itemState = new List<ItemParameter>(itemState ?? item.DefaultParametersList)
            };

            for (int i = 0; i < chestItems.Count; i++)
            {
                if (chestItems[i].IsEmpty)
                {
                    chestItems[i] = newItem;
                    Debug.Log($"ChestInventorySO: Added {item.Name} to slot {i} with quantity {quantity}");
                    return quantity;
                }
            }
            Debug.LogWarning("ChestInventorySO: Failed to add item - Chest is full.");
            return 0;
        }

        private bool IsChestFull() => chestItems.All(item => !item.IsEmpty);

        private int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < chestItems.Count; i++)
            {
                if (chestItems[i].IsEmpty)
                    continue;

                if (chestItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTake = chestItems[i].item.MaxStackSize - chestItems[i].quantity;

                    if (quantity > amountPossibleToTake)
                    {
                        chestItems[i] = chestItems[i].ChangeQuantity(chestItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                        Debug.Log($"ChestInventorySO: Stacked {amountPossibleToTake} of {item.Name} to slot {i}. Remaining quantity: {quantity}");
                    }
                    else
                    {
                        chestItems[i] = chestItems[i].ChangeQuantity(chestItems[i].quantity + quantity);
                        Debug.Log($"ChestInventorySO: Stacked {quantity} of {item.Name} to slot {i}. No remaining quantity.");
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while (quantity > 0 && !IsChestFull())
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
                Debug.Log($"ChestInventorySO: Added {newQuantity} of {item.Name} to a new slot. Remaining quantity: {quantity}");
            }
            return quantity;
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (itemIndex >= 0 && itemIndex < chestItems.Count)
            {
                if (chestItems[itemIndex].IsEmpty)
                    return;

                int remainder = chestItems[itemIndex].quantity - amount;
                if (remainder <= 0)
                    chestItems[itemIndex] = InventoryItem.GetEmptyItem();
                else
                    chestItems[itemIndex] = chestItems[itemIndex].ChangeQuantity(remainder);

                InformAboutChange();
            }
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity, item.itemState);
        }

        public Dictionary<int, InventoryItem> GetCurrentChestState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

            for (int i = 0; i < chestItems.Count; i++)
            {
                if (!chestItems[i].IsEmpty)
                {
                    returnValue[i] = chestItems[i];
                }
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return chestItems[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItem tempItem = chestItems[itemIndex_1];
            chestItems[itemIndex_1] = chestItems[itemIndex_2];
            chestItems[itemIndex_2] = tempItem;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnChestUpdated?.Invoke(GetCurrentChestState());
        }

        // Implementasi Save dan Load jika diperlukan
    }
}
