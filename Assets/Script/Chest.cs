using System.Collections.Generic;
using Inventory.Model;
using Inventory.UI;
using UnityEngine;

namespace Inventory.Controller
{
    public class Chest : MonoBehaviour
    {
        [SerializeField]
        private ChestInventorySO chestInventory;

        [SerializeField]
        private UIInventoryPage chestUI;

        private bool isOpen = false;

        private void Start()
        {
            if (chestInventory == null || chestUI == null)
            {
                Debug.LogError("Chest: ChestInventorySO atau ChestUI belum di-assign.");
                return;
            }

            chestInventory.Initialize();
            chestInventory.OnChestUpdated += UpdateChestUI;
            chestUI.gameObject.SetActive(false);
        }

        private void Update()
        {
            // Implementasikan logika pembukaan chest, misalnya dengan mendeteksi jarak atau input
            // Contoh sederhana: Tekan tombol 'E' ketika berada di dekat chest
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
            {
                ToggleChest();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (isOpen)
                {
                    ToggleChest();
                }
            }
        }

        private void ToggleChest()
        {
            isOpen = !isOpen;
            chestUI.gameObject.SetActive(isOpen);
            if (isOpen)
            {
                UpdateChestUI(chestInventory.GetCurrentChestState());
            }
            else
            {
                chestUI.gameObject.SetActive(false);
            }
        }

        private void UpdateChestUI(Dictionary<int, InventoryItem> chestState)
        {
            // Asumsikan chestUI adalah instance dari UIInventoryPage
            // Anda mungkin perlu membuat UI khusus untuk chest jika ingin menampilkan kedua inventaris sekaligus

            // Contoh: Reset dan update chest UI
            chestUI.ResetAllItems();
            foreach (var item in chestState)
            {
                chestUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void OnDestroy()
        {
            chestInventory.OnChestUpdated -= UpdateChestUI;
        }
    }
}
