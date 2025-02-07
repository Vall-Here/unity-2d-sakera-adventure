using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;

public class ItemButton : MonoBehaviour
{
    [Header("UI Elements")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemPriceText;

    private ItemSO currentItemSO;
    private MerchantUIController uiController;

    public void Setup(ItemSO itemSO, MerchantUIController controller)
    {
        currentItemSO = itemSO;
        uiController = controller;

        // Mengatur ikon, nama, dan harga item di UI
        if (itemIcon != null)
            itemIcon.sprite = itemSO.ItemImage;
        
        if (itemNameText != null)
            itemNameText.text = itemSO.Name;
        
        if (itemPriceText != null)
            itemPriceText.text = itemSO.Price.ToString("D0"); // Format tanpa desimal

        // Menghapus listener sebelumnya untuk mencegah penambahan listener ganda
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnBuyButtonClicked);
        }
        else
        {
            Debug.LogWarning("Button component missing on ItemButton GameObject.");
        }
    }

    // Method yang dipanggil saat tombol dibeli diklik
    private void OnBuyButtonClicked()
    {
        if (uiController != null && uiController.currentMerchant != null)
        {
            uiController.currentMerchant.BuyItem(currentItemSO);
            uiController.OnItemPurchased();
        }
        else
        {
            Debug.LogWarning("UIController atau currentMerchant tidak ditemukan.");
        }
    }
}
