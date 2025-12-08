using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
  public Transform target;          // сюди перетягни свій об'єкт-діру
  public float followSpeed = 10f;    // швидкість слідування (плавність)
  public float height = 8f;         // висота над дірою (Y)
  public float backOffset = -8f;    // відстань ззаду (Z, відносно напрямку діри)
  public float cameraYAngle = 45f;

  void LateUpdate() 
  {
    if (target == null) return;

    transform.position = target.position;
  }
  
}
