using UnityEngine;

public class Drone : MonoBehaviour
{
    [Header("Movement")]
    public float orbitRadius = 10f;       
    public float orbitSpeed = 3f;         
    public float moveSmooth = 2f;         
    public float hoverAmplitude = 0.5f;   
    public float hoverSpeed = 2f;         

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

    void Start()
    {
        if (!player)
        {
            Debug.Log("EnemyDrone: Player not assigned!");
            enabled = false;
            return;
        }

        hoverOffset = Random.Range(0f, Mathf.PI * 2f);

        Vector3 offset = Random.onUnitSphere * orbitRadius;
        offset.y = Mathf.Clamp(offset.y, 1f, 3f);
        orbitTarget = player.position + offset;

        fireTimer = fireRate;
    }

    void Update()
    {
        if (!player) return;
        Vector3 offset = Random.onUnitSphere * orbitRadius;
        offset.y = Mathf.Clamp(offset.y, 1f, 3f);
        orbitTarget = player.position + offset;

        fireTimer -= Time.deltaTime;

        Vector3 toDrone = transform.position - player.position;
        Vector3 tangent = Vector3.Cross(Vector3.up, toDrone).normalized;
        orbitTarget = player.position + toDrone.normalized * orbitRadius + tangent * Time.deltaTime * orbitSpeed;

        Vector3 desiredPosition = Vector3.Lerp(transform.position, orbitTarget, moveSmooth * Time.deltaTime);

        desiredPosition.y += Mathf.Sin(Time.time * hoverSpeed + hoverOffset) * hoverAmplitude * Time.deltaTime;

        transform.position = desiredPosition;

        Vector3 lookDir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) < detectionRange && fireTimer <= 0f)
        {
            Debug.Log("fire" + fireTimer + " " + fireRate);
            FireLaser();
            fireTimer = fireRate;
        }
    }

    void FireLaser()
    {
        if (!laserPrefab || !firePoint) return;
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
    }
}
