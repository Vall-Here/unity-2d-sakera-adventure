using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject Coin, Health, Stamina;

    [SerializeField] private int dropamountStart;
    [SerializeField] private int dropamountEnd;

    public void DropItems () {

        int random = Random.Range(1, 4);

        if ( random == 1) {
            int randomCoin = Random.Range(0,  dropamountEnd);
            for (int i = 0; i < randomCoin; i++) {
                Instantiate(Coin, transform.position, Quaternion.identity);
            }
        }

        if ( random == 2) {
            int randomHealth = Random.Range(dropamountStart,  dropamountEnd);
            for (int i = 0; i < randomHealth; i++) {
                Instantiate(Health, transform.position, Quaternion.identity);
            }
        }

        if ( random == 3) {
            int randomStamina = Random.Range(dropamountStart, dropamountEnd);
            for (int i = 0; i < randomStamina; i++) {
                Instantiate(Stamina, transform.position, Quaternion.identity);
            }
        }
    }


}
