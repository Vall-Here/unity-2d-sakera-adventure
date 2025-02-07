using System.Numerics;
using Inventory.Model;

[System.Serializable]
public class GameData
{
    public string currentScene;
    public float[] playerPosition;
    public InventoryData inventoryData;
    public CheckpointData lastCheckpoint;
    public int currentHealth;
    public int gold;
    
    public bool isDay;
    public float timeElapsed; 
    public float dayDuration; 
}

[System.Serializable]
public class CheckpointData
{
    public string sceneName;
    public float[] position;
}