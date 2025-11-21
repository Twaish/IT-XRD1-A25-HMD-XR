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
    
    public Transform player;
    private float timeSinceLastSwing = 0f;
    private Quaternion originalStickRotation;

    private bool isCommittedToSwing = false;
    private bool isStunned = false;
    private Coroutine currentMotion;

    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent missing on enemy!");
            enabled = false;
            return;
        }

        // Configure agent at runtime
        agent.speed = speed;
        agent.stoppingDistance = distanceToStop;
        agent.updateRotation = false; // We handle rotation manually for smoother look-at

        if (stick != null)
            originalStickRotation = stick.localRotation;

        if (stickHitbox != null)
            stickHitbox.enabled = false;

        if (player == null)
        {
            Debug.LogWarning("Enemy: Player not assigned!");
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
            if (!isStunned)
            {
                agent.SetDestination(player.position);
            }

            // Rotate to face player, unless it is moving around other objects or such.
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            dirToPlayer.y = 0;
            if (dirToPlayer.magnitude > 0.01f)
            {
                Vector3 lookDir = agent.velocity.magnitude > 0.1f ? agent.velocity : dirToPlayer;
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
            }

            float minSafe = distanceToStop * 0.7f;
            if (distanceToPlayer > distanceToStop)
            {
                //transform.position += dirToPlayer.normalized * speed * Time.deltaTime;
                timeSinceLastSwing = Mathf.Max(0, swingCooldown / 3);
            }
            else if (distanceToPlayer < minSafe && !isCommittedToSwing)
            {
                transform.position -= dirToPlayer.normalized * speed * Time.deltaTime * 0.8f;
                timeSinceLastSwing = swingCooldown / 3;
            }
            else if (!isStunned) 
            {
                timeSinceLastSwing += Time.deltaTime; 

                if (timeSinceLastSwing >= swingCooldown && !isCommittedToSwing && stick != null)
                {
                    timeSinceLastSwing = 0;
                    StartCoroutine(SwingOnce());
                }
            }
        }
        else
        {
            agent.ResetPath();
        }
    }

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
        if (stick == null || isStunned) yield break;

        if (currentMotion != null) StopCoroutine(currentMotion);
        isCommittedToSwing = true;

        // Wind-up
        Quaternion windup = Quaternion.Euler(-10, 0, 0);
        yield return SmoothRotateTo(windup, 0.15f);

        // Swing down
        Quaternion hit = Quaternion.Euler(72, 0, 10);
        yield return SmoothRotateTo(hit, swingDownTime);

        // Hit phase
        if (stickHitbox != null) stickHitbox.enabled = true;
        yield return new WaitForSeconds(pauseAfterHit);
        if (stickHitbox != null) stickHitbox.enabled = false;

        // Recovery — only if not stunned *during* hit pause
        if (currentMotion != null) StopCoroutine(currentMotion);
            currentMotion = StartCoroutine(SmoothStickRecovery(swordRecoveryTime));
        
        isCommittedToSwing = false;
        if (!isStunned)
        {
            yield return SmoothRotateTo(originalStickRotation, swingUpTime);
        }
    }

    [SerializeField] private float swordRecoveryTime = 0.3f;

    public void ApplyStun(float stunDuration = 0.5f)
    {
        Debug.Log("Stunned");
        timeSinceLastSwing = -stunDuration;

        isCommittedToSwing = false;
        if (stickHitbox != null) stickHitbox.enabled = false;

        if (stick != null)
        {
            if (currentMotion != null) StopCoroutine(currentMotion);
            currentMotion = StartCoroutine(SmoothStickRecovery(swordRecoveryTime));
        }

        StartCoroutine(ResumeAfterStun(stunDuration));
    }

    IEnumerator ResumeAfterStun(float duration)
    {
        yield return new WaitForSeconds(duration);
        isStunned = false;
        if (agent != null) agent.isStopped = false;
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

    public void StopSwinging()
    {
        StopAllCoroutines();
        isCommittedToSwing = false;
        isStunned = false;
        timeSinceLastSwing = 0;
        currentMotion = null;
        if (stickHitbox != null) stickHitbox.enabled = false;
        if (stick != null) stick.localRotation = originalStickRotation;

        // Also stop NavMesh movement
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    /*
    // Optional: cleanup if object is disabled
    void OnDisable()
    {
        StopAllCoroutines();
        currentMotion = null;
        if (agent != null) agent.ResetPath();
    } */
}