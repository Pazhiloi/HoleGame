using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHole : MonoBehaviour
{
  [SerializeField] private float captureRadius = 0.5f; // Наскільки близько до центру має бути ворог, щоб впасти
  [SerializeField] private float suckSpeed = 10f;     // Швидкість затягування в центр
  private List<Enemy> enemiesInZone = new List<Enemy>();



  private void Update() {
    WorkWithEnemyList();
  }
  private void OnTriggerEnter(Collider other) {
    Enemy enemy = other.GetComponent<Enemy>();

    if (enemy != null && !enemiesInZone.Contains(enemy))
    {
      enemiesInZone.Add(enemy);
    }
    }

  private void OnTriggerExit(Collider other)
  {
    Enemy enemy = other.GetComponent<Enemy>();
    if (enemy != null)
    {
      enemiesInZone.Remove(enemy);
    }
  }


  private void WorkWithEnemyList()
  {
    for (int i = enemiesInZone.Count - 1; i >= 0; i--)
    {
      Enemy enemy = enemiesInZone[i];
      if (enemy == null) { enemiesInZone.RemoveAt(i); continue; }

      // Рахуємо дистанцію тільки по горизонталі (X та Z), ігноруючи висоту Y
      float distance = Vector2.Distance(
          new Vector2(transform.position.x, transform.position.z),
          new Vector2(enemy.transform.position.x, enemy.transform.position.z)
      );

      // Якщо ворог ще не падає, але він у зоні — підтягуємо його до центру
      if (distance > captureRadius)
      {
        // Ефект магніту: тягнемо ворога до центру дірки
        Vector3 targetPos = new Vector3(transform.position.x, enemy.transform.position.y, transform.position.z);
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPos, suckSpeed * Time.deltaTime);
      }
      else
      {
        // Тільки коли він чітко над центром — він падає!
        StartFalling(enemy);
        enemiesInZone.RemoveAt(i);
      }
    }
  }

  private void StartFalling(Enemy enemy)
  {
    // Вимикаємо йому можливість ходити (AI)
    enemy.enabled = false;

    // Робимо його тригером, щоб він провалився
    enemy.enemyCollider.isTrigger = true;

    // Знищуємо об'єкт через 2 секунди після падіння
    Destroy(enemy.gameObject, 2f);
  }
}
