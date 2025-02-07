using UnityEngine;
using TMPro;
using Inventory.Model; // Pastikan namespace ini sesuai dengan struktur project Anda

public class QtyArrowText : MonoBehaviour
{
    [Tooltip("Assign the EquipSlot that holds the arrow items.")]
    [SerializeField] private EquipSlot equipSlot; // Assign di Inspector

    [Tooltip("Assign the TMP_Text component where quantity akan ditampilkan.")]
    [SerializeField] private TMP_Text quantityText; // Assign di Inspector

    private void Start()
    {
        if (equipSlot == null)
        {
            Debug.LogError("EquipSlot belum di-assign di QtyArrowText.");
            return;
        }

        if (quantityText == null)
        {
            Debug.LogError("TMP_Text belum di-assign di QtyArrowText.");
            return;
        }

        // Inisialisasi teks berdasarkan quantity saat game dimulai
        // UpdateQuantityText();

        // Subscribe ke event UpdateUI dari EquipSlot jika tersedia
        // Agar QtyArrowText dapat merespon perubahan quantity
        equipSlot.OnQuantityChanged += UpdateQuantityText;
    }

    private void OnDestroy()
    {
        // Pastikan untuk unsubscribe saat objek dihancurkan
        if (equipSlot != null)
        {
            equipSlot.OnQuantityChanged -= UpdateQuantityText;
        }
    }

    /// <summary>
    /// Update teks quantity berdasarkan EquipSlot.
    /// </summary>
    private void UpdateQuantityText()
    {
        if (equipSlot.EquipItemSO != null && equipSlot.EquipItemSO.EquippedItem.IsStackable)
        {
            int quantity = equipSlot.EquipItemSO.Quantity;
            quantityText.text = quantity.ToString();
            quantityText.gameObject.SetActive(quantity > 1); // Tampilkan hanya jika quantity > 1
        }
        else
        {
            quantityText.gameObject.SetActive(false); // Sembunyikan jika tidak stackable atau tidak ada item
        }
    }
}
