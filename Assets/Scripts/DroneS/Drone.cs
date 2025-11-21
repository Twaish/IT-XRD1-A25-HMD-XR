using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public string droneTag = "Drone";

    [Header("Shooting")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float fireDelay = 2f;
    public float detectionRange = 30f;

    [Header("References")]
    public Transform player;

    private float fireTimer;
    private float hoverOffset;
    private Vector3 orbitTarget;
    private Vector3 previousAvoidance;

    [Header("Targeting / Laser Sight")]
    public GameObject laserSightPrefab;
    public float aimDuration = 1.5f;
    public float laserOvershoot = 5f;
    private LineRenderer currentSight;

    public event Action OnDeath;
    public event Action OnFire;

    private bool isAiming = false;
    private Vector3 lockedBeamEnd;
    private Vector3 lockedPlayerPosition;

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

        fireTimer = fireDelay;
    }

    void Update()
    {
        if (!player)
            return;

        fireTimer -= Time.deltaTime;

        Vector3 toDrone = transform.position - player.position;
        Vector3 tangent = Vector3.Cross(Vector3.up, toDrone).normalized;
        orbitTarget =
            player.position
            + toDrone.normalized * orbitRadius
            + tangent * Time.deltaTime * orbitSpeed;

        Vector3 desiredPosition = Vector3.Lerp(
            transform.position,
            orbitTarget,
            moveSmooth * Time.deltaTime
        );

        desiredPosition.y +=
            Mathf.Sin(Time.time * hoverSpeed + hoverOffset) * hoverAmplitude * Time.deltaTime;

        Vector3 avoidance = ComputeSeparationForce();
        avoidance = Vector3.Lerp(previousAvoidance, avoidance, Time.deltaTime * 5f);
        previousAvoidance = avoidance;
        desiredPosition += avoidance * Time.deltaTime;

        desiredPosition = ClampHeight(desiredPosition);

        transform.position = desiredPosition;

        if (!isAiming)
        {
            Vector3 lookDir = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                5f * Time.deltaTime
            );
        }

        if (
            Vector3.Distance(transform.position, player.position) < detectionRange
            && fireTimer <= 0f
        )
        {
            StartCoroutine(AimAndFire());
            fireTimer = fireDelay + aimDuration;
        }
    }

    private IEnumerator AimAndFire()
    {
        isAiming = true;

        // Create laser sight if needed
        if (laserSightPrefab && !currentSight)
        {
            GameObject sightObj = Instantiate(
                laserSightPrefab,
                firePoint.position,
                Quaternion.identity
            );

            currentSight = sightObj.GetComponent<LineRenderer>();
            currentSight.useWorldSpace = true;

            sightObj.transform.SetParent(null); //unparent the laser
        }

        Quaternion originalRotation = transform.rotation;
        Quaternion flippedRotation = originalRotation * Quaternion.Euler(0, 180f, 0);
        transform.rotation = flippedRotation;

        // *** LOCK THE PLAYER'S POSITION AT START OF AIM ***
        lockedPlayerPosition = player.position + Vector3.up * -0.2f;

        float timer = 0f;

        while (timer < aimDuration)
        {
            timer += Time.deltaTime;

            if (currentSight)
            {
                // Start point moves with drone
                currentSight.SetPosition(0, firePoint.position);

                // Recompute direction based on MOVING firePoint → LOCKED player position
                Vector3 dynamicDir = (lockedPlayerPosition - firePoint.position).normalized;

                // End point always lies PAST the locked player position
                currentSight.SetPosition(1, lockedPlayerPosition + dynamicDir * laserOvershoot);
            }

            yield return null;
        }

        if (currentSight)
        {
            Destroy(currentSight.gameObject);
            currentSight = null;
        }

        transform.rotation = originalRotation;
        isAiming = false;

        FireLaser();
    }

    void FireLaser()
    {
        if (!laserPrefab || !firePoint)
            return;

        // Compute direction FROM firePoint to locked END POINT
        Vector3 dir = (lockedPlayerPosition - firePoint.position).normalized;

        Quaternion shotRotation = Quaternion.LookRotation(dir);

        GameObject laser = Instantiate(laserPrefab, firePoint.position, shotRotation);

        LaserProjectile laserScript = laser.GetComponent<LaserProjectile>();
        laserScript.originDrone = gameObject;

        OnFire?.Invoke();
    }

    Vector3 ComputeSeparationForce()
    {
        Vector3 force = Vector3.zero;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, separationDistance);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject)
                continue;

            if (!neighbor.CompareTag(droneTag))
                continue;

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

    public void Die()
    {
        StopAllCoroutines();

        if (currentSight)
        {
            Destroy(currentSight.gameObject);
            currentSight = null;
        }

        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
