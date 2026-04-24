using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
  public GameObject iconPanel;
  public ParticleSystem collectVFX;
  public GameObject backSide;// Твоя порожня рамка (спина)
  public bool isFinishing = false;
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
        if (goal.backSide != null) goal.backSide.SetActive(false);
      }
    }

  }

  public void AnimateEnemyCollection(Vector3 holeWorldPos, EnemyType type)
  {
    VictoryGoal goal = victoryGoals.Find(g => g.type == type);

    // 1. ГОЛОВНА ПЕРЕВІРКА: Якщо ціль уже виконана (0 або менше), 
    // або рамка вже зникла — просто виходимо і нічого не спавнимо.
    if (goal == null || goal.iconPrefab == null || goal.remainingCount <= 0) return;

    // 2. МИТТЄВО зменшуємо лічильник. 
    // Наступний ворог, який викличе цей метод через мілісекунду, побачить уже менше число.
    goal.remainingCount--;

    // Далі твій стандартний код розрахунку позицій...
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(holeWorldPos);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 startLocalPoint);

    Vector2 targetScreenPoint = RectTransformUtility.WorldToScreenPoint(null, goal.targetUI.position);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPoint, null, out Vector2 targetLocalPoint);

    GameObject icon = Instantiate(goal.iconPrefab, canvasRect);
    RectTransform iconRect = icon.GetComponent<RectTransform>();

    iconRect.anchoredPosition = startLocalPoint;
    iconRect.localScale = Vector3.zero;
    iconRect.DOScale(Vector3.one, 0.2f);

    iconRect.DOAnchorPos(targetLocalPoint, flyDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
          // ЗАПУСК ЕФЕКТУ БЛИСКІТОК
          if (goal.collectVFX != null)
          {
            // .Stop() потрібен, щоб скинути попередній запуск, якщо іконки летять дуже швидко
            goal.collectVFX.Stop();
            goal.collectVFX.Play();
          }
          goal.targetPanel.DOKill();
          goal.targetPanel.localScale = Vector3.one;
          goal.targetPanel.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.2f);
          Destroy(icon);

          // 3. ОНОВЛЮЄМО ТЕКСТ. 
          // Тут ми вже не віднімаємо одиницю (ми це зробили на старті), 
          // а просто показуємо поточне значення.
          goal.countText.text = goal.remainingCount.ToString();

          // 4. ПЕРЕВІРКА НА ФІНАЛ. 
          // Якщо після прильоту іконки лічильник став 0 — запускаємо твою круту анімацію.
          if (goal.remainingCount <= 0)
          {

            StartFinalAnimation(goal);
           
          }

          // Важливо викликати перевірку перемоги тут
          CheckVictory();
        });

    iconRect.DORotate(new Vector3(0, 0, 360), flyDuration, RotateMode.FastBeyond360);
  }

  private void StartFinalAnimation(VictoryGoal goal)
    {
    goal.isFinishing =false;
       goal.countText.gameObject.SetActive(false);
        goal.readyIcon.SetActive(true);
        if (goal.backSide != null) goal.backSide.SetActive(false);

        goal.readyIcon.transform.localScale = Vector3.zero;
            goal.readyIcon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        LayoutElement layoutElement = goal.targetPanel.GetComponent<LayoutElement>();
        Sequence finishSequence = DOTween.Sequence();

    finishSequence.Append(goal.targetPanel.DORotate(new Vector3(0, 360, 0), 0.3f, RotateMode.FastBeyond360)
        .SetRelative(true)
        .SetEase(Ease.Linear)
        .OnUpdate(() => UpdateCardVisibility(goal)));

    // 2. ПАУЗА (невелика затримка для стабілізації)
    finishSequence.AppendInterval(0.1f);

    // 3. РОЗВОРОТ СПИНОЮ (180 градусів)
    finishSequence.Append(goal.targetPanel.DORotate(new Vector3(0, 180, 0), 0.3f, RotateMode.FastBeyond360)
        .SetRelative(true)
        .SetEase(Ease.OutQuad)
        .OnUpdate(() => UpdateCardVisibility(goal)));

    finishSequence.AppendCallback(() =>
    {
      goal.isFinishing = true;
      // Вимикаємо іконку ворога, бо панель вже стоїть до нас спиною
      goal.readyIcon.SetActive(false);
      goal.iconPanel.SetActive(false);
      goal.targetUI.gameObject.SetActive(false);

      // Якщо є ще якісь елементи на лицьовій стороні, які "світяться" — їх теж тут
      if (goal.countText != null) goal.countText.gameObject.SetActive(false);
    });

    // 4. ЗМЕНШЕННЯ (Scale до 0)
    finishSequence.Append(goal.targetPanel.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));

    // 5. ЗВУЖЕННЯ (Preferred Width до 0)
    if (layoutElement != null)
    {
      finishSequence.Join(DOTween.To(() => layoutElement.preferredWidth, x => layoutElement.preferredWidth = x, 0, 0.1f));
    }

    finishSequence.OnComplete(() =>
    {
      goal.targetPanel.gameObject.SetActive(false);
      CheckVictory();
    });

  }

  private void UpdateCardVisibility(VictoryGoal goal)
  {
    if (goal.isFinishing)return;
    // Отримуємо поточний кут Y в межах 0-360
    float yAngle = goal.targetPanel.localEulerAngles.y;

    // Нормалізуємо кут (щоб уникнути багів Unity з від'ємними значеннями)
    float normalizedY = yAngle % 360;
    if (normalizedY < 0) normalizedY += 360;

    // Твоя ідея: якщо кут між 90 і 270 — показуємо спину
    bool isBackVisible = normalizedY > 90 && normalizedY < 270;

    if (goal.readyIcon.activeSelf == isBackVisible) // Вмикаємо Front, якщо !isBackVisible
    {
      goal.readyIcon.SetActive(!isBackVisible);
    }

    if (goal.backSide != null && goal.backSide.activeSelf != isBackVisible)
    {
      goal.backSide.SetActive(isBackVisible);
    }
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
