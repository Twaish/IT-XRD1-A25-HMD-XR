using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject enemyPrefab;
    public int initialSpawnLimit = 5;
    public Vector3 spawnArea = new Vector3(20, 1, 20);
    public Transform player;
    [Header("Orientation")]
    public SpawnOrientation spawnOrientation = SpawnOrientation.FacePlayer;

    [Header("Timing")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 3f;
    public float fixedSpawnDelay = 5f;

    [Header("Progression")]
    public float increaseInterval = 10f; 
    public int increaseAmount = 1;


    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentSpawnLimit;

    void Start()
    {
        currentSpawnLimit = initialSpawnLimit;
        StartCoroutine(SpawnEnemiesContinuously());
        StartCoroutine(IncreaseSpawnLimitOverTime());
    }

    private IEnumerator SpawnEnemiesContinuously()
    {
        while (true)
        {
            activeEnemies.RemoveAll(d => d == null);

            if (activeEnemies.Count < currentSpawnLimit)
            {
                Vector3 pos = transform.position + new Vector3(
                    Random.Range(-spawnArea.x, spawnArea.x),
                    spawnArea.y,
                    Random.Range(-spawnArea.z, spawnArea.z)
                );

                // —— Determine rotation based on spawnOrientation ——
                //Quaternion rotation = enemyPrefab.transform.rotation;
                Quaternion rotation = Quaternion.identity;

                if (spawnOrientation == SpawnOrientation.FacePlayer && player != null)
                {
                    // Rotate to face player (only around Y axis, assuming ground-based movement)
                    Vector3 directionToPlayer = (player.position - pos).normalized;
                    directionToPlayer.y = 0; // ignore height for horizontal facing
                    if (directionToPlayer != Vector3.zero)
                        rotation = Quaternion.LookRotation(directionToPlayer);
                }
                else if (spawnOrientation == SpawnOrientation.RandomDirection)
                {
                    // Random yaw (Y-axis) rotation
                    float randomY = Random.Range(0f, 360f);
                    rotation = Quaternion.Euler(0f, randomY, 0f);
                }

                GameObject enemy = Instantiate(enemyPrefab, pos, rotation);
                enemy.GetComponent<Enemies>().player = player;
                activeEnemies.Add(enemy);
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

public enum SpawnOrientation
{
    PrefabDefault,
    FacePlayer,
    RandomDirection
}