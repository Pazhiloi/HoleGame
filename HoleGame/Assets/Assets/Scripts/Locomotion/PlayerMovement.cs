using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Налаштування руху")]
  public float moveSpeed = 8f;           // швидкість переміщення
  public float rotationSpeed = 12f;      // як швидко повертається до напрямку руху
  public float arrivalDistance = 0.5f;   // на якій відстані вважати, що дійшли

  [Header("Клік мишею")]
  public LayerMask groundLayer = 1;      // шар землі, по якій клікаємо

  private Rigidbody rb;
  private Vector3 targetPosition;
  private Camera mainCam;
  private PlayerArrow arrow;

  void Awake()
  {
    arrow = GetComponentInChildren<PlayerArrow>();
    rb = GetComponent<Rigidbody>();
    mainCam = Camera.main;

    // Починаємо з поточної позиції (щоб не їхала нікуди)
    targetPosition = transform.position;
  }

  void Update()
  {
    HandleMouseClick();
    HandleKeyboardInput();
  }

  void FixedUpdate()
  {
    MoveTowardsTarget();
  }

  void HandleMouseClick()
  {
    if (Input.GetMouseButtonDown(0)) // ліва кнопка
    {
      Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
      {
        targetPosition = hit.point;
        targetPosition.y = transform.position.y; // залишаємо ту ж висоту
      }
    }
  }

  void HandleKeyboardInput()
  {
    // WASD — рух відносно камери
    Vector3 inputWASD = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    // Стрілки — рух по світових осях
    Vector3 inputArrows = Vector3.zero;
    if (Input.GetKey(KeyCode.LeftArrow)) inputArrows.x -= 1;
    if (Input.GetKey(KeyCode.RightArrow)) inputArrows.x += 1;
    if (Input.GetKey(KeyCode.UpArrow)) inputArrows.z += 1;
    if (Input.GetKey(KeyCode.DownArrow)) inputArrows.z -= 1;

    Vector3 desiredDir = Vector3.zero;

    // Пріоритет: спочатку WASD (як у нормальних іграх), потім стрілки
    if (inputWASD.sqrMagnitude > 0.01f)
    {
      Vector3 camForward = mainCam.transform.forward;
      Vector3 camRight = mainCam.transform.right;
      camForward.y = 0; camRight.y = 0;
      camForward.Normalize(); camRight.Normalize();

      desiredDir = (camForward * inputWASD.z + camRight * inputWASD.x).normalized;
    }
    else if (inputArrows.sqrMagnitude > 0.01f)
    {
      desiredDir = new Vector3(inputArrows.x, 0, inputArrows.z).normalized;
    }

    // Якщо є ввід з клавіатури — постійно оновлюємо ціль далеко вперед
    if (desiredDir != Vector3.zero)
    {
      targetPosition = transform.position + desiredDir * 100f;
    }
  }

  void MoveTowardsTarget()
  {
    Vector3 toTarget = targetPosition - transform.position;
    toTarget.y = 0; // рухаємося тільки по X/Z

    float distance = toTarget.magnitude;

    if (distance > arrivalDistance)
    {
      Vector3 direction = toTarget.normalized;

      // Рух (в FixedUpdate — через фізику)
      Vector3 velocity = direction * moveSpeed;
      velocity.y = rb.velocity.y; // зберігаємо вертикальну швидкість (падіння тощо)
      rb.velocity = velocity;

      // Плавний поворот у напрямку руху (щоб діра котилася правильно)
      // if (direction != Vector3.zero)
      // {
      //   Quaternion targetRot = Quaternion.LookRotation(direction);
      //   arrow.transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
      // }
    }
    else
    {
      // Дійшли — гальмуємо
      rb.velocity = new Vector3(0, rb.velocity.y, 0);
      rb.angularVelocity = Vector3.zero;
    }
  }

  // Дебаг: бачимо куди їде діра
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(targetPosition, 0.5f);
    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, targetPosition + Vector3.up * 0.1f);
  }
}
