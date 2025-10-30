using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    public GameObject dronePrefab;
    public int numberToSpawn = 5;
    public Vector3 spawnArea = new Vector3(20, 5, 20);
    public Transform player;

    void Start()
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
        }
    }
}
