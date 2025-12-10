using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Налаштування руху")]
  public float moveSpeed = 8f;           // швидкість переміщення
  public float rotationSpeed = 12f;      // як швидко повертається до напрямку руху (якщо ввімкнеш)
  public float arrivalDistance = 0.5f;   // на якій відстані вважати, що дійшли

  [Header("Клік мишею")]
  public LayerMask groundLayer = 1;      // шар землі, по якій клікаємо

  private Rigidbody rb;
  private Vector3 targetPosition;
  private Camera mainCam;

  // Додаємо прапорець: рухаємося ми до точки кліку чи ні
  private bool isMovingToClick = false;

  void Awake()
  {
    rb = GetComponent<Rigidbody>();
    mainCam = Camera.main;

    // Початкова позиція — сама себе
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
    if (Input.GetMouseButtonDown(0)) // ліва кнопка миші
    {
      Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
      {
        targetPosition = hit.point;
        targetPosition.y = transform.position.y; // залишаємо ту ж висоту
        isMovingToClick = true; // вмикаємо режим руху до кліку
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

    // Пріоритет: WASD, потім стрілки
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

    // Якщо є ввід з клавіатури — рухаємося в цьому напрямку і вимикаємо рух до кліку
    if (desiredDir != Vector3.zero)
    {
      targetPosition = transform.position + desiredDir * 100f; // дуже далеко вперед
      isMovingToClick = false; // клавіші мають пріоритет над кліком
    }
    else
    {
      // Якщо клавіші відпущені І ми не рухаємося до точки кліку — зупиняємося
      if (!isMovingToClick)
      {
        targetPosition = transform.position; // ціль = поточна позиція → зупинка
      }
    }
  }

  void MoveTowardsTarget()
  {
    Vector3 toTarget = targetPosition - transform.position;
    toTarget.y = 0;

    float distance = toTarget.magnitude;

    if (distance > arrivalDistance)
    {
      Vector3 direction = toTarget.normalized;

      // Рух
      Vector3 velocity = direction * moveSpeed;
      velocity.y = rb.velocity.y; // зберігаємо Y (гравітація тощо)
      rb.velocity = velocity;

      // Плавний поворот (якщо захочеш увімкнути — розкоментуй)
      // if (direction != Vector3.zero)
      // {
      //     Quaternion targetRot = Quaternion.LookRotation(direction);
      //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
      // }
    }
    else
    {
      // Дійшли до цілі або відпустили клавіші — зупиняємо горизонтальний рух
      rb.velocity = new Vector3(0, rb.velocity.y, 0);
      rb.angularVelocity = Vector3.zero;

      // Якщо це був рух до кліку — вимикаємо прапорець
      if (isMovingToClick)
      {
        isMovingToClick = false;
      }
    }
  }

  // Для компаса (стрілки UI) — повертає поточний напрямок руху
  public Vector3 GetCurrentMoveDirection()
  {
    Vector3 dir = targetPosition - transform.position;
    dir.y = 0;

    if (dir.sqrMagnitude > 0.1f)
      return dir.normalized;

    if (rb.velocity.sqrMagnitude > 0.1f)
    {
      Vector3 vel = rb.velocity;
      vel.y = 0;
      return vel.normalized;
    }

    return transform.forward; // або Vector3.forward, якщо ротація не використовується
  }

  // Дебаг
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(targetPosition, 0.5f);
    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, targetPosition + Vector3.up * 0.1f);
  }
}