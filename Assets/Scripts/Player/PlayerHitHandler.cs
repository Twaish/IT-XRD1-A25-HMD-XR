using UnityEngine;
using System;

public class PlayerHitHandler : MonoBehaviour
{
    public event Action OnHit;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laser") || other.CompareTag("Sword"))
        {
            OnHit?.Invoke();
        }
    }
}
