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
  [SerializeField] private List<VictoryGoal> victoryGoals;
  [SerializeField] private VictoryUI victoryUIScript;

  private void Awake()
  {
    InitializeSingleton();
    InitializeGoals();
  }

  // --- МЕТОДИ ІНІЦІАЛІЗАЦІЇ ---

  private void InitializeSingleton()
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
  }

  private void InitializeGoals()
  {
    foreach (var goal in victoryGoals)
    {
      if (goal?.countText != null)
      {
        goal.countText.text = goal.remainingCount.ToString();
        goal.readyIcon.SetActive(false);
        goal.backSide?.SetActive(false);
      }
    }
  }

  // --- ГОЛОВНА ЛОГІКА ЗБОРУ ---

  public void AnimateEnemyCollection(Vector3 holeWorldPos, EnemyType type)
  {
    VictoryGoal goal = victoryGoals.Find(g => g.type == type);

    if (!CanCollect(goal)) return;

    goal.remainingCount--;

    // Розрахунок позицій
    Vector2 startPos = CalculateStartScreenPos(holeWorldPos);
    Vector2 targetPos = CalculateTargetScreenPos(goal.targetUI);

    // Створення та запуск польоту іконки
    CreateFlyingIcon(goal, startPos, targetPos);
  }

  private bool CanCollect(VictoryGoal goal)
  {
    return goal != null && goal.iconPrefab != null && goal.remainingCount > 0;
  }

  // --- МАТЕМАТИКА КООРДИНАТ ---

  private Vector2 CalculateStartScreenPos(Vector3 worldPos)
  {
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 localPoint);
    return localPoint;
  }

  private Vector2 CalculateTargetScreenPos(RectTransform targetUI)
  {
    Vector2 targetScreenPoint = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPoint, null, out Vector2 localPoint);
    return localPoint;
  }

  // --- КЕРУВАННЯ АНІМАЦІЯМИ ІКОНОК ---

  private void CreateFlyingIcon(VictoryGoal goal, Vector2 startPos, Vector2 targetPos)
  {
    GameObject icon = Instantiate(goal.iconPrefab, canvasRect);
    RectTransform iconRect = icon.GetComponent<RectTransform>();

    iconRect.anchoredPosition = startPos;
    iconRect.localScale = Vector3.zero;

    // Початковий "виліт"
    iconRect.DOScale(Vector3.one, 0.2f);

    // Політ до цілі
    iconRect.DOAnchorPos(targetPos, flyDuration).SetEase(Ease.InQuad)
        .OnComplete(() => OnIconReachedTarget(icon, goal));

    // Обертання в польоті
    iconRect.DORotate(new Vector3(0, 0, 360), flyDuration, RotateMode.FastBeyond360);
  }

  private void OnIconReachedTarget(GameObject icon, VictoryGoal goal)
  {
    PlayCollectEffects(goal);
    Destroy(icon);

    goal.countText.text = goal.remainingCount.ToString();

    if (goal.remainingCount <= 0)
    {
      StartFinalAnimation(goal);
    }

    CheckVictory();
  }

  private void PlayCollectEffects(VictoryGoal goal)
  {
    if (goal.collectVFX != null)
    {
      goal.collectVFX.Stop();
      goal.collectVFX.Play();
    }

    goal.targetPanel.DOKill();
    goal.targetPanel.localScale = Vector3.one;
    goal.targetPanel.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.2f);
  }

  // --- ФІНАЛЬНІ АНІМАЦІЇ КАРТКИ ---

  private void StartFinalAnimation(VictoryGoal goal)
  {
    SetupFinalAnimationState(goal);

    Sequence finishSequence = DOTween.Sequence();
    LayoutElement layout = goal.targetPanel.GetComponent<LayoutElement>();

    // 1. Повне обертання
    finishSequence.Append(CreateRotationTween(goal, 360, Ease.Linear));
    finishSequence.AppendInterval(0.1f);

    // 2. Розмивання спиною
    finishSequence.Append(CreateRotationTween(goal, 180, Ease.OutQuad));

    // 3. Колбек для приховування нутрощів
    finishSequence.AppendCallback(() => FinalizeCardVisuals(goal));

    // 4. Зникнення
    finishSequence.Append(goal.targetPanel.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));

    if (layout != null)
      finishSequence.Join(DOTween.To(() => layout.preferredWidth, x => layout.preferredWidth = x, 0, 0.1f));

    finishSequence.OnComplete(() =>
    {
      goal.targetPanel.gameObject.SetActive(false);
      CheckVictory();
    });
  }

  private void SetupFinalAnimationState(VictoryGoal goal)
  {
    goal.isFinishing = false;
    goal.countText.gameObject.SetActive(false);
    goal.readyIcon.SetActive(true);
    goal.backSide?.SetActive(false);
    goal.readyIcon.transform.localScale = Vector3.zero;
    goal.readyIcon.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
  }

  private Tween CreateRotationTween(VictoryGoal goal, float angle, Ease ease)
  {
    return goal.targetPanel.DORotate(new Vector3(0, angle, 0), 0.3f, RotateMode.FastBeyond360)
        .SetRelative(true)
        .SetEase(ease)
        .OnUpdate(() => UpdateCardVisibility(goal));
  }

  private void FinalizeCardVisuals(VictoryGoal goal)
  {
    goal.isFinishing = true;
    goal.readyIcon.SetActive(false);
    goal.iconPanel.SetActive(false);
    goal.targetUI.gameObject.SetActive(false);
    goal.countText.gameObject.SetActive(false);
  }

  private void UpdateCardVisibility(VictoryGoal goal)
  {
    if (goal.isFinishing) return;

    float normalizedY = goal.targetPanel.localEulerAngles.y % 360;
    if (normalizedY < 0) normalizedY += 360;

    bool isBackVisible = normalizedY > 90 && normalizedY < 270;

    if (goal.readyIcon.activeSelf == isBackVisible)
      goal.readyIcon.SetActive(!isBackVisible);

    if (goal.backSide != null && goal.backSide.activeSelf != isBackVisible)
      goal.backSide.SetActive(isBackVisible);
  }

  private void CheckVictory()
  {
    bool allGoalsMet = victoryGoals.TrueForAll(g => g.remainingCount <= 0);
    if (allGoalsMet)
    {
      int stars = CalculateStars(); 
      victoryUIScript.ShowVictoryScreen(stars);
    }
  }
  private int CalculateStars()
  {
    // Приклад простої логіки:
    // 3 зірки — завжди, якщо пройшов (або додай умови за часом/HP)
    return 2; // Тимчасово повертаємо 2 для тесту
  }

}
