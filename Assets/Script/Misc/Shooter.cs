using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] public Transform bulletSpawnPoint;

    public void Attack(){
        Vector2 TargetDirection = PlayerController.Instance.transform.position - transform.position;
        GameObject newBullet = Instantiate(projectilePrefab, bulletSpawnPoint.position, Quaternion.identity);
        newBullet.transform.right = TargetDirection;
    }
}
