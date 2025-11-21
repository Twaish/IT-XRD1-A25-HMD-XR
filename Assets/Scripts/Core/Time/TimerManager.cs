using System;
using UnityEngine;

// https://github.com/Twaish/Go-Marble/blob/main/Assets/Scripts/Core/Time/TimerManager.cs
public class TimerManager : MonoBehaviour {
  private float startTime;
  private float pausedTime;
  private bool isRunning;
  private bool isPaused;

  public bool IsRunning => isRunning;
  public bool IsPaused => isPaused;

  public event Action<float> OnTimerUpdated;
  public event Action OnTimerStarted;
  public event Action<float> OnTimerStopped;
  public event Action OnTimerReset;
  
  private float currentTime;

  private void Update() {
    if (isRunning && !isPaused) {
      currentTime = Time.time - startTime;
      OnTimerUpdated?.Invoke(currentTime);
    }
  }

  public void StartTimer() {
    if (isRunning) return;
    isRunning = true;
    isPaused = false;
    startTime = Time.time - pausedTime;
    OnTimerStarted?.Invoke();
  }

  public void StopTimer() {
    if (!isRunning) return;
    isRunning = false;
    isPaused = false;

    currentTime = Time.time - startTime;
    pausedTime = currentTime;

    OnTimerStopped?.Invoke(currentTime);
  }

  public void PauseTimer() {
    if (!isRunning || isPaused) return;
    isPaused = true;
    currentTime = Time.time - startTime;
    pausedTime = currentTime;
  }

  public void ResumeTimer() {
    if (!isPaused) return;
    isPaused = false;
    startTime = Time.time - pausedTime;
  }

  public void ResetTimer() {
    isRunning = false;
    isPaused = false;
    startTime = 0f;
    pausedTime = 0f;
    currentTime = 0f;
    OnTimerReset?.Invoke();
    OnTimerUpdated?.Invoke(0f);
  }

  public float GetTime() => currentTime;
}