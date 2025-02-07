

using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Pickup : MonoBehaviour
{
    // [SerializeField] private ItemId itemId; 
    [SerializeField] private float pickupDistance = 5f;
    [SerializeField] private InventorySO inventoryData;
    [SerializeField] private float acceleration = .2f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;

    private Vector3 moveDir;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        // inventoryData = GetComponent<InventorySO>();
    }

    private void Start() {
        StartCoroutine(animCurvesSpawnRoutine());
    }

    private void Update() {
        Vector3 playerPos = PlayerController.Instance.transform.position;

        if ( Vector3.Distance(transform.position, playerPos) < pickupDistance ) {
            moveDir = (playerPos - transform.position).normalized;
            speed += acceleration;
        } else {
            rb.velocity = Vector2.zero;
            speed = 0;
        }
    }

    private void FixedUpdate() {
        rb.velocity = speed * Time.deltaTime * moveDir;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.GetComponent<PlayerController>()) {
            DetectPickupType();
            Destroy(gameObject);
        }
    }

    private IEnumerator animCurvesSpawnRoutine() {
        Vector2 starPoint = transform.position;
        float randomX = transform.position.x + Random.Range(-2f, 2f);
        float randomY = transform.position.y + Random.Range(-1f, 1f);

        Vector2 endPoint = new Vector2(randomX, randomY);
        float timePassed = 0f;

        while (timePassed < popDuration) {
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(starPoint, endPoint, linearT) + new Vector2(0f, height);

            yield return null;
        }
    }

    // Menggunakan sistem item
private void DetectPickupType() {
    Item item = GetComponent<Item>();
    if (item == null) {
        return;
    }
    
    string itemName = item.InventoryItem.Name;
    // Debug.Log("Collected: " + itemName);
    switch (itemName) {
        case "Coin":
            EconomyManager.Instance.UpdateCurrentGold();
            break;
        case "Leaves":
            AddItemToInventory(item);
            break;
        case "Planks" :
            AddItemToInventory(item);
            break;
        case "HealthParticle" :
            PlayerController.Instance.GetComponent<PlayerHealth>().HealPlayer();
            break;
        case "StaminaParticle" :
            PlayerController.Instance.GetComponent<Stamina>().RegenStamina();
            break;
        case "Stones" :
            AddItemToInventory(item);
            break;
        case "Sabit Monteng" :
            AddItemToInventory(item);
            break;
        case "Busur Panah" :
            AddItemToInventory(item);
            break;
        case "Anak Panah" :
            AddItemToInventory(item);
            break;
        default :
            Debug.Log("Item not found");
            break;
    }
}




    private void AddItemToInventory(Item itemsparam) 
    {
        if (itemsparam != null)
        {
            Debug.Log("Attempting to add item: " + itemsparam.InventoryItem.Name + ", Quantity: " + itemsparam.Quantity);
            
            int reminder = inventoryData.AddItem(itemsparam.InventoryItem, itemsparam.Quantity);
            Debug.Log("Reminder after adding item: " + reminder);

            if (reminder == 0) 
            {
                itemsparam.DestroyItem(); 
                Debug.Log("Item destroyed: " + itemsparam.InventoryItem.Name);
            }
            else 
            {  
                itemsparam.Quantity = reminder;
                Debug.Log("Item not fully added, remaining quantity: " + reminder);
            }
        }
        else{
            Debug.LogError("Itemsparam is null!");
        }
    }

}






