using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField]
    private int health = 100;

    [SerializeField]
    private int minHealth = 0;
    [SerializeField]
    private int maxHealth = 100;

    public event Action<int> OnHealthChanged;

    private void Start()
    {
        OnHealthChanged?.Invoke(health);
    }

    public void Damage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, minHealth, maxHealth);
        OnHealthChanged?.Invoke(health);
    }

    public int MaxHealth => maxHealth;
}
