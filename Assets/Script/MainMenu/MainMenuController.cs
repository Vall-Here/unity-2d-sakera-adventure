using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public ConfirmationPopupController confirmationPopup;

    /// <summary>
    /// Dipanggil saat tombol "Play" ditekan untuk memulai game baru.
    /// </summary>
    public void PlayGame()
    {
        if (confirmationPopup != null)
        {
            confirmationPopup.Show("Apakah Anda yakin ingin memulai game baru? Semua progress saat ini akan hilang.", ConfirmStartNewGame);
        }
        else
        {
            Debug.LogError("MainMenuController: ConfirmationPopupController not assigned!");
        }
    }

    private void ConfirmStartNewGame()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.StartNewGame();
            // Debug.Log("MainMenuController: StartNewGame called.");
        }
        else
        {
            // Debug.LogError("MainMenuController: GameController Instance not found!");
        }
    }

    /// <summary>
    /// Dipanggil saat tombol "Load Game" ditekan untuk memuat game yang disimpan.
    /// </summary>
    
    
    
    
    
    public void LoadGame()
    {
    if (confirmationPopup != null)
            {
                confirmationPopup.Show("Apakah Anda yakin ingin memuat data save game?.", ConfirmLoadGame);
            }
            else
            {
                // Debug.LogError("MainMenuController: ConfirmationPopupController not assigned!");
            }
    }
    public void ConfirmLoadGame()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.LoadGame();
        }
    }

    /// <summary>
    /// Dipanggil saat tombol "Exit" ditekan untuk keluar dari aplikasi.
    /// </summary>
    public void ExitGame()
    {
        // Tampilkan popup konfirmasi sebelum keluar
        if (confirmationPopup != null)
        {
            confirmationPopup.Show("Apakah Anda yakin ingin keluar dari game?", ConfirmExit);
        }
        else
        {
            // Debug.LogError("MainMenuController: ConfirmationPopupController not assigned!");
        }
    }

    /// <summary>
    /// Aksi yang dipanggil saat konfirmasi exit diterima.
    /// </summary>
    private void ConfirmExit()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.ExitGame();
            Debug.Log("MainMenuController: ExitGame called.");
        }
        else
        {
            // Debug.LogError("MainMenuController: GameController Instance not found!");
        }
    }
}
