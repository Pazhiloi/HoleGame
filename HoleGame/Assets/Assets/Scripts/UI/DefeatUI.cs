using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
  [SerializeField] private GameObject defeatPanel;

  [SerializeField] private CanvasGroup canvasGroupDefeat; // Додай CanvasGroup на VictoryPanel для плавного проявлення

  [SerializeField] private RectTransform defeatText;
  [SerializeField] private GameObject ButtonsListParent;
  [SerializeField] private Timer timer;


  private void Awake() {
    HandleDefeatPanel();
    ButtonsOff();
  }



  private void HandleDefeatPanel()
  {
    defeatPanel.SetActive(false);
    if (canvasGroupDefeat != null) canvasGroupDefeat.alpha = 0;
  }

  public void ShowDefeatScreen()
  {
    defeatPanel.SetActive(true);
    canvasGroupDefeat?.DOFade(1f, 0.5f);
    if (defeatText != null)
    {
      defeatText.localScale = Vector3.zero;
      defeatText.DOScale(Vector3.one, 1.6f).SetEase(Ease.OutBack);
    }
    ButtonsOn();
  }

  private void ButtonsOff()
  {
    var buttons = ButtonsListParent.GetComponentsInChildren<Button>();

    foreach (var button in buttons)
    {
      button.gameObject.SetActive(false);
    }
  }
  private void ButtonsOn()
  {
    var buttons = ButtonsListParent.GetComponentsInChildren<Button>(true);
    float delay = 0f;

    foreach (var button in buttons)
    {
      Debug.Log("Знайдено кнопку: " + button.name); 
      button.transform.localScale = Vector3.zero;
      button.gameObject.SetActive(true);
      button.transform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack).SetDelay(delay);
      delay += 0.2f;
    }
  }


  public  void ContinueForMoney()
  {
    Debug.Log("ContinueForMoney");
    ProcessContinue();
  }
  public  void ContinueForAds()
  {
    Debug.Log("ContinueForAds");
    ProcessContinue();
  }
  public void RetryButton()
  {
    Debug.Log("RetryButton");
    TestManager.Instance.RestartLevel();
  }

  private void ProcessContinue()
  {
    // 1. Додаємо час (наприклад 60 секунд)
    timer.AddExtraTime(15f);

    // 2. Закриваємо панель з анімацією
    HideDefeatScreen();
  }

  private void HideDefeatScreen()
  {
    // Плавне зникнення панелі
    canvasGroupDefeat?.DOFade(0f, 0.5f).OnComplete(() =>
    {
      defeatPanel.SetActive(false);
      ButtonsOff(); // Ховаємо кнопки для наступного разу
    });

    // Анімація тексту (зменшення)
    defeatText?.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
  }



}
