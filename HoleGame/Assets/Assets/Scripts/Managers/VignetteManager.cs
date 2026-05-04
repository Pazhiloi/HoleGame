using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
// using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class VignetteManager : MonoBehaviour
{

  // Start is called before the first frame update

  private Volume volume;
  private Vignette vignette;

  [SerializeField] private float pulseDuration = 0.5f;
  [SerializeField] private float maxIntensity = 0.45f;
  public bool isActive = false;
  void Awake()
  {
    volume = GetComponent<Volume>();
    if (volume.profile.TryGet<Vignette>(out var v))
    {
      vignette = v;
      vignette.intensity.value = 0;
    }
  }



  public void StartVignettePulse()
  {
    if (vignette == null) return;

    // Встановлюємо червоний колір
    vignette.active = true;
    vignette.color.value = Color.red;

    // Анімуємо інтенсивність туди-сюди (Yoyo)
    DOTween.To(() => vignette.intensity.value,
               x => vignette.intensity.value = x,
               maxIntensity, pulseDuration)
           .SetLoops(-1, LoopType.Yoyo)
           .SetEase(Ease.InOutQuad)
           .SetId("VignettePulse"); // ID, щоб легко зупинити
  }

  public void StopVignette()
  {
    // Зупиняємо анімацію за ID
    DOTween.Kill("VignettePulse");

    // Плавно повертаємо до нуля
    DOTween.To(() => vignette.intensity.value,
               x => vignette.intensity.value = x,
               0f, 0.3f);
  }


}
