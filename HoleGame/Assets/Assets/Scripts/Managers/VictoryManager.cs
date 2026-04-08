using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class VictoryManager : MonoBehaviour
{
  public static VictoryManager Instance { get; private set; }

  [Header("UI Елементи")]
  [SerializeField] private GameObject enemyIconPrefab;
  [SerializeField] private RectTransform canvasRect;
  [SerializeField] private RectTransform victoryTarget;
  [SerializeField] private float victoryCount;
  [SerializeField] private TMP_Text victoryCountText;

   [Header("Налаштування польоту")]
  [SerializeField] private float flyDuration = 0.8f;

  private void Awake()
  {
    // Реалізація паттерна Сінглтон
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject); // Видаляємо дублікат, якщо він з'явився
    }
    else
    {
      Instance = this;
      DontDestroyOnLoad(gameObject); // Розкоментуйте, якщо об'єкт має жити при переході між сценами
    }
  }

  public void AnimateEnemyCollection(Vector3 holeWorldPos)
  {
    // 1. ПЕРЕКЛАД З 3D В 2D
    // Отримуємо позицію в пікселях екрана (Screen Space)
    Vector2 screenPoint = Camera.main.WorldToScreenPoint(holeWorldPos);

    // Перетворюємо пікселі в локальні координати твого Canvas
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvasRect,
        screenPoint,
        null, // Для Overlay Canvas тут null, для Camera Canvas — Camera.main
        out Vector2 localPoint
    );

    // 2. СТВОРЕННЯ ТА ПОЛІТ
    GameObject icon = Instantiate(enemyIconPrefab, canvasRect);
    RectTransform iconRect = icon.GetComponent<RectTransform>();

    // Ставимо іконку в центр дірки на екрані

    // Летимо до рамки
    // victoryTarget.anchoredPosition — це координати твоєї рамки відносно Canvas
    iconRect.DOLocalMove(victoryTarget.localPosition, flyDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
          // Ефект прильоту: рамка трохи "дригається"
          victoryTarget.DOPunchScale(new Vector3(0.15f, 0.15f, 0.15f), 0.2f);
          Destroy(icon);
          victoryCount-=1;
          victoryCountText.text = victoryCount.ToString();

          if (victoryCount <=0)
          {
            victoryCount = 0;
            victoryCountText.text = victoryCount.ToString();
            Debug.Log("Victory!!!");
          }
        });

    // Додамо трохи обертання для краси
    iconRect.DORotate(new Vector3(0, 0, 360), flyDuration, RotateMode.FastBeyond360);
  }

}
