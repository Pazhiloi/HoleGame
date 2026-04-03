using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  public SphereCollider enemyCollider;

  private void Awake() {
    enemyCollider = GetComponent<SphereCollider>();
  }
}
