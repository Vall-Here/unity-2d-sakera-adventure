using UnityEngine;
using UnityEngine.Tilemaps; // Untuk akses ke Tilemap

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] objectToSpawn;
    [SerializeField] private GameObject ENV;
    [SerializeField] private Transform spawnAreaCenter;
    [SerializeField] private float spawnAreaRadius = 10f;
    [SerializeField] private float spawnCooldown = 5f;
    [SerializeField] private float requiredDistance = 20f;
    [SerializeField] private int objectLimit = 5;
    [SerializeField] private bool destroyOldestIfLimitReached = false;
    [SerializeField] private Tilemap[] restrictedTilemaps; // Array untuk beberapa Tilemap

    private Camera playerCamera;
    private float spawnTimer = 0f;

    void Start()
    {
        playerCamera = Camera.main;
        spawnTimer = spawnCooldown;
    }

    void Update()
    {
        // Hitung jarak antara kamera player dan pusat area spawn
        float distance = Vector3.Distance(playerCamera.transform.position, spawnAreaCenter.position);

    
        if (distance > requiredDistance)
        {
            if (ENV.transform.childCount < objectLimit)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0f)
                {
                    SpawnObject(); 
                    spawnTimer = spawnCooldown; 
                }
            }
            else if (ENV.transform.childCount >= objectLimit && destroyOldestIfLimitReached)
            {
                DestroyOldestChild(); 
            }
        }
    }


    private void SpawnObject()
    {
        Vector3 randomPosition = GetRandomPositionInArea();

        if (IsPositionInRestrictedTilemap(randomPosition))
        {    
            return; 
        }
        GameObject randomObject = objectToSpawn[Random.Range(0, objectToSpawn.Length)];
        GameObject spawnedObject = Instantiate(randomObject, randomPosition, Quaternion.identity);
        spawnedObject.transform.SetParent(ENV.transform);
    }
    

    private Vector3 GetRandomPositionInArea()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnAreaRadius;
        return new Vector3(spawnAreaCenter.position.x + randomCircle.x, spawnAreaCenter.position.y + randomCircle.y, 0);
    }

    // Fungsi untuk mengecek apakah posisi berada di salah satu Tilemap yang dibatasi
    private bool IsPositionInRestrictedTilemap(Vector3 position)
    {
        foreach (Tilemap tilemap in restrictedTilemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position); // Mengonversi posisi dunia ke cell Tilemap
            if (tilemap.HasTile(cellPosition)) // Jika cell memiliki tile, return true
            {
                return true;
            }
        }
        return false; // Tidak ditemukan tile di semua Tilemap yang dibatasi
    }

    // Fungsi untuk menghancurkan objek paling lama jika limit tercapai
    private void DestroyOldestChild()
    {
        if (ENV.transform.childCount > 0)
        {
            Transform oldestChild = ENV.transform.GetChild(0);
            Destroy(oldestChild.gameObject); // Menghancurkan objek yang paling lama
        }
    }
}
