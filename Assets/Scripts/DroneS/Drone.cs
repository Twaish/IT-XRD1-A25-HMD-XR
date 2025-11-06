using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("Movement")]
    public float orbitRadius = 10f;
    public float orbitSpeed = 3f;
    public float moveSmooth = 2f;
    public float hoverAmplitude = 0.5f;
    public float hoverSpeed = 2f;
    public float minHeight = 1.5f;   
    public float maxHeight = 5f;

    [Header("Separation / Avoidance")]
    public float separationDistance = 3f;
    public float separationStrength = 2f;
    public LayerMask droneMask;

    [Header("Shooting")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float detectionRange = 30f;

    [Header("References")]
    public Transform player;

    private float fireTimer;
    private float hoverOffset;
    private Vector3 orbitTarget;
    private Vector3 previousAvoidance;

    void Start()
    {
        if (!player)
        {
            Debug.LogWarning("EnemyDrone: Player not assigned!");
            enabled = false;
            return;
        }

        hoverOffset = Random.Range(0f, Mathf.PI * 2f);

        Vector3 offset = Random.onUnitSphere * orbitRadius;
        offset.y = Mathf.Clamp(offset.y, minHeight, maxHeight);
        orbitTarget = player.position + offset;

        fireTimer = fireRate;
        gameObject.layer = LayerMask.NameToLayer("Drone");
    }

    void Update()
    {
        if (!player) return;

        fireTimer -= Time.deltaTime;

        Vector3 toDrone = transform.position - player.position;
        Vector3 tangent = Vector3.Cross(Vector3.up, toDrone).normalized;
        orbitTarget = player.position + toDrone.normalized * orbitRadius + tangent * Time.deltaTime * orbitSpeed;

        Vector3 desiredPosition = Vector3.Lerp(transform.position, orbitTarget, moveSmooth * Time.deltaTime);

        desiredPosition.y += Mathf.Sin(Time.time * hoverSpeed + hoverOffset) * hoverAmplitude * Time.deltaTime;

        Vector3 avoidance = ComputeSeparationForce();
        avoidance = Vector3.Lerp(previousAvoidance, avoidance, Time.deltaTime * 5f);
        previousAvoidance = avoidance;
        desiredPosition += avoidance * Time.deltaTime;

        desiredPosition = ClampHeight(desiredPosition);

        transform.position = desiredPosition;

        Vector3 lookDir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) < detectionRange && fireTimer <= 0f)
        {
            FireLaser();
            fireTimer = fireRate;
        }
    }

    void FireLaser()
    {
        if (!laserPrefab || !firePoint) return;
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
    }

    Vector3 ComputeSeparationForce()
    {
        Vector3 force = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, separationDistance, droneMask);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;
            Vector3 away = transform.position - neighbor.transform.position;
            float distance = away.magnitude;
            if (distance > 0f)
                force += away.normalized / distance;
        }

        return force * separationStrength;
    }

    Vector3 ClampHeight(Vector3 position)
    {
        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

        return position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationDistance);
    }
}
