using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;

        [SerializeField] private InventorySO inventoryData;

        public List<InventoryItem> initialItems = new List<InventoryItem>();

        [SerializeField] private ActiveInventory activeInventory; 
        [SerializeField] private EquipmentManager equipmentManager;

        [SerializeField] private ChestInventorySO chestInventory;

        private void Start()
        {
            if (inventoryUI == null || inventoryData == null || activeInventory == null || equipmentManager == null)
            {
                Debug.LogError("InventoryController: One or more references are not set.");
                return;
            }
            // PrepareUI();
            // PrepareInventoryData();
        }  

        public void InitializeReferences(UIInventoryPage uI,InventorySO data, ActiveInventory activeInv, EquipmentManager equipManager)
        {
            inventoryUI = uI;
            inventoryData = data;
            activeInventory = activeInv;
            equipmentManager = equipManager;

            PrepareUI();
            PrepareInventoryData();
        }

        private void PrepareInventoryData()
        {
            // inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
                Debug.Log($"InventoryController: Added {item.item.name} to inventory.");
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, 
                    item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            bool[] hasItems = new bool[inventoryData.Size];

            inventoryUI.InitializeInventoryUI(inventoryData.Size, hasItems);
            
            // Subscribe to UI events
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnItemEquipped += HandleItemEquipped;   
            inventoryUI.OnItemUnequipped += HandleItemUnequipped;  
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
        }

        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void HandleItemEquipped(int itemIndex)
        {
            if (inventoryData == null)
            {
                Debug.LogError("inventoryData is null!");
                return;
            }
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                Debug.LogError($"Inventory item at index {itemIndex} is null or empty.");
                return;
            }
            ItemSO itemToEquip = inventoryItem.item;
            if (itemToEquip == null)
            {
                Debug.LogError($"Item to equip at index {itemIndex} is null.");
                return;
            }
            if (equipmentManager == null)
            {
                Debug.LogError("EquipmentManager is not assigned.");
                return;
            }
            if (equipmentManager.EquipItem(itemToEquip))
            {
                inventoryData.RemoveItem(itemIndex, 1);
                inventoryUI.ResetAllItems();
                UpdateInventoryUI(inventoryData.GetCurrentInventoryState());
            }
            else
            {
                Debug.LogWarning($"Failed to equip item {itemToEquip.name}.");
            }
        }

        public void HandleItemUnequipped(int slotIndex)
        {
            if (equipmentManager == null)
            {
                Debug.LogError("EquipmentManager is not assigned.");
                return;
            }
            ItemSO equippedItem = equipmentManager.GetEquippedItemByIndex(slotIndex);
            if (equippedItem == null)
            {
                Debug.LogWarning($"No equipped item found at index {slotIndex}.");
                return;
            }
            equipmentManager.UnequipItem(slotIndex);
            inventoryData.AddItem(equippedItem, 1);
            inventoryUI.ResetAllItems();
            UpdateInventoryUI(inventoryData.GetCurrentInventoryState());
        }

        public bool TransferItemToChest(int playerItemIndex, int quantity)
        {
            InventoryItem itemToTransfer = inventoryData.GetItemAt(playerItemIndex);
            if (itemToTransfer.IsEmpty)
                return false;

            int remaining = chestInventory.AddItem(itemToTransfer.item, quantity, itemToTransfer.itemState);
            if (remaining < quantity)
            {
                inventoryData.RemoveItem(playerItemIndex, quantity - remaining);
                return true;
            }
            return false;
        }

        // Tambahkan metode untuk mentransfer item dari chest ke player
        public bool TransferItemFromChest(int chestItemIndex, int quantity)
        {
            InventoryItem itemToTransfer = chestInventory.GetItemAt(chestItemIndex);
            if (itemToTransfer.IsEmpty)
                return false;

            int remaining = inventoryData.AddItem(itemToTransfer.item, quantity, itemToTransfer.itemState);
            if (remaining < quantity)
            {
                chestInventory.RemoveItem(chestItemIndex, quantity - remaining);
                return true;
            }
            return false;
        }
    }
}
