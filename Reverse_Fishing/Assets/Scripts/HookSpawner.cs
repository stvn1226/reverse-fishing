using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Reference to the object you want to spawn
    public List<GameObject> spawnedObjects;
    public Vector2 spawnAreaSize = new Vector2(5f, 5f); // Size of the spawning area
    public float minSpawnInterval; // Minimum time between spawns (in seconds)
    public float maxSpawnInterval; // Maximum time between spawns (in seconds)

    private int globalPower;
    private float nextSpawnTime;

    public int GlobalPower
    {
        get { return globalPower; }
        set { globalPower = value; }
    }

    private void Start()
    {
        globalPower = 100;
        // Set the initial time for the first spawn
        nextSpawnTime = Time.time + 1.0f;
        spawnedObjects = new List<GameObject>();
    }

    private void Update()
    {
        // Check if it's time to spawn a new object
        if (Time.time >= nextSpawnTime)
        {
            //if (spawnedObjects.Count < 12)
            //{
                SpawnObject();
            //}
            SetNextSpawnTime();
        }
    }

    private void SpawnObject()
    {
        // Calculate a random position within the spawn area
        Vector3 spawnPosition = new Vector3(
            transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            transform.position.y + Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            transform.position.z
        );

        GameObject dunder = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        // Instantiate the object at the calculated position
        spawnedObjects.Add(dunder);
    }

    private void SetNextSpawnTime()
    {
        // Set the time for the next spawn within the specified range
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
