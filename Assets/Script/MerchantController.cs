using System.Collections.Generic;
using Inventory.Model;
using UnityEngine;
using UnityEngine.InputSystem;

public class Merchant : MonoBehaviour
{
    public List<ItemSO> itemsForSale; 
    private bool isPlayerInRange = false;
    
    private PlayerControls playerControls;
    
    [SerializeField] public Transform itemSpawnPoint;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Interact.interaction.performed += OnInteractPerformed;
        playerControls.Interact.Enable();
    }

    private void OnDisable()
    {
        playerControls.Interact.interaction.performed -= OnInteractPerformed;
        playerControls.Interact.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (isPlayerInRange)
        {
            MerchantUIController.Instance.OpenMerchantUI(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            MerchantUIController.Instance.CloseMerchantUI();
        }
    }

    public void BuyItem(ItemSO itemSO)
    {
        if (EconomyManager.Instance.CurrentGold >= itemSO.Price)
        {
            EconomyManager.Instance.CurrentGold -= itemSO.Price;

            Vector3 spawnPosition = itemSpawnPoint.position;
            GameObject instantiatedItem = Instantiate(itemSO.ItemUIPrefab, spawnPosition, Quaternion.identity);
            instantiatedItem.transform.localScale = Vector3.one;
        }
        MerchantUIController.Instance.OnItemPurchased();
    }
}
