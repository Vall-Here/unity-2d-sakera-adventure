using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class InventorySO : ScriptableObject
    {
        public static InventorySO Instance { get; private set; }
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                Debug.LogWarning("Multiple instances of InventorySO detected! Ensure only one instance exists.");
            }
        }

        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
        {
            Debug.Log($"AddItem called for {item.Name} with quantity {quantity}");
            if (item.IsStackable == false)
            {
                while (quantity > 0 && IsInventoryFull() == false)
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

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = newItem;
                    Debug.Log($"Added {item.Name} to slot {i} with quantity {quantity}");
                    return quantity;
                }
            }
            Debug.LogWarning("Failed to add item: Inventory is full.");
            return 0;
        }

        private bool IsInventoryFull() => inventoryItems.All(item => !item.IsEmpty);

        private int AddStackableItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;

                if (inventoryItems[i].item.ID == item.ID)
                {
                    int amountPossibleToTake = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                    if (quantity > amountPossibleToTake)
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                        quantity -= amountPossibleToTake;
                        Debug.Log($"Stacked {amountPossibleToTake} of {item.Name} to slot {i}. Remaining quantity: {quantity}");
                    }
                    else
                    {
                        inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].quantity + quantity);
                        Debug.Log($"Stacked {quantity} of {item.Name} to slot {i}. No remaining quantity.");
                        InformAboutChange();
                        return 0;
                    }
                }
            }
            while (quantity > 0 && IsInventoryFull() == false)
            {
                int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
                quantity -= newQuantity;
                AddItemToFirstFreeSlot(item, newQuantity);
                Debug.Log($"Added {newQuantity} of {item.Name} to a new slot. Remaining quantity: {quantity}");
            }
            return quantity;
        }

        public void RemoveItem(int itemIndex, int amount)
        {
            if (itemIndex >= 0 && itemIndex < inventoryItems.Count)
            {
                if (inventoryItems[itemIndex].IsEmpty)
                    return;

                int remainder = inventoryItems[itemIndex].quantity - amount;
                if (remainder <= 0)
                    inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
                else
                    inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(remainder);

                InformAboutChange();
            }
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity, item.itemState);
        }

        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (!inventoryItems[i].IsEmpty)
                {
                    returnValue[i] = inventoryItems[i];
                }
            }
            return returnValue;
        }

        public InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            InventoryItem tempItem = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = tempItem;
            InformAboutChange();
        }

        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }

        // Hapus metode SaveInventory() dan LoadInventory()

        public InventoryData GetInventoryData()
        {
            InventoryData data = new InventoryData();

            foreach (var item in inventoryItems)
            {
                if (!item.IsEmpty)
                {
                    var itemData = new InventoryItemData
                    {
                        itemName = item.item.Name,
                        quantity = item.quantity,
                        itemParameters = item.itemState.Select(param => new ItemParameterData
                        {
                            parameterName = param.itemParameter.ParameterName,
                            value = param.value
                        }).ToList()
                    };
                    data.items.Add(itemData);
                }
            }
            return data;
        }
        private void ClearInventory()
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                inventoryItems[i] = InventoryItem.GetEmptyItem();
            }
        }
        public void SetInventoryData(InventoryData data)
        {
            Debug.Log("SetInventoryData called.");
            ClearInventory(); 
            Initialize(); 

            foreach (var itemData in data.items)
            {
                ItemSO loadedItem = Resources.Load<ItemSO>("Items/" + itemData.itemName);
                if (loadedItem != null)
                {
                    Debug.Log($"Loading item: {itemData.itemName}, Quantity: {itemData.quantity}");

                    List<ItemParameter> itemState = new List<ItemParameter>();
                    if (itemData.itemParameters != null && itemData.itemParameters.Count > 0)
                    {
                        foreach (var paramData in itemData.itemParameters)
                        {
                            ItemParameterSO parameterSO = Resources.Load<ItemParameterSO>("Parameters/" + paramData.parameterName);
                            if (parameterSO != null)
                            {
                                ItemParameter itemParam = new ItemParameter
                                {
                                    itemParameter = parameterSO,
                                    value = paramData.value
                                };
                                itemState.Add(itemParam);
                            }
                            else
                            {
                                Debug.LogWarning($"ItemParameterSO tidak ditemukan: {paramData.parameterName}");
                            }
                        }
                    }

                    AddItem(loadedItem, itemData.quantity, itemState);
                }
                else
                {
                    Debug.LogWarning($"Item tidak ditemukan: {itemData.itemName}");
                }
            }

            InformAboutChange();
        }

    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public List<ItemParameter> itemState;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
                itemState = new List<ItemParameter>(this.itemState)
            };
        }

        public static InventoryItem GetEmptyItem()
            => new InventoryItem
            {
                item = null,
                quantity = 0,
                itemState = new List<ItemParameter>()
            };
    }

    [Serializable]
    public class InventoryData
    {
        public List<InventoryItemData> items = new List<InventoryItemData>();
    }

    [Serializable]
    public class InventoryItemData
    {
        public string itemName;
        public int quantity;
        public List<ItemParameterData> itemParameters;
    }

    [Serializable]
    public class ItemParameterData
    {
        public string parameterName;
        public float value;
    }
}
