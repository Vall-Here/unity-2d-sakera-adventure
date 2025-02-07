using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory.Model;

public class MerchantUIController : Singleton<MerchantUIController>
{
    [Header("UI Elements")]
    public Transform itemsContainer; 
    public GameObject itemButtonPrefab; 
    public TMP_Text playerGoldText; 
    public Button exitButton;
    public GameObject panel;

    [HideInInspector]
    public Merchant currentMerchant; 

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false); 
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseMerchantUI);
        }
    }

    public void OpenMerchantUI(Merchant merchant)
    {
        currentMerchant = merchant;
        if (panel != null)
        {
            panel.SetActive(true);
        }
        PopulateItems(merchant);
        UpdateGoldDisplay();
    }

    public void CloseMerchantUI()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        currentMerchant = null;
    }

    public void PopulateItems(Merchant merchant)
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemSO itemSO in merchant.itemsForSale)
        {
            GameObject itemButton = Instantiate(itemButtonPrefab, itemsContainer);
            ItemButton ib = itemButton.GetComponent<ItemButton>();
            ib.Setup(itemSO, this);
        }
    }

    public void UpdateGoldDisplay()
    {
        playerGoldText.text = "Gold: " + EconomyManager.Instance.CurrentGold.ToString("D3");
    }

    public void OnItemPurchased()
    {
        UpdateGoldDisplay();
    }
}
