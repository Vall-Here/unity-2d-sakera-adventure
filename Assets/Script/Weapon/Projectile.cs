using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;

    private WeaponInfo weaponInfo;
    private Vector3 StartPosition;

    private void Start() {
        StartPosition = transform.position;
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    public void UpdateWeaponInfo(WeaponInfo weaponInfo)
    {
        this.weaponInfo = weaponInfo;
    }

    public void UpdateProjectileRange(float projectileRange) {
        this.projectileRange = projectileRange;
    }


    private void OnTriggerEnter2D(Collider2D other) {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        Indiestructible indestructible = other.GetComponent<Indiestructible>();
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();


        if(!other.isTrigger && (enemyHealth || indestructible || playerHealth)){
            if((playerHealth && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile) || indestructible) {
                if (playerHealth != null && isEnemyProjectile) {
                    playerHealth.TakeDamage(1, transform, 5f);
                }
                else if (enemyHealth && !isEnemyProjectile) {
                    // Debug.Log("Enemy Health");
                    enemyHealth?.TakeDamage(weaponInfo.weaponDamage);
                    // Debug.Log(weaponInfo.weaponDamage);
                }
                Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
                Destroy(gameObject);
            } else if (other.isTrigger && indestructible) {
            Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        } }
 
    }

    private void DetectFireDistance()
    {
        if(Vector3.Distance(StartPosition, transform.position) > projectileRange)
        {
            Destroy(gameObject);
        }
    }
}
