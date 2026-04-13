using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable] // Щоб ми бачили це в інспекторі
public class VictoryGoal
{
  public EnemyType type;         // Тип ворога (Red, Blue...)
  public GameObject iconPrefab;  // ГОТОВИЙ ПРЕФАБ ІКОНКИ (вже налаштований розмір)
  public RectTransform targetUI; // Рамка, куди цей ворог має летіти
  public TMP_Text countText;     // Текст із цифрою для цієї рамки
  public float remainingCount;   // Скільки ще треба зібрати
}

public class VictoryManager : MonoBehaviour
{
  public static VictoryManager Instance { get; private set; }

  [Header("UI Елементи")]
  [SerializeField] private RectTransform canvasRect;

   [Header("Налаштування польоту")]
  [SerializeField] private float flyDuration = 0.8f;
  [Header("Цілі перемоги")]
  [SerializeField] private List<VictoryGoal> victoryGoals; // Список наших цілей

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject); 
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); 
    }
    foreach (var goal in victoryGoals)
    {
      // Перевіряємо, чи ми не забули призначити текст в інспекторі
      if (goal != null && goal.countText != null)
      {
        goal.countText.text = goal.remainingCount.ToString();
      }
    }

  }

  public void AnimateEnemyCollection(Vector3 holeWorldPos, EnemyType type)
  {
    VictoryGoal goal = victoryGoals.Find(g => g.type == type);
    // Якщо такий ворог не потрібен для перемоги — нічого не робимо
    if (goal == null || goal.iconPrefab == null) return;
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(holeWorldPos);

    // Перетворюємо пікселі в локальні координати твого Canvas
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRect,
        screenPoint,
        null, // Для Overlay Canvas тут null, для Camera Canvas — Camera.main
        out Vector2 localPoint
    );

    // 3. Створення іконки
    GameObject icon = Instantiate(goal.iconPrefab, canvasRect);

    RectTransform iconRect = icon.GetComponent<RectTransform>();
    iconRect.anchoredPosition = localPoint;
    iconRect.localScale = Vector3.zero;
    iconRect.DOScale(Vector3.one, 0.2f);

    iconRect.DOAnchorPos(goal.targetUI.anchoredPosition, flyDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
          // Ефект прильоту: рамка трохи "дригається"
          goal.targetUI.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.2f);
          Destroy(icon);
          if (goal.remainingCount > 0)
          {
            goal.remainingCount--;
            goal.countText.text = goal.remainingCount.ToString();
            CheckVictory();
          }
        });

    // Додамо трохи обертання для краси
    iconRect.DORotate(new Vector3(0, 0, 360), flyDuration, RotateMode.FastBeyond360);
  }

  private void CheckVictory()
  {
    // Перевіряємо, чи всі цілі виконані (всі лічильники <= 0)
    bool allGoalsMet = victoryGoals.TrueForAll(g => g.remainingCount <= 0);

    if (allGoalsMet)
    {
      Debug.Log("LEVEL COMPLETE! ALL GOALS MET!");
    }
  }

}
