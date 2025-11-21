using System;
using UnityEngine;

// https://github.com/Twaish/Go-Marble/blob/main/Assets/Scripts/Core/Time/TimerUIController.cs
public class TimerUIController : MonoBehaviour {
  [SerializeField] private TMPro.TMP_Text timerText;

  private TimerManager timerManager;

  private void Awake() {
    if (timerText == null) {
      Debug.LogError("TimerUIController: timerText not assigned");
      enabled = false;
      return;
    }

    timerManager = GetComponent<TimerManager>();
    if (timerManager == null) {
      Debug.LogError("TimerUIController: TimerManager not found");
      enabled = false;
      return;
    }

    timerManager.OnTimerUpdated += UpdateTimerText;
    timerManager.OnTimerStopped += UpdateTimerText;
    timerManager.OnTimerReset += ClearText;
  }

  private void OnDestroy() {
    if (timerManager == null) return;
    timerManager.OnTimerUpdated -= UpdateTimerText;
    timerManager.OnTimerReset -= ClearText;
  }

  private void UpdateTimerText(float timeInSeconds) {
    TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
    timerText.text = $"{time:mm\\:ss\\.ff}s";
  }

  private void ClearText() {
    UpdateTimerText(0);
  }
}