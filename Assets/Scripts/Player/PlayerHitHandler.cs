using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHitHandler : MonoBehaviour
{
    public int hitsToPassthrough = 5;
    private int currentHits = 0;

    public InputAction resetAction;

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

    void OnTriggerEnter(Collider other)
    {
        RegisterHit();
    }

    void RegisterHit()
    {
        currentHits++;

        if (currentHits >= hitsToPassthrough)
            EnableRealityCheck();
    }

    void EnableRealityCheck()
    {
        SceneManager.LoadScene("Insane Asylum");
    }

    void OnResetPressed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
