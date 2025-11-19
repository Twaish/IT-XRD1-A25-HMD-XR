using UnityEngine;

public class Stick : MonoBehaviour
{

    public float stunDuration = 8f;

    void Start()
    {
        // Make sure we have a trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>(); // Add collider if missing
        }
        col.isTrigger = true; // Make sure it's a trigger
        Debug.Log("Stick collider setup complete", this);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLISION DETECTED! Hit: " + other.name + " with tag: " + other.tag, this);

        if (other.CompareTag("Sword"))
        {
            Debug.Log("Sword hit detected — applying stun!", this);

            // Find the enemy (assumed to be parent or same GameObject)
            Enemies enemy = GetComponentInParent<Enemies>();
            if (enemy != null)
            {
                enemy.ApplyStun(stunDuration); // tweak duration as needed
            }
            else
            {
                Debug.LogWarning("No Enemies component found on parent!", this);
            }
        }
    }
}