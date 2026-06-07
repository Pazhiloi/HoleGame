using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
  [Header("Налаштування магніту")]
  [SerializeField] private float magnetRadius = 5f;       // Радіус дії магніту
  [SerializeField] private float pullSpeed = 8f;         // Швидкість притягування
  [SerializeField] private LayerMask enemyLayer;         // Шар (Layer) ворогів, щоб не перевіряти зайві об'єкти
  [Header("Ефекти Візуалізації (VFX)")]
  [SerializeField] private GameObject magnetVFXObject;   // Об'єкт з ефектом магніту
  private bool isActive = false;
  private float durationTimer = 0f;
  private ParticleSystem vfxParticleSystem;

  private void Awake()
  {
    // На старті гри перевіряємо, чи є на об'єкті система часток, щоб керувати нею коректно
    if (magnetVFXObject != null)
    {
      magnetVFXObject.SetActive(false); // Про всяк випадок вимикаємо на старті
    }
  }

  void Update()
  {
    if (!isActive) return;

    HandleMagnetDuration();
    PullEnemies();
  }

  // Метод для активації магніту (викликається з кнопки UI або менеджера бустерів)
  public void ActivateMagnet(float duration)
  {
    durationTimer = duration;
    isActive = true;
    Debug.Log($"Магніт активовано на {duration} сек!");
    // Вмикаємо VFX ефект
    if (magnetVFXObject != null)
    {
      magnetVFXObject.SetActive(true);
    }

    }

  private void HandleMagnetDuration()
  {
    if (durationTimer > 0)
    {
      durationTimer -= Time.deltaTime;
    }
    else
    {
      DeactivateMagnet();
    }
  }

  private void PullEnemies()
  {
    Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

    foreach (Enemy enemy in allEnemies)
    {
      // Перевіряємо відстань між гравцем і цим ворогом
      float distance = Vector3.Distance(transform.position, enemy.transform.position);

      // Якщо ворог увійшов у радіус дії магніту
      if (distance <= magnetRadius)
      {
        // Плавно тягнемо його до гравця
        enemy.transform.position = Vector3.MoveTowards(
            enemy.transform.position,
            transform.position,
            pullSpeed * Time.deltaTime
        );
      }
    }
  }

  private void DeactivateMagnet()
  {
    isActive = false;
    Debug.Log("Магніт вимкнено!");
    // Вимикаємо VFX ефект
    if (magnetVFXObject != null)
    {
      magnetVFXObject.SetActive(false);
    }
    }

  // Візуалізація радіусу магніту в редакторі Unity
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.position, magnetRadius);
  }
}
