using UnityEngine;
using System.Collections;

public class Enemies : MonoBehaviour
{
    public float speed = 2f;
    public float distanceToStop = 2f;
    public float detectionRange = 10f;

    public Transform stick;
    public Collider stickHitbox;
    public float swingCooldown = 2f;
    public float swingDownTime = 0.25f;
    public float swingUpTime = 0.25f;
    public float pauseAfterHit = 0.2f;
    
    private bool isCommittedToSwing = false;
    public Transform player;
    private float timeSinceLastSwing = 0f;
    private Quaternion originalStickRotation;

    // 🔑 NEW: State & motion control
    private bool isStunned = false;
    private Coroutine currentMotion;

    void Start()
    {
        /*player = Camera.main.transform;
        if (stick != null)
            originalStickRotation = stick.localRotation;
        if (stickHitbox != null)
            stickHitbox.enabled = false;*/

        if (!player)
        {
            Debug.LogWarning("EnemyDrone: Player not assigned!");
            enabled = false;
            return;
        }

    }

    void Update()
    {
        if (player == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Rotate to face player
            Vector3 dirToPlayer = player.position - transform.position;
            dirToPlayer.y = 0;
            if (dirToPlayer.magnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 5f);
            }

            float minSafe = distanceToStop * 0.7f;

            if (distanceToPlayer > distanceToStop)
            {
                transform.position += dirToPlayer.normalized * speed * Time.deltaTime;
                timeSinceLastSwing = swingCooldown / 3;
            }
            else if (distanceToPlayer < minSafe && !isCommittedToSwing)
            {
                transform.position -= dirToPlayer.normalized * speed * Time.deltaTime * 0.8f;
                timeSinceLastSwing = swingCooldown / 3;
            }
            else if (!isStunned) // 🔑 Only allow swing counting if NOT stunned
            {
                timeSinceLastSwing += Time.deltaTime; // always increment

                // But only allow swing if NOT mid-swing AND timer is ready
                if (timeSinceLastSwing >= swingCooldown && !isCommittedToSwing && stick != null)
                {
                    timeSinceLastSwing = 0;
                    StartCoroutine(SwingOnce());
                }
            }
        }
    }

    // 🔁 Unified safe rotation helper
    private IEnumerator SmoothRotateTo(Quaternion target, float duration)
    {
        if (stick == null) yield break;
        Quaternion start = stick.localRotation;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            stick.localRotation = Quaternion.Slerp(start, target, t / duration);
            yield return null;
        }
        stick.localRotation = target;
    }

    IEnumerator SwingOnce()
    {
        if (stick == null || isStunned) yield break; // 🔑 Early exit if stunned

        // 🔑 Cancel any ongoing motion (including prior stun recovery)
        if (currentMotion != null) StopCoroutine(currentMotion);

        isCommittedToSwing = true;

        // Wind-up
        Quaternion windup = Quaternion.Euler(-10, 0, 0);
        yield return SmoothRotateTo(windup, 0.15f);

        // Swing down
        Quaternion hit = Quaternion.Euler(60, 0, 0);
        yield return SmoothRotateTo(hit, swingDownTime);

        // Hit phase
        if (stickHitbox != null) stickHitbox.enabled = true;
        yield return new WaitForSeconds(pauseAfterHit);
        if (stickHitbox != null) stickHitbox.enabled = false;

        // Recovery — only if not stunned *during* hit pause
        isCommittedToSwing = false;
        if (!isStunned)
        {
            yield return SmoothRotateTo(originalStickRotation, swingUpTime);
        }
        // If stunned during pauseAfterHit, ApplyStun() has already taken over
    }

    [SerializeField] private float swordRecoveryTime = 0.3f;

    public void ApplyStun(float stunDuration = 0.5f)
    {
        Debug.Log("Stunned");
        // 🔑 1. Block attacks for `stunDuration` seconds
        timeSinceLastSwing = -stunDuration; // will take `stunDuration` seconds to reach 0

        // 🔑 2. Cancel any active swing (but keep hitbox off)
        isCommittedToSwing = false;
        if (stickHitbox != null) stickHitbox.enabled = false;

        // 🔑 3. Visually recover sword — in FIXED time (e.g. 0.3s), unrelated to stunDuration
        if (stick != null)
        {
            if (currentMotion != null) StopCoroutine(currentMotion);
            currentMotion = StartCoroutine(SmoothStickRecovery(swordRecoveryTime));
        }
    }

    private IEnumerator SmoothStickRecovery(float duration)
    {
        if (stick == null) yield break;
        Quaternion start = stick.localRotation;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            stick.localRotation = Quaternion.Slerp(start, originalStickRotation, t / duration);
            yield return null;
        }
        stick.localRotation = originalStickRotation;
        currentMotion = null;
    }

    // Keep StopSwinging for death/cleanup
    public void StopSwinging()
    {
        StopAllCoroutines(); // okay here — total reset
        isCommittedToSwing = false;
        isStunned = false;
        timeSinceLastSwing = 0;
        currentMotion = null;
        if (stickHitbox != null) stickHitbox.enabled = false;
        if (stick != null) stick.localRotation = originalStickRotation;
    }

    // Optional: cleanup if object is disabled
    void OnDisable()
    {
        StopAllCoroutines();
        currentMotion = null;
    }
}