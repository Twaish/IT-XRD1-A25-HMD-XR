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
    private Transform player;
    private float timeSinceLastSwing = 0f;

    void Start()
    {
        player = Camera.main.transform;
        if (stickHitbox != null) stickHitbox.enabled = false;
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
                // Advance
                transform.position += dirToPlayer.normalized * speed * Time.deltaTime;
                timeSinceLastSwing = 0; // reset
            }
            else if (distanceToPlayer < minSafe && !isCommittedToSwing)
            {
                // Retreat
                transform.position -= dirToPlayer.normalized * speed * Time.deltaTime * 0.8f;
                timeSinceLastSwing = 0; // reset
            }
            else
            {
                // In sweet spot → count up
                timeSinceLastSwing += Time.deltaTime;

                // Swing when ready
                if (timeSinceLastSwing >= swingCooldown && stick != null && !isCommittedToSwing)
                {
                    timeSinceLastSwing = 0;
                    StartCoroutine(SwingOnce());
                }
            }
        }
    }

    IEnumerator SwingOnce()
    {
        if (stick == null) yield break;

        isCommittedToSwing = true;

        // Wind-up
        Quaternion ready = Quaternion.identity;
        Quaternion windup = Quaternion.Euler(-10, 0, 0);
        float t = 0;
        while (t < 0.15f) { stick.localRotation = Quaternion.Slerp(ready, windup, t / 0.15f); t += Time.deltaTime; yield return null; }
        stick.localRotation = windup;

        // Swing
        Quaternion hit = Quaternion.Euler(60, 0, 0);
        t = 0;
        while (t < swingDownTime) { stick.localRotation = Quaternion.Slerp(windup, hit, t / swingDownTime); t += Time.deltaTime; yield return null; }
        stick.localRotation = hit;

        if (stickHitbox != null) stickHitbox.enabled = true;
        yield return new WaitForSeconds(pauseAfterHit);
        if (stickHitbox != null) stickHitbox.enabled = false;

        // Recovery
        isCommittedToSwing = false;

        Quaternion recover = Quaternion.Euler(0, 0, 0);
        t = 0;
        while (t < swingUpTime) { stick.localRotation = Quaternion.Slerp(hit, recover, t / swingUpTime); t += Time.deltaTime; yield return null; }
        stick.localRotation = recover;
    }

    // Keep StopSwinging for death
    public void StopSwinging()
    {
        StopAllCoroutines();
        isCommittedToSwing = false;
        timeSinceLastSwing = 0;
        if (stickHitbox != null) stickHitbox.enabled = false;
        if (stick != null) stick.localRotation = Quaternion.identity;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword")) Debug.Log("Blocked!");
    }
}