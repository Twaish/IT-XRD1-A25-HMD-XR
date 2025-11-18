using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject dronePrefab;
    public int initialSpawnLimit = 5;
    public Vector3 spawnArea = new Vector3(20, 5, 20);
    public Transform player;

    [Header("Timing")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 3f;
    public float fixedSpawnDelay = 5f;

    [Header("Progression")]
    public float increaseInterval = 10f; 
    public int increaseAmount = 1;

    private List<GameObject> activeDrones = new List<GameObject>();
    private int currentSpawnLimit;

    void Start()
    {
        currentSpawnLimit = initialSpawnLimit;
        StartCoroutine(SpawnDronesContinuously());
        StartCoroutine(IncreaseSpawnLimitOverTime());
    }

    private IEnumerator SpawnDronesContinuously()
    {
        while (true)
        {
            activeDrones.RemoveAll(d => d == null);

            if (activeDrones.Count < currentSpawnLimit)
            {
                Vector3 pos = transform.position + new Vector3(
                    Random.Range(-spawnArea.x, spawnArea.x),
                    Random.Range(1, spawnArea.y),
                    Random.Range(-spawnArea.z, spawnArea.z)
                );

                GameObject drone = Instantiate(dronePrefab, pos, Quaternion.identity);
                drone.GetComponent<Drone>().player = player;
                activeDrones.Add(drone);
            }

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay) + fixedSpawnDelay;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator IncreaseSpawnLimitOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(increaseInterval);
            currentSpawnLimit += increaseAmount;
            Debug.Log("New spawn limit: " + currentSpawnLimit);
        }
    }
}
