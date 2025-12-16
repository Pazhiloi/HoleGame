using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PlayerLevelUI : MonoBehaviour
{
  public TMP_Text levelText;
  public Vector3 scaleParams = new Vector3(2f, 2f,2f);
  public float scaleDuration = 0.1f;



  public void UpdateLevelText(int level)
  {
    levelText.transform.DOScale(scaleParams, scaleDuration).SetEase(Ease.Flash);
    levelText.text = "Lvl" + level.ToString();
  }
  
}
