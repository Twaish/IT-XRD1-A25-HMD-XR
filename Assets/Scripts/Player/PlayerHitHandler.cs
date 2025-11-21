using UnityEngine;
using System;

public class PlayerHitHandler : MonoBehaviour
{
    public event Action OnHit;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser"))
        {
            OnHit?.Invoke();
        }

        if (other.CompareTag("Sword"))
        {
            Debug.Log("I HAVE BEEN HIT AND AM IN A INSUFFRABLE AMOUNT OF PAIN I AM DEFENETLY ABOUT TO GO INSANE I CAN NOT TAKE THIS ANYMORE PLS SAVE ME TAKE ME AWAY BFFORE I EAT MY SABER; AAAAAHHHHHHHRRHRHRHHRHRHRRRRHHHH");
            RegisterHit();
        }
    }
}
