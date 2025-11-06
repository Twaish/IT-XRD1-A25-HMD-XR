using TMPro;
using UnityEngine;

public class PlayerHealthUIController : MonoBehaviour
{
    [SerializeField]
    private HealthSystem healthSystem;

    [SerializeField]
    private TextMeshProUGUI healthText;

    private void Start()
    {
        healthSystem.OnHealthChanged += UpdateHealth;
    }
    
    private void UpdateHealth(int health)
    {
        int healthPercentage = Mathf.RoundToInt((float)health / healthSystem.MaxHealth * 100);
        healthText.text = healthPercentage + "%";
    }
}
