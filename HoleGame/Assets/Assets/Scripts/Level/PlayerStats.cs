using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
  [Header("Дані гравця")]
  public int currentLevel = 1;
  public int currentXP = 0;      // тимчасовий XP (скидається при level up)
  public int totalXP = 0;        // загальний XP (не скидається)
  public int xpToNextLevel = 10; // скільки потрібно для наступного рівня
  [Header("UI")]
  public Slider xpSlider;
  public float smoothTime = 0.3f;     // швидкість анімації (0.2–0.5 — добре)

  private Coroutine smoothCoroutine;  // щоб не плодити 100 корутин
  private void Awake()
  {
    if (xpSlider != null)
      xpSlider.value = GetProgress();
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

  // Геттери для UI
  public int Level => currentLevel;
  public int XP => currentXP;
  public int XPNext => xpToNextLevel;
  public int TotalXP => totalXP;

  public void AddExpCheat()
  {
    if (Input.GetKeyDown(KeyCode.Alpha9))
    {
      AddXP(1);
    }
  }
}
