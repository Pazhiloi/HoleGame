using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBooster : MonoBehaviour
{
  [Header("Increase Scale")]
  public float increaseScale = 1.1f;
  public float scaleTransitionDuration = 0.5f;
  public float scaleDuration = 10f;
  private Vector3 _originalScale;
  private Coroutine _growthCoroutine;

  [Header("Increase Speed")]
  public float speedBoostScale = 2f;
  public float speedDuration = 10f;
  private PlayerMovement playerMovement;
  private float _baseSpeed;
  private Coroutine _speedBoostCoroutine;
  [Header("Magnet Settings")]
  [SerializeField] private Magnet playerMagnet;
  [SerializeField] private float  magnetDuration = 10f;


  void Awake()
  {
    _originalScale = transform.localScale;
    playerMovement = GetComponent<PlayerMovement>();
    _baseSpeed = playerMovement.moveSpeed;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      OnScaleButtonClick();
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      OnSpeedButtonClick();
    }
    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      OnMagnetButtonClick();
    }
    
  }

  private void OnMagnetButtonClick()
  {
    playerMagnet.ActivateMagnet(magnetDuration);
  }

  public void OnScaleButtonClick()
  {
    // Якщо анімація вже йде, зупиняємо її, щоб вони не конфліктували
    if (_growthCoroutine != null)
    {
      StopCoroutine(_growthCoroutine);
    }

    // Запускаємо нову корутину
    _growthCoroutine = StartCoroutine(GrowAndShrinkRoutine());
  }

  public void OnSpeedButtonClick()
  {
    // Якщо бустер вже активований, ми його перезапускаємо (оновлюємо час)
    if (_speedBoostCoroutine != null)
    {
      StopCoroutine(_speedBoostCoroutine);
    }

    _speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine());
  }











  IEnumerator GrowAndShrinkRoutine()
  {
    Vector3 targetScale = _originalScale * increaseScale; // +10%
    float transitionDuration = scaleTransitionDuration; // тривалість самого збільшення/зменшення

    // 1. Плавно збільшуємо (аналог DOScale)
    float elapsed = 0;
    while (elapsed < transitionDuration)
    {
      elapsed += Time.deltaTime;
      transform.localScale = Vector3.Lerp(_originalScale, targetScale, elapsed / transitionDuration);
      yield return null; // чекаємо наступного кадру
    }
    transform.localScale = targetScale; // фіксуємо точний розмір

    // 2. Чекаємо 10 секунд
    yield return new WaitForSeconds(scaleDuration);

    // 3. Плавно повертаємо назад
    elapsed = 0;
    while (elapsed < transitionDuration)
    {
      elapsed += Time.deltaTime;
      transform.localScale = Vector3.Lerp(targetScale, _originalScale, elapsed / transitionDuration);
      yield return null;
    }
    transform.localScale = _originalScale; // повертаємо точний оригінал

    _growthCoroutine = null;
  }


  IEnumerator SpeedBoostRoutine()
  {
    // 1. Збільшуємо швидкість у 2 рази
    playerMovement.moveSpeed = _baseSpeed * speedBoostScale;
    Debug.Log("Бустер активовано! Швидкість: " + playerMovement.moveSpeed);

    // 2. Чекаємо 10 секунд
    yield return new WaitForSeconds(speedDuration);

    // 3. Повертаємо початкову швидкість
    playerMovement.moveSpeed = _baseSpeed;
    Debug.Log("Дія бустера завершена. Швидкість: " + playerMovement.moveSpeed);

    _speedBoostCoroutine = null;
  }
}
