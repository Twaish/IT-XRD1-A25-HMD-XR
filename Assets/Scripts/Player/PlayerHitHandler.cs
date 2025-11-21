using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class PlayerHitHandler : MonoBehaviour
{
    public int hitsToPassthrough = 5;
    private int currentHits = 0;

    public InputAction resetAction;

    private int previousSceneIndex;

    public event Action OnHit;

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
        if (other.CompareTag("Laser"))
        {
            OnHit?.Invoke();
            RegisterHit();
        }

        if (other.CompareTag("Sword"))
        {
            Debug.Log("I HAVE BEEN HIT AND AM IN A INSUFFRABLE AMOUNT OF PAIN I AM DEFENETLY ABOUT TO GO INSANE I CAN NOT TAKE THIS ANYMORE PLS SAVE ME TAKE ME AWAY BFFORE I EAT MY SABER; AAAAAHHHHHHHRRHRHRHHRHRHRRRRHHHH");
            RegisterHit();
        }
    }

    void RegisterHit()
    {
        currentHits++;

        if (currentHits >= hitsToPassthrough)
            EnableRealityCheck();
    }

    void EnableRealityCheck()
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Insane Asylum");
    }

    void OnResetPressed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("Drones", LoadSceneMode.Single);
    }
}
