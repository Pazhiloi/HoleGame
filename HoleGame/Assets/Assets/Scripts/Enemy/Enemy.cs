using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  public SphereCollider enemyCollider;
  public bool isConsumed = false;
  public int expforEnemy = 1;

  private void Awake() {
    enemyCollider = GetComponent<SphereCollider>();
  }
}
