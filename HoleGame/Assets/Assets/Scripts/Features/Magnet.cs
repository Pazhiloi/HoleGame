using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
  [Header("Налаштування магніту")]
  [SerializeField] private float magnetRadius = 5f;       // Радіус дії магніту
  [SerializeField] private float pullSpeed = 8f;         // Швидкість притягування
  [SerializeField] private LayerMask enemyLayer;         // Шар (Layer) ворогів, щоб не перевіряти зайві об'єкти

  private bool isActive = false;
  private float durationTimer = 0f;

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
    // Знаходимо всіх коллайдерів ворогів у радіусі магніту
    Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, magnetRadius, enemyLayer);

    foreach (Collider2D enemyCollider in enemies)
    {
      // Тягнемо ворога до позиції гравця
      Transform enemyTransform = enemyCollider.transform;

      // Плавно переміщуємо ворога до гравця
      enemyTransform.position = Vector3.MoveTowards(
          enemyTransform.position,
          transform.position,
          pullSpeed * Time.deltaTime
      );
    }
  }

  private void DeactivateMagnet()
  {
    isActive = false;
    Debug.Log("Магніт вимкнено!");
  }

  // Візуалізація радіусу магніту в редакторі Unity
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.cyan;
    Gizmos.DrawWireSphere(transform.position, magnetRadius);
  }
}
