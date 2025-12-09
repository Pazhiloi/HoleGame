using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
  public float rotationSpeed;
   
public void RotateArrowTowardsTarget(Transform target)
  {
    var fromRotation = transform.rotation;
    var toRotation = Quaternion.LookRotation(target.position - transform.position);
    var speed  =rotationSpeed * Time.deltaTime;

    transform.rotation = Quaternion.Slerp(fromRotation, toRotation, speed);
  }
}
