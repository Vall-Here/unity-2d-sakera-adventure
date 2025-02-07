using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public int damageAmount = 1;
 
    private void OnTriggerEnter2D(Collider2D other)
    {


        if(other.gameObject.GetComponent<EnemyHealth>()){
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);
        }

    }
}

