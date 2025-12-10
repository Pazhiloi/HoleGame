using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
  private PlayerMovement player;
  public float rotationSpeed = 0.1f;
  private float targetAngle = 0f;
  private float currentAngle = 0f;
  private Vector3 lastDirection = Vector3.forward;

  private void Awake() {
    player = GetComponentInParent<PlayerMovement>();
  }

  void Update()
  {
    if (player == null) return;
    MoveArrow();
  }

  private void MoveArrow()
  {
    Vector3 direction = player.GetCurrentMoveDirection();

    // Якщо не рухаємося — плавно гальмуємо до останнього кута
    if (direction.sqrMagnitude < 0.01f)
    {
      currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
      ApplyRotation();
      return;
    }

    // Нормалізуємо напрямок
    direction.y = 0;
    direction.Normalize();

    // Новий цільовий кут
    float newTargetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    targetAngle = -newTargetAngle;

    // Плавний Lerp (Mathf.LerpAngle враховує найкоротший шлях, без стрибків!)
    currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

    ApplyRotation();
  }

  void ApplyRotation()
  {
    transform.localEulerAngles = new Vector3(0f, 0f, currentAngle);
  }
}
