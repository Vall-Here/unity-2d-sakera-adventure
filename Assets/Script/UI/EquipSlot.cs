

using System;
using Inventory;
using Inventory.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemTypeEnum.SlotItemType itemTypeEnum;  

    [SerializeField] private Image itemImage;  
    [SerializeField] private Sprite defaultImage;  

    [SerializeField] private EquipItemSO equipItemSO; 

    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private TMP_Text quantityText;
    
    [SerializeField] private bool allowsStacking = false;  

    public int slotIndex;  
    public event Action OnQuantityChanged;


    private void Awake(){
        initializeInventoryController(FindObjectOfType<InventoryController>());
        UpdateUI();
    }

    public bool AllowsStacking
    {
        get { return allowsStacking; }
    }

    public EquipItemSO EquipItemSO{
        get { return equipItemSO; }
    }
    public void initializeInventoryController (InventoryController iinventoryController){
        inventoryController = iinventoryController;
    }
    public void UpdateUI(){
        if (equipItemSO != null && !equipItemSO.IsEmpty)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = equipItemSO.EquippedItem.ItemImage; 

            if (equipItemSO.EquippedItem.IsStackable && equipItemSO.Quantity > 1)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = equipItemSO.Quantity.ToString();
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
            OnQuantityChanged?.Invoke();
        }
        else
        {
            ClearUI(); 
            OnQuantityChanged?.Invoke();
        }
    }

    public void ClearUI()
    {
        itemImage.gameObject.SetActive(false);
        itemImage.sprite = defaultImage;
        quantityText.gameObject.SetActive(false);  
    }
    public void SetEquipItemSO(EquipItemSO newEquipItemSO)
    {
        equipItemSO = newEquipItemSO; 
        UpdateUI();
    }
    public void EquipItem(ItemSO newItem, int quantity = 1)
        {
            if (equipItemSO != null)
            {
                if (allowsStacking && newItem.IsStackable)
                {
                    equipItemSO.Equip(newItem, quantity);  
                }
                else
                {
                    equipItemSO.Equip(newItem, 1);  
                }
                UpdateUI();  
            }
        }
    public void UnequipItem()
    {
        if (equipItemSO != null)
        {
            equipItemSO.Unequip(1); 
            UpdateUI();  
        }
    }

    public ItemSO GetItemSO()
    {
        return equipItemSO?.EquippedItem; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (equipItemSO != null && !equipItemSO.IsEmpty)
        {
           
            if (inventoryController != null)
            {
                int slotIndex = GetSlotIndex();
                Debug.Log($"Unequipping item from slot {slotIndex}");
                inventoryController.HandleItemUnequipped(slotIndex);
            }
        }
    }

    private int GetSlotIndex()
    {
        return slotIndex;
    }
}
