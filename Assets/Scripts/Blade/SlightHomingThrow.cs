using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlightHomingThrow : MonoBehaviour
{
    [Header("Targeting Settings")]
    [Tooltip("Tag of enemy objects to slightly home toward.")]
    public string enemyTag = "Drone";

    [Tooltip("How much the object is influenced by the target (0 = ignore, 1 = fully toward target).")]
    [Range(0f, 1f)]
    public float homingStrength = 0.05f;

    [Tooltip("Maximum distance to search for targets.")]
    public float targetSearchRadius = 10f;

    [Header("Throw Settings")]
    [Tooltip("Time in seconds during which homing is active after throw.")]
    public float homingDuration = 0.5f;

    [Tooltip("Rotate object to face movement direction.")]
    public bool rotateTowardsVelocity = true;

    [Header("Flight Effects")]
    [Tooltip("How fast the projectile spins while flying.")]
    public float spinSpeed = 720f;

    [Tooltip("Extra acceleration toward the target or flight direction.")]
    public float acceleration = 5f;

    private Rigidbody rb;
    private Transform target;
    private float homingTimer = 0f;
    private bool homingActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!homingActive)
            return;

        homingTimer -= Time.fixedDeltaTime;
        if (homingTimer <= 0f)
        {
            homingActive = false;
            target = null;
            return;
        }

        if (target == null)
            FindNearestTarget();

        if (rb.linearVelocity.magnitude > 0.01f)
        {
            Vector3 newDir = rb.linearVelocity.normalized;

            if (target != null)
            {
                Vector3 toTarget = (target.position - transform.position).normalized;
                newDir = Vector3.Lerp(rb.linearVelocity.normalized, toTarget, homingStrength).normalized;
            }

            rb.AddForce(newDir * acceleration, ForceMode.Acceleration);

            rb.linearVelocity = newDir * rb.linearVelocity.magnitude;

            if (rotateTowardsVelocity)
                //transform.rotation = Quaternion.LookRotation(rb.linearVelocity);

            transform.Rotate(Vector3.forward, spinSpeed * Time.fixedDeltaTime, Space.Self);
        }
    }

    public void ActivateHoming()
    {
        homingActive = true;
        homingTimer = homingDuration;
    }

    private void FindNearestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, targetSearchRadius);
        float closestDist = float.MaxValue;
        Transform closest = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hit.transform;
                }
            }
        }

        target = closest;
    }
}
