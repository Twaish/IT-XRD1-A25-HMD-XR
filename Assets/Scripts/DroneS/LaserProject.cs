using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float speed = 25f;
    public float lifetime = 5f;
    public bool isDeflected = false;
    private Rigidbody rb;

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
            transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void Deflect(Vector3 newDirection, float newSpeed)
    {
        isDeflected = true;
        rb.linearVelocity = newDirection * newSpeed;
        transform.forward = newDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by laser!");
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Saber"))
        {
            Destroy(gameObject);
        }
    }
}
