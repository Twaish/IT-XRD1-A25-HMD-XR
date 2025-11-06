using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float speed = 2f; // units per second
    public float distanceToStop = 2f;
    public float detectionRange = 10f;
    private Transform player;
    float distanceToPlayer = 0f;

    void Start()
    {
        // Assuming the VR camera or XR Rig camera is tagged "MainCamera"
        player = Camera.main.transform;
    }

    void Update()
    {
        if (player == null) return;
        Vector3 distance = player.position - transform.position;
        distanceToPlayer = distance.magnitude;
        if (distanceToPlayer <= detectionRange && distanceToPlayer > distanceToStop)
        {
            // Move towards the player's position
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
