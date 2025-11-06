using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BladeDeflect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private float deflectForce = 30f;
    [SerializeField] private float bigBoxRandomAngle = 20f;
    [SerializeField] private DeflectScheme deflectScheme = DeflectScheme.Return;

    public enum DeflectScheme
    {
        Random,
        Return
    }

    public event Action OnDeflect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<LaserProjectile>(out var laser))
            return;

        if (!other.TryGetComponent<Rigidbody>(out var rb))
            return;

        Vector3 deflectDir = Vector3.zero;
        laser.isDeflected = true;

        switch (deflectScheme)
        {
            case DeflectScheme.Return:
                GameObject enemy = laser.originDrone;
                if (enemy != null)
                    deflectDir = (enemy.transform.position - transform.position).normalized;
                else
                    deflectDir = transform.forward;
                break;
            case DeflectScheme.Random:
                Vector3 awayFromPlayer = (other.transform.position - player.position).normalized;
                deflectDir = Quaternion.Euler(
                    Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                    Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                    0f
                ) * awayFromPlayer;
                break;
        }

        rb.linearVelocity = deflectDir * deflectForce;
        laser.transform.forward = deflectDir;
        OnDeflect?.Invoke();
    }
}
