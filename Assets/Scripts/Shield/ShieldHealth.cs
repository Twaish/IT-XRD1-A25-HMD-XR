using UnityEngine;

public class ShieldHealth : MonoBehaviour
{
    [Header("Shield Settings")]
    public int maxHealth = 5;
    public float rechargeTime = 5f;

    [Header("Components")]
    public MeshRenderer shieldRenderer;
    public Collider shieldCollider;
    
    private int currentHealth;
    private bool isRecharging = false;
    private Collider[] childColliders; 

    void Start()
    {
        currentHealth = maxHealth;
        if (shieldRenderer == null) shieldRenderer = GetComponent<MeshRenderer>();
        if (shieldCollider == null) shieldCollider = GetComponent<Collider>();
        
        CacheChildColliders();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Shield trigger hit by: " + other.name);

        if (other.CompareTag("Laser"))
        {
            TakeHit(1);
        }
    }

    private void DestroyNearestLaser()
    {
        GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
        if (lasers.Length == 0) return;
        
        GameObject nearest = null;
        float nearestDist = Mathf.Infinity;
        Vector3 shieldPos = transform.position;
        
        foreach (GameObject laser in lasers)
        {
            float dist = Vector3.Distance(shieldPos, laser.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = laser;
            }
        }
        
        if (nearest != null)
        {
            Destroy(nearest);
        }
    }

    public void TakeHit(int damage = 1)
    {
        if (isRecharging) return;
        currentHealth -= damage;
        Debug.LogWarning("Shield took hit. Remaining: " + currentHealth);
        if (currentHealth <= 0)
        {
            BreakShield();
        }
    }

    private void CacheChildColliders()
    {
        childColliders = GetComponentsInChildren<Collider>(true); 
    }

    private void BreakShield()
    {
        Debug.LogWarning("Shield Broke");
        isRecharging = true;
        currentHealth = 0;
        shieldRenderer.enabled = false;
        shieldCollider.enabled = false;
        
        foreach (Collider col in childColliders)
        {
            if (col != shieldCollider)
            {
                col.enabled = false;
            }
        }
        
        Invoke(nameof(RechargeShield), rechargeTime);
    }

    private void RechargeShield()
    {
        currentHealth = maxHealth;
        isRecharging = false;
        shieldRenderer.enabled = true;
        shieldCollider.enabled = true;
        
        foreach (Collider col in childColliders)
        {
            if (col != shieldCollider)
            {
                col.enabled = true;
            }
        }
        
        Debug.Log("Shield Recharged");
    }
}