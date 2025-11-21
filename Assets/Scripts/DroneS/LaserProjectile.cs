using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LaserProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifetime = 5f;
    public bool isDeflected = false;
    private Rigidbody rb;
    public GameObject originDrone;
    public GameObject destroyParticlePrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isDeflected)
            rb.linearVelocity = rb.linearVelocity;
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
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (destroyParticlePrefab != null)
        {
            Instantiate(destroyParticlePrefab, transform.position, transform.rotation);
        }
    }
}
