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
  public RectTransform targetPanel; // Рамка, куди цей ворог має летіти
  public TMP_Text countText;     // Текст із цифрою для цієї рамки
  public float remainingCount; // Скільки ще треба зібрати
  public GameObject readyIcon;  
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
        goal.readyIcon.SetActive(false);
      }
    }

  }

  public void AnimateEnemyCollection(Vector3 holeWorldPos, EnemyType type)
  {
    VictoryGoal goal = victoryGoals.Find(g => g.type == type);
    if (goal == null || goal.iconPrefab == null) return;

    // 1. Початкова точка (позиція дірки)
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(holeWorldPos);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 startLocalPoint);

    // 2. Ключове виправлення: вираховуємо реальну позицію ЦІЛІ відносно Canvas
    // Отримуємо позицію рамки в екранних координатах
    Vector2 targetScreenPoint = RectTransformUtility.WorldToScreenPoint(null, goal.targetUI.position);
    // Перетворюємо її в локальні координати нашого Canvas
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPoint, null, out Vector2 targetLocalPoint);

    // 3. Створення іконки
    GameObject icon = Instantiate(goal.iconPrefab, canvasRect);
    RectTransform iconRect = icon.GetComponent<RectTransform>();

    // Ставимо в точку дірки
    iconRect.anchoredPosition = startLocalPoint;
    iconRect.localScale = Vector3.zero;
    iconRect.DOScale(Vector3.one, 0.2f);

    // Тепер летимо до targetLocalPoint, а не до anchoredPosition
    iconRect.DOAnchorPos(targetLocalPoint, flyDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
          goal.targetPanel.DOKill();
          goal.targetPanel.localScale = Vector3.one;
          goal.targetPanel.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.2f);
          Destroy(icon);

          if (goal.remainingCount > 0)
          {
            goal.remainingCount--;
            goal.countText.text = goal.remainingCount.ToString();
            if (goal.remainingCount <= 0)
            {
              goal.countText.gameObject.SetActive(false);
              goal.readyIcon.SetActive(true);

              // Можна додати маленький ефект появи галочки
              goal.readyIcon.transform.localScale = Vector3.zero;
              goal.readyIcon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

              Sequence finishSequence = DOTween.Sequence();

              finishSequence.AppendInterval(0.3f); // Невелика пауза, щоб гравець побачив галочку

              // 3. Обертання та зменшення одночасно (Join)
              finishSequence.Append(goal.targetPanel.DORotate(new Vector3(0, 360, 0), 0.6f, RotateMode.FastBeyond360));
              finishSequence.Join(goal.targetPanel.DOScale(Vector3.zero, 0.6f).SetEase(Ease.InBack));

              // 4. Вимикаємо об'єкт після завершення
              finishSequence.OnComplete(() =>
              {
                goal.targetPanel.gameObject.SetActive(false);
              });
            }
            CheckVictory();
          }
          
        });

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
