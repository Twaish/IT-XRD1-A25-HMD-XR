using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerHitHandler))]
[RequireComponent(typeof(HealthSystem))]
public class PlayerDamageReceiver : MonoBehaviour
{
    private HealthSystem healthSystem;
    private PlayerHitHandler hitHandler;
    
    private int previousSceneIndex;

    public InputAction resetAction;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        hitHandler = GetComponent<PlayerHitHandler>();

        hitHandler.OnHit += HandleHit;
        healthSystem.OnHealthChanged += HandleHealthChanged;
    }
    
    void OnEnable()
    {
        resetAction.Enable();
        resetAction.performed += OnResetPressed;
    }

    void OnDisable()
    {
        resetAction.performed -= OnResetPressed;
        resetAction.Disable();
    }

    private void HandleHit()
    {
        healthSystem.Damage(20);
    }

    private void HandleHealthChanged(int health)
    {
        if (health <= 0)
        {
            EnableRealityCheck();
        }
    }

    private void EnableRealityCheck()
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Insane Asylum");
    }

    private void OnResetPressed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Drones", LoadSceneMode.Single);
    }
}
