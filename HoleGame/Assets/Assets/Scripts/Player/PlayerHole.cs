using UnityEngine;

public class PlayerHole : MonoBehaviour
{
  [Header("Налаштування")]
  [SerializeField] private float baseRadius = 1.5f; // Радіус при scale (1,1,1)
  [SerializeField] private float suctionSpeed = 8f;
  [SerializeField] private float fallSpeed = 5f;
  [SerializeField] private float shrinkSpeed = 2f;

  public PlayerStats playerStats;

  private void Awake() {
    playerStats = GetComponent<PlayerStats>();
  }

  // Геттер для отримання реального радіусу в реальному часі
  public float CurrentRadius => baseRadius * transform.localScale.x;

  private void OnTriggerStay(Collider other)
  {
    if (other.CompareTag("Enemy"))
    {
      Vector3 holeCenter = transform.position;
      Vector3 enemyPos = other.transform.position;

      // Рахуємо дистанцію в 2D (горизонтальна площина)
      float distance = Vector2.Distance(
          new Vector2(holeCenter.x, holeCenter.z),
          new Vector2(enemyPos.x, enemyPos.z)
      );

      // Використовуємо динамічний радіус замість статичного
      if (distance < CurrentRadius)
      {
        ProcessFalling(other.gameObject, distance);
      }
    }
  }

  private void ProcessFalling(GameObject enemyObj, float distance)
  {
    // Зупиняємо логіку ворога
    Enemy enemyScript = enemyObj.GetComponent<Enemy>();
    if (enemyScript != null) enemyScript.enabled = false;

    Rigidbody rb = enemyObj.GetComponent<Rigidbody>();
    if (rb != null)
    {
      // rb.isKinematic = true;
      rb.velocity = Vector3.zero;
    }

    // Центрування
    Vector3 targetCenter = new Vector3(transform.position.x, enemyObj.transform.position.y, transform.position.z);
    enemyObj.transform.position = Vector3.MoveTowards(enemyObj.transform.position, targetCenter, suctionSpeed * Time.deltaTime);

    // Початок падіння (коли ворог зайшов глибше ніж на половину поточного радіусу)
    if (distance < CurrentRadius * 0.5f)
    {
      enemyObj.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

      // Зменшуємо ворога відносно його початкового розміру
      enemyObj.transform.localScale = Vector3.Lerp(enemyObj.transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);
      if (!enemyScript.isConsumed)
      {
        playerStats.AddXP(enemyScript.expforEnemy);
        enemyScript.isConsumed = true;
        // Debug.Log("add ststs");
        if (enemyScript.isReqForVictory)
        {
          VictoryManager.Instance.AnimateEnemyCollection(transform.position);
        }
      }


      if (enemyObj.transform.localScale.x < 0.05f || enemyObj.transform.position.y < transform.position.y - 3f)
      {
        Destroy(enemyObj);
      }
    }
  }

  // Для візуального контролю в редакторі
  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, CurrentRadius);
  }
}