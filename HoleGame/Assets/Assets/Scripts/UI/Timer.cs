using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Timer : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private float initialTime = 60f; // Початковий час рівня
  [SerializeField] private Slider timerSlider;

  [Header("Налаштування кольорів")]
  [SerializeField] private Color normalColor = Color.green;
  [SerializeField] private Color warningColor = Color.yellow;
  [SerializeField] private Color dangerColor = Color.red;
  [SerializeField] private Image sliderFillImage;
  [Header("Налаштування Зірок")]
  [Tooltip("Скільки часу має ЗАЛИШИТИСЯ для 3 зірок")]
  [SerializeField] private float remainingForThreeStars = 30f;
  [Tooltip("Скільки часу має ЗАЛИШИТИСЯ для 2 зірок")]
  [SerializeField] private float remainingForTwoStars = 10f;

  [Header("Ефекти Віньєтки")]
  [SerializeField] private VignetteManager vignetteManager;
  private float remainingTime;
  private bool isTimerRunning = true;
  private bool isPulsing = false;// Щоб не запускати анімацію блимання щокадру

  private void Awake()
  {
    HandleSliderOnAwake();
  }

  void Update()
  {
    HandleTimer();
  }

  private void HandleSliderOnAwake()
  {
    remainingTime = initialTime;
    timerSlider.maxValue = initialTime;
    timerSlider.value = initialTime;
    if (sliderFillImage != null) sliderFillImage.color = normalColor;
  }

  private void HandleTimer()
  {
    if (!isTimerRunning) return;
    if (remainingTime > 0)
    {
      remainingTime -= Time.deltaTime;
      timerSlider.value = remainingTime;
      UpdateVisuals();
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

  private void UpdateVisuals()
  {
    float ratio = remainingTime / initialTime;
    if (sliderFillImage != null)
    {
      if (ratio <= 0.2f || remainingTime <= 10f) // 1/5 часу
        sliderFillImage.color = dangerColor;
      else if (ratio <= 0.5f) // Половина часу
        sliderFillImage.color = warningColor;
      else
        sliderFillImage.color = normalColor;
    }

    if (remainingTime <= 10f && !isPulsing)
    {
      StartTextPulse();
    }
  }

  private void StartTextPulse()
  {
    isPulsing = true;
    timerText.color = dangerColor;

    // DOTween: плавно міняємо прозорість або колір туди-сюди
    timerText.DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);

    // Можна також додати легке збільшення
    timerText.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);

    // НОВЕ: Робота з віньєткою
    StartVignette();
  }

  private void StartVignette(){
    if (vignetteManager != null)
    {
      vignetteManager.StartVignettePulse();
    }
  }
  private void StopVignette(){
    if (vignetteManager != null)
    {
      vignetteManager.StopVignette();
    }
  }

  public void StopTimer()
  {
    isTimerRunning = false;
    timerText.DOKill();
    timerText.transform.localScale = Vector3.one;
    StopVignette();
  }
  private void OnTimerEnd()
  {
    VictoryManager.Instance.Defeat();
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
