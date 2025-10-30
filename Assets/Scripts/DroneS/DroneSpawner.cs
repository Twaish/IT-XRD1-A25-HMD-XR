using UnityEngine;
using System.Collections;

public class DroneSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject dronePrefab;
    public int numberToSpawn = 5;
    public Vector3 spawnArea = new Vector3(20, 5, 20);
    public Transform player;

    [Header("Timing")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 3f;
    public float fixedSpawnDelay = 5f;

    void Start()
    {
        StartCoroutine(SpawnDronesWithDelay());
    }

    private IEnumerator SpawnDronesWithDelay()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(1, spawnArea.y),
                Random.Range(-spawnArea.z, spawnArea.z)
            );

            var drone = Instantiate(dronePrefab, pos, Quaternion.identity);
            drone.GetComponent<Drone>().player = player;

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay) + fixedSpawnDelay;
            yield return new WaitForSeconds(delay);
        }
    }
}
