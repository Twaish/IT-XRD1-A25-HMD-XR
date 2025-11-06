using UnityEngine;

public class BladeDeflect : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public float deflectForce = 30f;
    public float bigBoxRandomAngle = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Laser"))
            return;

        LaserProjectile laser = other.GetComponent<LaserProjectile>();
        if (laser == null)
            return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        Vector3 deflectDir = Vector3.zero;

        int layer = gameObject.layer;
        int smallLayer = LayerMask.NameToLayer("DeflectSmall");
        int bigLayer = LayerMask.NameToLayer("DeflectBig");

        if (layer == LayerMask.NameToLayer("DeflectSmall"))
        {
            // Perfect deflection directly back to enemy
            GameObject enemy = GameObject.FindWithTag("Enemy");
            if (enemy != null)
                deflectDir = (enemy.transform.position - transform.position).normalized;
            else
                deflectDir = transform.forward;
        }
        else if (layer == LayerMask.NameToLayer("DeflectBig"))
        {
            // Imperfect deflection, slightly randomized
            Vector3 awayFromPlayer = (other.transform.position - player.position).normalized;
            deflectDir = Quaternion.Euler(
                Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                Random.Range(-bigBoxRandomAngle, bigBoxRandomAngle),
                0f
            ) * awayFromPlayer;
        }

        rb.linearVelocity = deflectDir * deflectForce;
        laser.enabled = false; // stop forward motion code
        laser.transform.forward = deflectDir;
    }
}
