using System;
using UnityEngine;

public class PointSystem : MonoBehaviour
{
    [SerializeField]
    private int points;

    public event Action<int> OnPointsChanged;

    public void AddPoints(int amount)
    {
        points += amount;
        OnPointsChanged?.Invoke(points);
    }

    public void RemovePoints(int amount)
    {
        points -= amount;
        OnPointsChanged?.Invoke(points);
    }

    public void ResetPoints()
    {
        points = 0;
        OnPointsChanged?.Invoke(points);
    }

    public int Points => points;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnPointsChanged?.Invoke(points);
        }
    }
#endif
}
