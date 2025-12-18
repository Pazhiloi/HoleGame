using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PlayerLevelUI : MonoBehaviour
{
  public TMP_Text levelText;
  public TMP_Text hideLevelText;
  public Vector3 scaleParams = new Vector3(2f,2f,2f);
  public float scaleDuration = 0.1f;
  public float moveHideTextDistance = 5f;
  public float moveHideTextDuration = 0.3f;
  public ParticleSystem rippleVFX;

  private void Awake() {
    hideLevelText.fontSize = 0;
  }



  public void SetLevelText(int level)
  {
    levelText.text = "Lvl" + level.ToString();
  }
  public void SetLevelTextWithAnim(int level)
  {
    levelText.transform.DOScale(scaleParams, scaleDuration).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo); 
    levelText.text = "Lvl" + level.ToString();
  }

  public void PlayRippleVFX()
  {
    rippleVFX.Play();
  }
  public void ShowAndHideLevelText(int level)
  {
    // 1. Початкові налаштування
    hideLevelText.text = "Level " + level.ToString();
    hideLevelText.fontSize = 30;

    // Запам'ятовуємо початкову локальну позицію, щоб точно повернутися назад
    Vector3 startPos = hideLevelText.transform.localPosition;

    // 2. Анімація польоту вгору по осі Y
    hideLevelText.transform.DOLocalMoveY(startPos.y + moveHideTextDistance, moveHideTextDuration)
        .SetEase(Ease.OutQuad) // Плавне сповільнення в кінці
        .OnComplete(() =>
        {
          // 3. Скидання після завершення
          hideLevelText.fontSize = 0;
          hideLevelText.transform.localPosition = startPos; // Повертаємо в точну початкову точку
        });
  }

}
