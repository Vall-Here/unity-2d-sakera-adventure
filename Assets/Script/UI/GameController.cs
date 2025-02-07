// GameController.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Inventory.Model;
using Inventory;
using Inventory.UI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public InventorySO playerInventorySO;
    public GameObject playerPrefab;

    public static GameController Instance { get; private set; }

    private GameObject playerInstance;

    public delegate void PlayerInstantiatedAction(GameObject player);
    public event PlayerInstantiatedAction OnPlayerInstantiated;

    [SerializeField]
    private string firstGameSceneName = "House1";

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
    }

    public void SaveGameManually()
    {
        if (playerInstance == null)
        {
            return;
        }

        GameData newData = new GameData();
        newData.currentScene = SceneManager.GetActiveScene().name;
        newData.playerPosition = new float[]
        {
            playerInstance.transform.position.x,
            playerInstance.transform.position.y,
            playerInstance.transform.position.z
        };
        newData.currentHealth = PlayerHealth.Instance.currentHealth;
        newData.gold = EconomyManager.Instance.CurrentGold;

        if (playerInventorySO != null)
        {
            newData.inventoryData = playerInventorySO.GetInventoryData();
        }

        if (DayNightCycle.Instance != null)
        {
            var timeData = DayNightCycle.Instance.GetTimeData();
            newData.isDay = timeData.Item1;
            newData.timeElapsed = timeData.Item2;
            newData.dayDuration = timeData.Item3;
        }

        SaveSystem.SaveGame(newData, playerInventorySO);
    }

    private void OnApplicationQuit()
    {
        SaveGameManually();
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Memulai game baru dengan menginstansiasi player di scene pertama
    /// </summary>
    public void StartNewGame()
    {
        SaveSystem.DeleteSave();

        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.LoadTime(true, 450f, 1800f);
            DayNightCycle.Instance.ResetReferences();
        }
      
        SceneManager.LoadScene(firstGameSceneName);
        StartCoroutine(InstantiatePlayerAfterSceneLoad(Vector3.zero));
    }

    /// <summary>
    /// Memuat game yang disimpan
    /// </summary>
    public void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            StartCoroutine(LoadGameCoroutine(data));
        }
        else
        {
            // Debug.LogWarning("GameController: Tidak ada data save ditemukan!");
        }
    }

    /// <summary>
    /// Coroutine untuk memuat game
    /// </summary>
    private IEnumerator LoadGameCoroutine(GameData data)
    {
        // Debug.Log($"GameController: LoadGameCoroutine dimulai. Memuat scene: {data.currentScene}");

        if (data.currentScene != SceneManager.GetActiveScene().name)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.currentScene);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            // Debug.Log($"GameController: Scene {data.currentScene} telah dimuat.");
        }
        else
        {
            // Debug.Log("GameController: Scene saat ini sama dengan scene yang disimpan.");
        }

        yield return null;

        if (playerPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(
                data.playerPosition[0],
                data.playerPosition[1],
                data.playerPosition[2]
            );
            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            // Debug.Log($"GameController: Player diinstansiasi di posisi: {playerInstance.transform.position}");

            OnPlayerInstantiated?.Invoke(playerInstance);
        }
        else
        {
            // Debug.LogError("GameController: Player Prefab belum diassign!");
        }

        // Inisialisasi komponen player
        if (playerInstance != null)
        {
            InventoryController inventoryController = playerInstance.GetComponent<InventoryController>();
            if (inventoryController != null)
            {
                UIInventoryPage uiInventoryPage = UIInventoryPage.Instance;
                InventorySO inventorySO = InventorySO.Instance;
                ActiveInventory activeInv = ActiveInventory.Instance;
                EquipmentManager equipManager = EquipmentManager.Instance;

                if (equipManager != null)
                {
                    foreach (EquipSlot equipSlot in equipManager.equipSlotUI)
                    {
                        if (equipSlot != null)
                        {
                            equipSlot.initializeInventoryController(inventoryController);
                        }
                    }
                }

                inventoryController.InitializeReferences(uiInventoryPage, inventorySO, activeInv, equipManager);
                // Debug.Log("GameController: Referensi InventoryController diinisialisasi.");
            }
            else
            {
                // Debug.LogError("GameController: Komponen InventoryController tidak ditemukan pada prefab player!");
            }

            PlayerController playerController = playerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                UIManager uiManager = UIManager.Instance;
                playerController.InitializeReferences(uiManager);
                // Debug.Log("GameController: Referensi PlayerController diinisialisasi.");
            }
            else
            {
                // Debug.LogError("GameController: Komponen PlayerController tidak ditemukan pada prefab player!");
            }

            Stamina stamina = playerInstance.GetComponent<Stamina>();
            if (stamina != null && UIManager.Instance != null)
            {
                Transform staminaContainer = UIManager.Instance.GetStaminaContainer();
                if (staminaContainer != null)
                {
                    stamina.SetStaminaContainer(staminaContainer);
                    // Debug.Log("GameController: Referensi Stamina diinisialisasi.");
                }
                else
                {
                    // Debug.LogError("GameController: UIManager - Stamina Container tidak ditemukan!");
                }
            }


            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null && UIManager.Instance != null)
            {
                Slider healthSlider = UIManager.Instance.GetHealthSlider();
                Image healthFillImage = UIManager.Instance.GetHealthFillImage();
                playerHealth.InitializeReferences(healthSlider, healthFillImage);
                Debug.Log("GameController: Referensi PlayerHealth diinisialisasi.");
            }
 
        }

        // Memuat data inventory
        if (data.inventoryData != null)
        {
            InventorySO.Instance.SetInventoryData(data.inventoryData);
            // Debug.Log("GameController: Inventory data diatur setelah inisialisasi InventoryController.");
        }

        // Memuat data health
        if (playerInstance != null)
        {
            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetHealth(data.currentHealth);
            }
        }

        // Memuat data gold
        if (playerInstance != null)
        {
            EconomyManager.Instance.SetGold(data.gold);
        }

        // Memuat data waktu
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.LoadTime(data.isDay, data.timeElapsed, data.dayDuration);
            // Debug.Log("GameController: Waktu in-game dimuat.");
        }


        DayNightCycle.Instance?.ResetReferences();
        // Debug.Log("GameController: Game berhasil dimuat.");
    }

    /// <summary>
    /// Method untuk keluar dari game
    /// </summary>
    public void ExitGame()
    {
        SaveGameManually();
        Application.Quit();
    }

    /// <summary>
    /// Menginstansiasi player setelah scene dimuat
    /// </summary>
    /// <param name="position">Posisi spawn player</param>
    private IEnumerator InstantiatePlayerAfterSceneLoad(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        if (playerPrefab != null)
        {
            playerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
            // Debug.Log("GameController: Player diinstansiasi untuk game baru.");

            OnPlayerInstantiated?.Invoke(playerInstance);
        }

        // Inisialisasi komponen player
        if (playerInstance != null)
        {
            InventoryController inventoryController = playerInstance.GetComponent<InventoryController>();
            if (inventoryController != null)
            {
                UIInventoryPage uiInventoryPage = UIInventoryPage.Instance;
                InventorySO inventorySO = InventorySO.Instance;
                ActiveInventory activeInv = ActiveInventory.Instance;
                EquipmentManager equipManager = EquipmentManager.Instance;

                if (equipManager != null)
                {
                    foreach (EquipSlot equipSlot in equipManager.equipSlotUI)
                    {
                        if (equipSlot != null)
                        {
                            equipSlot.initializeInventoryController(inventoryController);
                        }
                    }
                }

                inventoryController.InitializeReferences(uiInventoryPage, inventorySO, activeInv, equipManager);
                // Debug.Log("GameController: Referensi InventoryController diinisialisasi.");
            }

            PlayerController playerController = playerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                UIManager uiManager = UIManager.Instance;
                playerController.InitializeReferences(uiManager);
                // Debug.Log("GameController: Referensi PlayerController diinisialisasi.");
            }

            Stamina stamina = playerInstance.GetComponent<Stamina>();
            if (stamina != null && UIManager.Instance != null)
            {
                Transform staminaContainer = UIManager.Instance.GetStaminaContainer();
                if (staminaContainer != null)
                {
                    stamina.SetStaminaContainer(staminaContainer);
                    // Debug.Log("GameController: Referensi Stamina diinisialisasi.");
                }

            }

            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null && UIManager.Instance != null)
            {
                Slider healthSlider = UIManager.Instance.GetHealthSlider();
                Image healthFillImage = UIManager.Instance.GetHealthFillImage();
                playerHealth.InitializeReferences(healthSlider, healthFillImage);
                // Debug.Log("GameController: Referensi PlayerHealth diinisialisasi.");
            }
            
            EconomyManager.Instance.SetGold(20);}
        if (DayNightCycle.Instance != null){
            DayNightCycle.Instance.LoadTime(true, 450f, 1800);
        }
        DayNightCycle.Instance?.ResetReferences();
        // Debug.Log("GameController: Player diinstansiasi dan diatur untuk game baru.");
    }

    /// <summary>
    /// Fungsi baru untuk memuat checkpoint terakhir
    /// </summary>
    public void LoadCheckpoint()
    {
        GameData data = SaveSystem.LoadGame();
        if (data != null)
        {
            StartCoroutine(LoadCheckpointCoroutine(data));
        }
        else
        {
            StartCoroutine(LoadDefaultCheckpoint());
            Debug.LogWarning("GameController: Tidak ada data save ditemukan untuk checkpoint!");
        }
    }

    /// <summary>
    /// Coroutine untuk memuat checkpoint
    /// </summary>
    private IEnumerator LoadCheckpointCoroutine(GameData data)
    {
        if (data.lastCheckpoint == null)
        {
            yield return StartCoroutine(LoadDefaultCheckpoint());
            yield break;
        }

 

        if (data.lastCheckpoint.sceneName != SceneManager.GetActiveScene().name)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.lastCheckpoint.sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Debug.Log($"GameController: Scene {data.lastCheckpoint.sceneName} telah dimuat.");
        }

        yield return null;

        if (playerPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(
                data.lastCheckpoint.position[0],
                data.lastCheckpoint.position[1],
                data.lastCheckpoint.position[2]
            );
            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"GameController: Player diinstansiasi di posisi: {playerInstance.transform.position}");

            OnPlayerInstantiated?.Invoke(playerInstance);
        }

        // Inisialisasi komponen player
        if (playerInstance != null)
        {
            InventoryController inventoryController = playerInstance.GetComponent<InventoryController>();
            if (inventoryController != null)
            {
                UIInventoryPage uiInventoryPage = UIInventoryPage.Instance;
                InventorySO inventorySO = InventorySO.Instance;
                ActiveInventory activeInv = ActiveInventory.Instance;
                EquipmentManager equipManager = EquipmentManager.Instance;

                if (equipManager != null)
                {
                    foreach (EquipSlot equipSlot in equipManager.equipSlotUI)
                    {
                        if (equipSlot != null)
                        {
                            equipSlot.initializeInventoryController(inventoryController);
                        }
                    }
                }

                inventoryController.InitializeReferences(uiInventoryPage, inventorySO, activeInv, equipManager);
                Debug.Log("GameController: Referensi InventoryController diinisialisasi.");
            }


            PlayerController playerController = playerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                UIManager uiManager = UIManager.Instance;
                playerController.InitializeReferences(uiManager);
                Debug.Log("GameController: Referensi PlayerController diinisialisasi.");
            }

            Stamina stamina = playerInstance.GetComponent<Stamina>();
            if (stamina != null && UIManager.Instance != null)
            {
                Transform staminaContainer = UIManager.Instance.GetStaminaContainer();
                if (staminaContainer != null)
                {
                    stamina.SetStaminaContainer(staminaContainer);
                    Debug.Log("GameController: Referensi Stamina diinisialisasi.");
                }

            }
            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null && UIManager.Instance != null)
            {
                Slider healthSlider = UIManager.Instance.GetHealthSlider();
                Image healthFillImage = UIManager.Instance.GetHealthFillImage();
                playerHealth.InitializeReferences(healthSlider, healthFillImage);
                Debug.Log("GameController: Referensi PlayerHealth diinisialisasi.");
            }
            else
            {
                if (playerHealth == null)
                {
                    // Debug.LogError("GameController: Komponen PlayerHealth tidak ditemukan pada prefab player!");
                }
                if (UIManager.Instance == null)
                {
                    // Debug.LogError("GameController: Instance UIManager tidak ditemukan!");
                }
            }
        }

        // Memuat data inventory
        if (data.inventoryData != null)
        {
            InventorySO.Instance.SetInventoryData(data.inventoryData);

        }

        // Memuat data health
        if (playerInstance != null)
        {
            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetHealth(data.currentHealth);
            }
        }

        // Memuat data gold
        if (playerInstance != null)
        {
            EconomyManager.Instance.SetGold(data.gold);
        }

        // Memuat data waktu
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.LoadTime(data.isDay, data.timeElapsed, data.dayDuration);
            DayNightCycle.Instance.ResetReferences();

        }
        else
        {
            // Debug.LogWarning("GameController: DayNightCycle.Instance tidak tersedia!");
        }

        // PixelCrushers.QuestMachine.Demo.DemoMenu.print("GameController: Game berhasil dimuat.");

        Debug.Log("GameController: Game berhasil dimuat.");
    }

    private IEnumerator LoadDefaultCheckpoint()
    {
        Debug.Log("GameController: Memuat default checkpoint di scene House1, posisi (0,0,0).");

        if (firstGameSceneName != SceneManager.GetActiveScene().name)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(firstGameSceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Debug.Log($"GameController: Scene {firstGameSceneName} telah dimuat.");
        }
  
        yield return null;

        if (playerPrefab != null)
        {
            Vector3 spawnPosition = Vector3.zero;
            playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"GameController: Player diinstansiasi di posisi default: {playerInstance.transform.position}");

            OnPlayerInstantiated?.Invoke(playerInstance);
        }
  

        // Inisialisasi komponen player
        if (playerInstance != null)
        {
            InventoryController inventoryController = playerInstance.GetComponent<InventoryController>();
            if (inventoryController != null)
            {
                UIInventoryPage uiInventoryPage = UIInventoryPage.Instance;
                InventorySO inventorySO = InventorySO.Instance;
                ActiveInventory activeInv = ActiveInventory.Instance;
                EquipmentManager equipManager = EquipmentManager.Instance;

                if (equipManager != null)
                {
                    foreach (EquipSlot equipSlot in equipManager.equipSlotUI)
                    {
                        if (equipSlot != null)
                        {
                            equipSlot.initializeInventoryController(inventoryController);
                        }
                    }
                }

                inventoryController.InitializeReferences(uiInventoryPage, inventorySO, activeInv, equipManager);

            }

            PlayerController playerController = playerInstance.GetComponent<PlayerController>();
            if (playerController != null)
            {
                UIManager uiManager = UIManager.Instance;
                playerController.InitializeReferences(uiManager);
            }

            Stamina stamina = playerInstance.GetComponent<Stamina>();
            if (stamina != null && UIManager.Instance != null)
            {
                Transform staminaContainer = UIManager.Instance.GetStaminaContainer();
                if (staminaContainer != null)
                {
                    stamina.SetStaminaContainer(staminaContainer);
                    Debug.Log("GameController: Referensi Stamina diinisialisasi.");
                }
            }

            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null && UIManager.Instance != null)
            {
                Slider healthSlider = UIManager.Instance.GetHealthSlider();
                Image healthFillImage = UIManager.Instance.GetHealthFillImage();
                playerHealth.InitializeReferences(healthSlider, healthFillImage);
            }
        }

        // Memuat data waktu in-game
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.LoadTime(true, 450f, 1800f); 
        }
        else
  

        Debug.Log("GameController: Default checkpoint berhasil dimuat.");
    }


}
