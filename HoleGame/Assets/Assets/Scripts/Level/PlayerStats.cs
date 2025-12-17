using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
  [Header("Дані гравця")]
  public int currentLevel = 1;
  public int currentXP = 0;      // тимчасовий XP (скидається при level up)
  public int totalXP = 0;    
  public int xpToNextLevel = 10; // скільки потрібно для наступного рівня
  [Header("Збільшення гравця")]
  public float increaseScale = 1.1f;
  public float increaseOffset = 1.1f;
  [Header("Scripts")]
  private PlayerLevelUI playerLevelUI;
  [Header("UI")]
  public Slider xpSlider;
  public float smoothTime = 0.3f;
  public Transform mainCameraTransform;  // швидкість анімації (0.2–0.5 — добре)

  private Coroutine smoothCoroutine;  // щоб не плодити 100 корутин
  private Coroutine scaleCoroutine;
  private Coroutine cameraCoroutine;

  private Vector3 targetScale;
  private void Awake()
  {
    playerLevelUI = GetComponent<PlayerLevelUI>();
    if (xpSlider != null)
      xpSlider.value = GetProgress();

    targetScale = transform.localScale;
    playerLevelUI.SetLevelText(currentLevel);
  }
  private void Update()
  {
    AddExpCheat();
  }

  /// <summary>
  /// Додає досвід. Автоматично level up, скидання currentXP, оновлення xpToNext.
  /// </summary>
  public void AddXP(int amount)
  {
    if (amount <= 0) return;

    totalXP += amount;
    currentXP += amount;

    // Level up loop
    while (currentXP >= xpToNextLevel)
    {
      currentXP -= xpToNextLevel;
      currentLevel++;
      playerLevelUI.SetLevelTextWithAnim(currentLevel);
      playerLevelUI.PlayRippleVFX();
      // Збільшуємо гравця на 10%
      targetScale *= increaseScale;
      if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
      scaleCoroutine = StartCoroutine(SmoothScale(targetScale));
      if (mainCameraTransform != null)
      {
        if (cameraCoroutine != null) StopCoroutine(cameraCoroutine);
        cameraCoroutine = StartCoroutine(SmoothCameraOffset(mainCameraTransform.localPosition * increaseOffset));
      }

      // Оновлюємо xpToNextLevel
      UpdateXPToNext();

      Debug.Log($"Level Up! {currentLevel} (Total XP: {totalXP})");
    }
    if (smoothCoroutine != null)
      StopCoroutine(smoothCoroutine);

    smoothCoroutine = StartCoroutine(SmoothFill(GetProgress()));
  }

  /// <summary>
  /// Оновлює xpToNextLevel залежно від рівня
  /// До level 10: завжди 10 (totalXP <100)
  /// Після: *1.1 (округлення до int)
  /// </summary>
  private void UpdateXPToNext()
  {
    if (currentLevel <= 10)
    {
      xpToNextLevel = 10;
    }
    else
    {
      xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);
    }
  }

  /// <summary>
  /// Прогрес бару для UI: 0..1
  /// </summary>
  public float GetProgress()
  {
    return xpToNextLevel > 0 ? (float)currentXP / xpToNextLevel : 0f;
  }

  private IEnumerator SmoothFill(float targetValue)
  {
    float startValue = xpSlider.value;
    float elapsed = 0f;

    while (elapsed < smoothTime)
    {
      elapsed += Time.deltaTime;
      xpSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / smoothTime);
      yield return null;
    }

    xpSlider.value = targetValue; // точне значення в кінці
    smoothCoroutine = null;
  }

  // Плавна анімація масштабу гравця
  private IEnumerator SmoothScale(Vector3 target)
  {
    Vector3 startScale = transform.localScale;
    float elapsed = 0f;

    while (elapsed < smoothTime)
    {
      elapsed += Time.deltaTime;
      transform.localScale = Vector3.Lerp(startScale, target, elapsed / smoothTime);
      yield return null;
    }
    transform.localScale = target;
    scaleCoroutine = null;
  }

  // Плавний від'їзд камери (Y і Z)
  private IEnumerator SmoothCameraOffset(Vector3 targetOffset)
  {
    Vector3 startOffset = mainCameraTransform.localPosition;
    float elapsed = 0f;
    yield return new WaitForSeconds(smoothTime);

    while (elapsed < smoothTime)
    {
      elapsed += Time.deltaTime;
      mainCameraTransform.localPosition = Vector3.Lerp(startOffset, targetOffset, elapsed / smoothTime);
      yield return null;
    }
    mainCameraTransform.localPosition = targetOffset;
    cameraCoroutine = null;
  }

  // Геттери для UI
  public int Level => currentLevel;
  public int XP => currentXP;
  public int XPNext => xpToNextLevel;
  public int TotalXP => totalXP;

  public void AddExpCheat()
  {
    if (Input.GetKeyDown(KeyCode.Alpha7))
    {
      AddXP(1);
    }
    if (Input.GetKeyDown(KeyCode.Alpha8))
    {
      AddXP(10);
    }
    if (Input.GetKeyDown(KeyCode.Alpha9))
    {
      AddXP(50);
    }
  }
}
