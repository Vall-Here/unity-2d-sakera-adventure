using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public float startDelay = 2f;
    public int maxEnemies = 10;
    public float spawnRadius = 10f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public string sceneToCheck = "YourSceneName";

    private void Start()
    {
        // Mulai spawn musuh setelah startDelay
        StartCoroutine(SpawnEnemies());
        
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            if (spawnedEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomPositionWithinRadius();
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies.Add(newEnemy);
    }

    private Vector3 GetRandomPositionWithinRadius()
    {
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        return new Vector3(transform.position.x + randomPos.x, transform.position.y, transform.position.z + randomPos.y);
    }


    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == sceneToCheck)
        {
            DespawnAllEnemies();
        }
    }

    private void DespawnAllEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
