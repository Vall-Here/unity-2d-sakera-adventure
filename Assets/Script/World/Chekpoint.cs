using Inventory.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public InventorySO playerInventorySO;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            GameData newData = new GameData();
            newData.currentScene = SceneManager.GetActiveScene().name;
            newData.lastCheckpoint = new CheckpointData
            {
                sceneName = newData.currentScene,
                position = new float[] { transform.position.x, transform.position.y, transform.position.z }
            };

            if (playerInventorySO != null)
            {
                newData.inventoryData = playerInventorySO.GetInventoryData(); // Sesuaikan dengan metode Anda
            }

            SaveSystem.SaveGame(newData, playerInventorySO);
            Debug.Log($"Checkpoint: Game saved at checkpoint {newData.lastCheckpoint.position[0]}, {newData.lastCheckpoint.position[1]}, {newData.lastCheckpoint.position[2]}");

            if (NotificationManager.Instance != null)
            {
                NotificationManager.Instance.ShowNotification("Checkpoint Saved!");
            }
            else
            {
                Debug.LogWarning("Checkpoint: NotificationManager instance is null!");
            }
        }
    }
}




