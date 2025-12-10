using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
  private PlayerMovement player;
  public float rotationSpeed;

  private void Awake() {
    player = GetComponentInParent<PlayerMovement>();
  }

  void Update()
  {
    if (player == null) return;

    // Обертаємо ВСЮ обгортку НАПРОТИК напрямку гравця (Y-ротація)
    // Стрілка всередині фіксована (вістрям "вгору" = Z=0)
    transform.localEulerAngles = new Vector3(0f, 0f, -player.transform.eulerAngles.y);
  }
}
