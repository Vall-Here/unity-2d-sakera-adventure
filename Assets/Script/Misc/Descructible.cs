using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Descructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVFX;
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<DamageSource>() || other.gameObject.GetComponent<Projectile>()) {
            GetComponent<PickupSpawner>().DropItems();
            Instantiate(destroyedVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    } 

}
