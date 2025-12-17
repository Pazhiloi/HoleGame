using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class PlayerLevelUI : MonoBehaviour
{
  public TMP_Text levelText;
  public Vector3 scaleParams = new Vector3(2f,2f,2f);
  public float scaleDuration = 0.1f;
  public ParticleSystem rippleVFX;



  public void SetLevelText(int level)
  {
    levelText.text = "Lvl" + level.ToString();
  }
  public void SetLevelTextWithAnim(int level)
  {
    levelText.transform.DOScale(scaleParams, scaleDuration).SetEase(Ease.Flash).SetLoops(2, LoopType.Yoyo); 
    levelText.text = "Lvl" + level.ToString();
  }

  public void PlayRippleVFX()
  {
    rippleVFX.Play();
  }
  
}
