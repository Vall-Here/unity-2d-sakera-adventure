using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material whiteFlashMat;
    [SerializeField] private float flashTime = 0.2f;


    private Material defaultMat;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMat = spriteRenderer.material;
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public IEnumerator FlashRoutine(){
        spriteRenderer.material = whiteFlashMat;
        yield return new WaitForSeconds(flashTime);
        spriteRenderer.material = defaultMat;
        // enemyHealth.DetectDeath();
    }
}
