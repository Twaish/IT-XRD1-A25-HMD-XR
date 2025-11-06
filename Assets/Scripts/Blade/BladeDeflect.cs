using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BladeDeflect : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public float deflectForce = 30f;
    public float bigBoxRandomAngle = 20f;

    public event Action OnDeflect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<LaserProjectile>(out var laser))
            return;

        if (!other.TryGetComponent<Rigidbody>(out var rb))
            return;

        Vector3 deflectDir = Vector3.zero;

        int layer = gameObject.layer;

        if (layer == LayerMask.NameToLayer("DeflectSmall"))
        {
            GameObject enemy = laser.originDrone;
            if (enemy != null)
                deflectDir = (enemy.transform.position - transform.position).normalized;
            else
                deflectDir = transform.forward;
            laser.isDeflected = true;
        }
        else if (layer == LayerMask.NameToLayer("DeflectBig"))
        {
            Vector3 awayFromPlayer = (other.transform.position - player.position).normalized;
            deflectDir = Quaternion.Euler(
                Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                0f
            ) * awayFromPlayer;
            laser.isDeflected = true;
        }

        rb.linearVelocity = deflectDir * deflectForce;
        laser.transform.forward = deflectDir;
        OnDeflect?.Invoke();
    }
}
