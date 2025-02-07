using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryPage : Singleton<UIInventoryPage>
    {
        [SerializeField]
        private UIInventoryItem itemPrefab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private UIInventoryDescription itemDescription;

        [SerializeField]
        private Button equipButton;  
        [SerializeField]
        private Button unequipButton; 

        List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
        public event Action<int> OnDescriptionRequested;
        public event Action<int> OnItemEquipped; 
        public event Action<int> OnItemUnequipped; 

        private int selectedItemIndex = -1;

        protected override void Awake()
        {
            base.Awake();
            Hide();
            itemDescription.ResetDescription();
            equipButton.onClick.AddListener(HandleEquipItem); 
            unequipButton.onClick.AddListener(HandleUnequipItem);  
        }

        public void InitializeInventoryUI(int inventorySize, bool[] hasItems)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                UIInventoryItem uiItem;
                uiItem = Instantiate(itemPrefab, contentPanel);
                uiItem.transform.localScale = Vector3.one;
                listOfUIItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;

            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            if (itemIndex >= 0 && itemIndex < listOfUIItems.Count)
            {
                listOfUIItems[itemIndex].Select();
                selectedItemIndex = itemIndex; 
            }
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }}
        //     else
        //     {
        //         Debug.LogWarning($"UIInventoryPage: Attempted to update non-existing slot at index {itemIndex}.");
        //     }
        // }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;

            OnDescriptionRequested?.Invoke(index);
        }

        private void HandleEquipItem()
        {
            if (selectedItemIndex != -1)
            {
                OnItemEquipped?.Invoke(selectedItemIndex); 
            }
        }

        private void HandleUnequipItem()
        {
            Debug.Log("UIInventoryPage: Unequip button clicked.");
            if (selectedItemIndex != -1)
            {
                OnItemUnequipped?.Invoke(selectedItemIndex); 
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
            selectedItemIndex = -1;
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
