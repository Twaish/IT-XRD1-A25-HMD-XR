using UnityEngine;

public class Stick : MonoBehaviour
{
    public float stunDuration = 8f;

    void Start()
    {
        // Ensure we have a Collider (non-trigger)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = false; // ← Critical: NOT a trigger

        // Ensure we have a Rigidbody (non-kinematic, unless other side has dynamic RB)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false; // ← Must be false for collision *if the other object is kinematic*
            rb.useGravity = false;
            rb.mass = 1f;
        }
        // Optional: freeze rotation/position if it's just a sword swing detector
        rb.constraints = RigidbodyConstraints.FreezeAll;

        Debug.Log("Stick setup: Collider (non-trigger) + Rigidbody added", this);
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider other = collision.collider;
        Debug.Log("COLLISION DETECTED! Hit: " + other.name + " with tag: " + other.tag, this);

        // Check tag + name condition (add parentheses for correct logic!)
        if (other.CompareTag("Saber") || other.CompareTag("Stun"))
        {
            Debug.Log("Saber/Stun hit detected — applying stun!", this);

            // Find enemy (parent or self)
            Enemies enemy = GetComponentInParent<Enemies>() ?? GetComponent<Enemies>();
            if (enemy != null)
            {
                enemy.ApplyStun(stunDuration);
                Debug.Log("Enemy stunned via physical collision with player saber.", this);
            }
            else
            {
                Debug.LogWarning("No Enemies component found in hierarchy!", this);
            }
        }
    }
}