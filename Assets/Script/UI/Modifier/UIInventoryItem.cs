using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;

        [SerializeField]
        private Image borderImage;

        public event Action<UIInventoryItem> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
            OnRightMouseBtnClick;

        private bool empty = true;
        private int quantity; 

        public void Awake()
        {
            ResetData();
            Deselect();

            if (itemImage.sprite != null && quantity > 0) 
            {
                SetData(itemImage.sprite, quantity);
            }
            
        }

        public void InitializeItem(Sprite sprite, int quantity)
        {
            if (sprite != null && quantity > 0)
            {
                SetData(sprite, quantity);
            }
        }

        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            quantityTxt.gameObject.SetActive(false);
            empty = true;
        }

        public void Deselect()
        {
            borderImage.enabled = false;
        }

        public void SetData(Sprite sprite, int quantity)
        {
            // Debug.Log("SetData");
            itemImage.gameObject.SetActive(true);
            quantityTxt.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            this.quantity = quantity; // Menyimpan quantity
            quantityTxt.text = quantity.ToString(); // Mengubah quantity menjadi string dan menampilkannya
            empty = false;
        }

        public void Select()
        {
            borderImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Handle drag event if needed
        }
    }
}
