using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] float remainingTime;
  [SerializeField] private Slider timerSlider;
  private bool isTimerEnded = false;

  private void Awake()
  {
    timerSlider.maxValue = remainingTime;
    timerSlider.value = remainingTime;
  }

  void Update()
  {
    HandleTimer();
  }

  private void HandleTimer()
  {
    if (isTimerEnded) return;
    if (remainingTime > 0)
    {
      remainingTime -= Time.deltaTime;
      timerSlider.value = remainingTime;
    }
    else if (remainingTime <= 0)
    {
      remainingTime = 0;
      timerSlider.value = 0;
      isTimerEnded = true;
    }
    UpdateUI();
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
