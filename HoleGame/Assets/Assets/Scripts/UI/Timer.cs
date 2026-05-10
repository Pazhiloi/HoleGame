using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class Timer : MonoBehaviour
{
  [Header("UI References")]
  [SerializeField] private TextMeshProUGUI timerText;
  [SerializeField] private Slider timerSlider;
  [SerializeField] private Image sliderFillImage;
  [SerializeField] private VignetteManager vignetteManager;

  [Header("Settings")]
  [SerializeField] private float initialTime = 60f; // Початковий час рівня

  [SerializeField] private Color normalColor = Color.green;
  [SerializeField] private Color warningColor = Color.yellow;
  [SerializeField] private Color dangerColor = Color.red;


  [Header("Star Thresholds")]
  [SerializeField] private float remainingForThreeStars = 30f;
  [SerializeField] private float remainingForTwoStars = 10f;

  private float remainingTime;
  private bool isTimerRunning = true;
  private bool isPulsing = false;// Щоб не запускати анімацію блимання щокадру

  private void Awake()
  {
    InitializeTimer();
  }

  void Update()
  {
    if (!isTimerRunning) return;
    HandleTimeFlow();
  }

  private void InitializeTimer()
  {
    remainingTime = initialTime;
    timerSlider.maxValue = initialTime;
    timerSlider.value = initialTime;
    if (sliderFillImage != null) sliderFillImage.color = normalColor;
    UpdateUI();
  }

  private void HandleTimeFlow()
  {
    if (remainingTime > 0)
    {
      remainingTime -= Time.deltaTime;
      timerSlider.value = remainingTime;
      UpdateVisuals();
    }
    else
    {
      // ВАЖЛИВО: Спочатку зупиняємо все, потім викликаємо поразку
      StopTimerHard();
      VictoryManager.Instance.Defeat();
    }
    UpdateUI();
  }

  private void UpdateVisuals()
  {
    // Якщо таймер не активний — жодних перевірок візуалу!
    if (!isTimerRunning) return;

    UpdateSliderColor();


    // Запуск пульсації
    if (remainingTime <= 10f && !isPulsing)
    {
      StartTextPulse();
    }
    // Автоматична зупинка, якщо додали час
    else if (remainingTime > 10f && isPulsing)
    {
      ResetVisualsToNormal();
    }
  }

  private void UpdateSliderColor()
  {
    if (sliderFillImage == null) return;

    float ratio = remainingTime / initialTime;

    if (remainingTime <= 10f || ratio <= 0.2f)
      sliderFillImage.color = dangerColor;
    else if (ratio <= 0.5f)
      sliderFillImage.color = warningColor;
    else
      sliderFillImage.color = normalColor;
  }

  public void StopTimerHard()
  {
    isTimerRunning = false;
    isPulsing = false; // Обов'язково скидаємо тут

    // Вбиваємо ВСІ твіни на тексті та самому об'єкті
    timerText.DOKill(true);
    timerText.transform.DOKill(true);

    // Примусово ставимо фінальний стан
    timerText.color = dangerColor;
    timerText.alpha = 1f;
    timerText.transform.localScale = Vector3.one;

    remainingTime = 0;
    timerSlider.value = 0;

    StopVignette();
  }

  private void StartTextPulse()
  {
    if (VictoryManager.Instance.isDefeat) return;

    isPulsing = true;
    timerText.color = dangerColor;

    // DOTween: плавно міняємо прозорість або колір туди-сюди
    timerText.DOFade(0.2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetId("TimerPulse"); ;

    // Можна також додати легке збільшення
    timerText.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetId("TimerPulse"); ;

    // НОВЕ: Робота з віньєткою
    StartVignette();

  }


  private void ResetVisualsToNormal()
  {
    isPulsing = false;
    timerText.DOKill();
    timerText.transform.DOKill();

    timerText.color = Color.white;
    timerText.alpha = 1f;
    timerText.transform.localScale = Vector3.one;
    StopVignette();
  }

  public void AddExtraTime(float seconds)
  {
    VictoryManager.Instance.isDefeat = false;
    remainingTime += seconds;

    if (remainingTime > timerSlider.maxValue)
      timerSlider.maxValue = remainingTime;

    timerSlider.value = remainingTime;

    isTimerRunning = true; // Тепер Update знову почне працювати
    ResetVisualsToNormal();

    // Якщо час став більше 10, ResetVisualsToNormal спрацює в UpdateVisuals
    UpdateUI();
  }

  

  private void StartVignette()
  {
    if (vignetteManager != null)
    {
      vignetteManager.StartVignettePulse();
    }
  }
  private void StopVignette()
  {
    if (vignetteManager != null)
    {
      vignetteManager.StopVignette();
    }
  }

  
  public int GetStarsResult()
  {
    if (remainingTime >= remainingForThreeStars) return 3;
    if (remainingTime >= remainingForTwoStars) return 2;
    return 1;
  }

  void UpdateUI()
  {
    int minutes = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) / 60);
    int seconds = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) % 60);
    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
  }

  public string GetCurrentTimeText()
  {
    int minutes = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) / 60);
    int seconds = Mathf.FloorToInt(Mathf.Max(remainingTime, 0) % 60);
    return string.Format("{0:00}:{1:00}", minutes, seconds);
  }


}
