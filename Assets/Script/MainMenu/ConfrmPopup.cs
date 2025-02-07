using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationPopupController : MonoBehaviour
{
    public TMP_Text confirmationText; 
    public Button confirmButton; 
    public Button cancelButton; 

    private System.Action onConfirm; 

    private void Awake()
    {
        gameObject.SetActive(false);

        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    /// <summary>
    /// </summary>
    /// <param name="message">Pesan konfirmasi yang akan ditampilkan.</param>
    /// <param name="confirmAction">Aksi yang akan dieksekusi jika konfirmasi diterima.</param>
    public void Show(string message, System.Action confirmAction)
    {
        confirmationText.text = message;
        onConfirm = confirmAction;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Menangani klik pada tombol konfirmasi (Ya).
    /// </summary>
    private void OnConfirmClicked()
    {
        gameObject.SetActive(false);
        onConfirm?.Invoke();
    }

    /// <summary>
    /// Menangani klik pada tombol batal (Tidak).
    /// </summary>
    private void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }
}
