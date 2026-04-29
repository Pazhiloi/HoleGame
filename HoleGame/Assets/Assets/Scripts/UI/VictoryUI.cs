using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
  [SerializeField] private GameObject victoryPanel;
  [SerializeField] private CanvasGroup canvasGroup; // Додай CanvasGroup на VictoryPanel для плавного проявлення
  [SerializeField] private RectTransform victoryText;
  [Header("Ефекти салюту")]
  [SerializeField] private List<ParticleSystem> fireworks;

  private void Awake()
  {
    // Ховаємо панель на старті
    victoryPanel.SetActive(false);
    if (canvasGroup != null) canvasGroup.alpha = 0;
  }

  public void ShowVictoryScreen()
  {
    victoryPanel.SetActive(true);

    // Плавна поява фону
    canvasGroup?.DOFade(1f, 0.5f);

    // Ефектна поява тексту (вилітає або збільшується)
    if (victoryText != null)
    {
      victoryText.localScale = Vector3.zero;
      victoryText.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack).OnComplete(() => 
                {
                    // Цей код виконається ТІЛЬКИ після завершення анімації тексту
                    LaunchFireworks();
                });;
    }
  }

  private void LaunchFireworks()
  {
    float delay = 0f;
    foreach (ParticleSystem ps in fireworks)
    {
      // Запускаємо через невелику затримку (наприклад, кожні 0.2 сек)
      DOVirtual.DelayedCall(delay, () => ps.Play());
      delay += 0.2f;
    }
  }

}