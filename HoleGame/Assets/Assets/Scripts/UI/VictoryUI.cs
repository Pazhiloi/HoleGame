using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class VictoryUI : MonoBehaviour
{
  [SerializeField] private GameObject victoryPanel;
  [SerializeField] private RectTransform victoryTimeTransform; 
  [SerializeField] private RectTransform continueButtonTransform;
  [SerializeField] private CanvasGroup canvasGroupVictory; // Додай CanvasGroup на VictoryPanel для плавного проявлення
  [SerializeField] private RectTransform victoryText;
  [SerializeField] private TextMeshProUGUI victoryTimeText;
  [Header("Зірки")]
  [SerializeField] private List<Image> starImages; // Сюди перетягни 3 картинки зірок
  [SerializeField] private Sprite fullStarSprite;  // Спрайт золотої зірки
  [SerializeField] private Sprite emptyStarSprite; // Спрайт порожньої зірки
  [Header("Ефекти салюту")]
  [SerializeField] private List<ParticleSystem> fireworks;
  private bool isAnimationPlaying = false; // Запобіжник

  private void Awake()
  {
    // Ховаємо панель на старті
    HandleVictoryPanel();
    starsOff();
  }

  private void HandleVictoryPanel()
  {
    victoryPanel.SetActive(false);
    if (canvasGroupVictory != null) canvasGroupVictory.alpha = 0;
  }

 
  public void ShowVictoryScreen(int starsEarned, string finalTime)
  {
    if (isAnimationPlaying) return;
    isAnimationPlaying = true;
   

    victoryPanel.SetActive(true);
    victoryTimeTransform.localScale = Vector3.zero;
    continueButtonTransform.localScale = Vector3.zero;
    

      if (victoryTimeText != null)
      {
        victoryTimeText.text = finalTime;
      }

    // Плавна поява фону
    canvasGroupVictory?.DOFade(1f, 0.5f);

    // Ефектна поява тексту (вилітає або збільшується)
    if (victoryText != null)
    {
      victoryText.localScale = Vector3.zero;
      victoryText.DOScale(Vector3.one, 1.6f).SetEase(Ease.OutBack).OnComplete(() => 
                {
                  victoryText.DOScale(Vector3.zero, 1.6f).OnComplete(() =>
                  {
                     victoryText.gameObject.SetActive(false);
                    starsOn();
                    foreach (var star in starImages)
                    {
                      star.sprite = emptyStarSprite;
                      star.rectTransform.localScale = Vector3.one; // Повертаємо нормальний масштаб для сірих
                    }

                    // Цей код виконається ТІЛЬКИ після завершення анімації тексту
                    LaunchFireworks();
                    AnimateStars(starsEarned);

                  });
                 
                });
    }

    
  }


private void AppearTimeAndButton()
  {
    Sequence victorySeq = DOTween.Sequence();

    victorySeq.Append(victoryTimeTransform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

    // Додаємо невелику паузу між текстом і кнопкою
    victorySeq.AppendInterval(0.2f);

    // З'являється кнопка продовжити
    victorySeq.Append(continueButtonTransform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
  }
  private void AnimateStars(int starsEarned)
  {
    foreach (var star in starImages) star.rectTransform.DOKill();
    Sequence starSequence = DOTween.Sequence();

    for (int i = 0; i < starsEarned; i++)
    {
      int index = i;

      // 1. Готуємо зірку: робимо її невидимою (scale = 0) перед тим як показати золоту
      starSequence.AppendCallback(() =>
      {
        starImages[index].rectTransform.localScale = Vector3.zero; // Зменшуємо в нуль
        starImages[index].sprite = fullStarSprite;                // Міняємо спрайт
      });

      // 2. Анімація появи: збільшуємо трохи більше одиниці і повертаємо в норму
      // Це створить ефект "вистрибування" без зайвого дрижання
      starSequence.Append(starImages[index].rectTransform
             .DOScale(Vector3.one, 0.4f)
             .SetEase(Ease.OutBack));

      // Маленька пауза перед наступною зіркою
      starSequence.AppendInterval(0.15f);
    }
    // В кінці всієї послідовності дозволяємо запуск знову (хоча панель уже буде закрита)
    starSequence.OnComplete(() => {
      isAnimationPlaying = false;
      AppearTimeAndButton();
    });
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

  private void starsOff()
  {
    foreach (var star in starImages)
    {
      star.gameObject.SetActive(false); 
    }
  }
  private void starsOn()
  {
    foreach (var star in starImages)
    {
      star.gameObject.SetActive(true);
    }
  }

}