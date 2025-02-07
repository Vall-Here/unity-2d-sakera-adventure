using System.Collections;
using Inventory.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    public static DeathSceneManager Instance;
    private GameController gameController;
    private IngameUi ingameUi;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameController = GameController.Instance;
        ingameUi = IngameUi.Instance;
    }

    public void Respawn()
    {
        gameController.LoadCheckpoint();
        ingameUi.gameObject.SetActive(true);
    }


    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}
