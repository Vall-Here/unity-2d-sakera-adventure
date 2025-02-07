// SaveSystem.cs
using System.IO;
using Inventory.Model;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.json");

    public static void SaveGame(GameData newData, InventorySO inventorySO)
    {
        GameData existingData = LoadGame() ?? new GameData();

        existingData.currentScene = newData.currentScene;
        existingData.playerPosition = newData.playerPosition;
        existingData.currentHealth = PlayerHealth.Instance.currentHealth;
        existingData.gold = EconomyManager.Instance.CurrentGold;

        if (inventorySO != null)
        {
            existingData.inventoryData = inventorySO.GetInventoryData();
        }

        existingData.isDay = newData.isDay;
        existingData.timeElapsed = newData.timeElapsed;
        existingData.dayDuration = newData.dayDuration;

        string json = JsonUtility.ToJson(existingData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("SaveSystem: Game berhasil disimpan.");
    }

    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("SaveSystem: Game berhasil dimuat.");
            return data;
        }
        else
        {
            Debug.LogWarning("SaveSystem: Tidak ada data save ditemukan!");
            return null;
        }
    }

    public static void DeleteSave()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogWarning("SaveSystem: Tidak ada data save yang ditemukan untuk dihapus.");
        }


    }
}
