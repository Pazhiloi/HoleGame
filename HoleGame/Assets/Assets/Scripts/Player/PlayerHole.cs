using UnityEngine;

public class PlayerHole : MonoBehaviour
{
  [Header("Налаштування")]
  [SerializeField] private float baseRadius = 1.5f;
  [SerializeField] private float suctionSpeed = 8f;
  [SerializeField] private float fallSpeed = 5f;
  [SerializeField] private float shrinkSpeed = 2f;

  private PlayerStats playerStats;

  private void Awake()
  {
    playerStats = GetComponent<PlayerStats>();
  }

  public float CurrentRadius => baseRadius * transform.localScale.x;

  private void OnTriggerStay(Collider other)
  {
    if (!other.CompareTag("Enemy")) return;

    float distance = GetHorizontalDistance(other.transform.position);

    if (distance < CurrentRadius)
    {
      ProcessFalling(other.gameObject, distance);
    }
  }

  private void ProcessFalling(GameObject enemyObj, float distance)
  {
    Enemy enemy = enemyObj.GetComponent<Enemy>();
    if (enemy == null) return;

    // 1. Підготовка фізики (тільки один раз або щокадру для стабілізації)
    PrepareEnemyForSucking(enemyObj, enemy);

    // 2. Горизонтальне всмоктування до центру
    MoveToHoleCenter(enemyObj);

    // 3. Вертикальне падіння та поглинання
    if (distance < CurrentRadius * 0.5f)
    {
      HandleSinking(enemyObj, enemy);
    }
  }

  // --- ДОПОМІЖНІ МЕТОДИ ---

  private float GetHorizontalDistance(Vector3 enemyPos)
  {
    Vector3 holePos = transform.position;
    return Vector2.Distance(
        new Vector2(holePos.x, holePos.z),
        new Vector2(enemyPos.x, enemyPos.z)
    );
  }

  private void PrepareEnemyForSucking(GameObject enemyObj, Enemy enemy)
  {
    if (enemy.enabled) enemy.enabled = false;

    Rigidbody rb = enemyObj.GetComponent<Rigidbody>();
    if (rb != null && !rb.isKinematic)
    {
      rb.isKinematic = true;
      rb.velocity = Vector3.zero;
    }

    Collider col = enemyObj.GetComponent<Collider>();
    if (col != null && !col.isTrigger)
    {
      col.isTrigger = true;
    }
  }

  private void MoveToHoleCenter(GameObject enemyObj)
  {
    Vector3 targetCenter = new Vector3(transform.position.x, enemyObj.transform.position.y, transform.position.z);
    enemyObj.transform.position = Vector3.MoveTowards(
        enemyObj.transform.position,
        targetCenter,
        suctionSpeed * Time.deltaTime
    );
  }

  private void HandleSinking(GameObject enemyObj, Enemy enemy)
  {
    // Падіння вниз
    enemyObj.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

    // Зменшення
    enemyObj.transform.localScale = Vector3.MoveTowards(
        enemyObj.transform.localScale,
        Vector3.zero,
        shrinkSpeed * Time.deltaTime
    );

    // Реєстрація поглинання
    if (!enemy.isConsumed)
    {
      RegisterEnemyConsumption(enemy);
    }

    // Видалення
    if (ShouldDestroy(enemyObj))
    {
      Destroy(enemyObj);
    }
  }

  private void RegisterEnemyConsumption(Enemy enemy)
  {
    enemy.isConsumed = true;
    playerStats.AddXP(enemy.expforEnemy);

    if (enemy.isReqForVictory)
    {
      VictoryManager.Instance.AnimateEnemyCollection(transform.position, enemy.enemyType);
    }
  }

  private bool ShouldDestroy(GameObject enemyObj)
  {
    return enemyObj.transform.localScale.x < 0.05f ||
           enemyObj.transform.position.y < transform.position.y - 3f;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, CurrentRadius);
  }
}