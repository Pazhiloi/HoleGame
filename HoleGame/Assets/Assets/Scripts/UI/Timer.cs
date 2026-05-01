using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private float initialTime = 60f; // Початковий час рівня
  [SerializeField] private Slider timerSlider;
  [Header("Налаштування Зірок")]
  [Tooltip("Скільки часу має ЗАЛИШИТИСЯ для 3 зірок")]
  [SerializeField] private float remainingForThreeStars = 30f;
  [Tooltip("Скільки часу має ЗАЛИШИТИСЯ для 2 зірок")]
  [SerializeField] private float remainingForTwoStars = 10f;
  private float remainingTime;
  private bool isTimerRunning = true;

  private void Awake()
  {
    remainingTime = initialTime;
    timerSlider.maxValue = initialTime;
    timerSlider.value = initialTime;
  }

  void Update()
  {
    HandleTimer();
  }

  private void HandleTimer()
  {
    if (!isTimerRunning) return;
    if (remainingTime > 0)
    {
      remainingTime -= Time.deltaTime;
      timerSlider.value = remainingTime;
    }
    else if (remainingTime <= 0)
    {
      remainingTime = 0;
      timerSlider.value = 0;
      isTimerRunning = false;
      OnTimerEnd();
    }
    UpdateUI();
  }
  public void StopTimer()
  {
    isTimerRunning = false;
  }
  private void OnTimerEnd()
  {
    // Логіка програшу, якщо час вийшов
    Debug.Log("Time is up! Game Over.");
  }
  public int GetStarsResult()
  {
    // 3 зірки: залишилося більше ніж remainingForThreeStars
    if (remainingTime >= remainingForThreeStars) return 3;

    // 2 зірки: залишилося більше ніж remainingForTwoStars
    if (remainingTime >= remainingForTwoStars) return 2;

    // 1 зірка: просто встиг до кінця таймера
    return 1;
  }

  void UpdateUI()
  {
    int minutes = Mathf.FloorToInt(remainingTime / 60);
    int seconds = Mathf.FloorToInt(remainingTime % 60);
    if (remainingTime <= 0)
    {
      timerText.text = "00:00";
    }
    else
    {
      timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
  }
}
