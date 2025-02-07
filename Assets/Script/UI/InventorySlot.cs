
using Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] public ItemSO itemSO; 
    [SerializeField] private int slotIndex; 

    public ItemTypeEnum.SlotItemType itemTypeEnum; 
    [SerializeField] private Image itemImage; 
    [SerializeField] private Sprite defaultImage; 

    private EquipmentManager equipmentManager;
    [SerializeField] private TMP_Text quantityText; 


    public void Initialize(EquipmentManager manager)
    {
        equipmentManager = manager;
        SyncWithEquipment();
        UpdateUI();
    }
    public ItemSO GetWeaponInfo()
    {
        return itemSO;
    }

    public ItemSO GetItemSO()
    {
        return itemSO;
    }
    public void EquipItem(ItemSO newItem)
    {
        itemSO = newItem;
        UpdateUI(); 
    }

    public void UnequipItem()
    {
        itemSO = null;
        ClearUI();
    }

    private void UpdateUI()
    {
        if (itemSO != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemSO.ItemImage; 
        }
        else
        {
            ClearUI(); 
        }
    }

    private void ClearUI()
    {
        itemImage.gameObject.SetActive(false);
        itemImage.sprite = defaultImage;
        quantityText.gameObject.SetActive(false); 
    }

 
    public void SyncWithEquipment()
    {
        if (equipmentManager != null)
        {
            ItemSO equippedItem = equipmentManager.GetEquippedItemByIndex(slotIndex);
            if (equippedItem != null)
            {
                EquipItem(equippedItem); 
                int quantity = equipmentManager.GetEquippedItemQuantity(slotIndex); 
                SetQuantity(quantity);
            }
            else
            {
                UnequipItem(); 
                SetQuantity(0);
            }
        }
    }

    private void SetQuantity(int quantity)
    {
        if (itemSO != null && itemSO.IsStackable)
        {
            if (quantity > 1)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.text = quantity.ToString();
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
    }

}
