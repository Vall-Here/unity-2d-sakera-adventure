using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField] private float despawnTime = 20f; 

    private void Start()
    {
        StartCoroutine(DespawnAfterTime());
    }

    private IEnumerator DespawnAfterTime()
    {
        // Tunggu selama waktu despawnTime
        yield return new WaitForSeconds(despawnTime);

        // Hancurkan gameObject ini setelah waktu habis
        Destroy(gameObject);
    }
}
