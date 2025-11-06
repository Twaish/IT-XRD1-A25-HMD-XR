using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaserProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifetime = 5f;
    public bool isDeflected = false;
    private Rigidbody rb;
    public GameObject originDrone;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isDeflected)
            transform.position += speed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drone") && isDeflected)
        {
            Debug.Log("Killed drone with deflected laser");
            Drone drone = originDrone.GetComponent<Drone>();
            drone.Die();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by laser!");
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Drone") && !other.CompareTag("Saber"))
        {
            Debug.Log(other.gameObject);
            Destroy(gameObject);
        }
    }
}
