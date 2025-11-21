using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;

    private void Start()
    {
        timerManager.StartTimer();
    }
}
