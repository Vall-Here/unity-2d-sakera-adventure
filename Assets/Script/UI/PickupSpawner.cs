


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;  // Pastikan namespace untuk ItemSO ter-include

public class PickupSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CoinPrefab, LeavesPrefab, PlankPrefab;

    [SerializeField] private ItemSO coinItemSO, leavesItemSO, plankItemSO;  // Tambahkan ItemSO untuk masing-masing item

    public void DropItems() 
    {
        int random = Random.Range(1, 4);

        if (random == 1) 
        {
            int randomCoin = Random.Range(0, 3);
            for (int i = 0; i < randomCoin; i++) 
            {
                SpawnItem(CoinPrefab, coinItemSO);
            }
        }

        if (random == 2) 
        {
            int randomLeaves = Random.Range(1, 3);
            for (int i = 0; i < randomLeaves; i++) 
            {
                SpawnItem(LeavesPrefab, leavesItemSO);
            }
        }

        if (random == 3) 
        {
            int randomPlank = Random.Range(1, 3);
            for (int i = 0; i < randomPlank; i++) 
            {
                SpawnItem(PlankPrefab, plankItemSO);
            }
        }
    }

    // Fungsi untuk menginstansiasi item dan assign ItemSO
    private void SpawnItem(GameObject itemPrefab, ItemSO itemSO)
    {
        GameObject itemInstance = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        Item itemScript = itemInstance.GetComponent<Item>();

        if (itemScript != null) 
        {
            itemScript.SetInventoryItem(itemSO);  
        }
    }
}
