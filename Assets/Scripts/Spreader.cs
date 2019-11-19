using UnityEngine;
using System.Collections.Generic;

public class Spreader : MonoBehaviour {
  public List<GameObject> targets;

  public float repeat = 0;
  public float pathLength = 48;

  [Header("Position")]
  public AnimationCurve xCurve = AnimationCurve.Linear(0.0f, 1.0f, 0.0f, 0.0f);
  public float xCurveLength = 1;
  [Space(10)]
  public AnimationCurve yCurve = AnimationCurve.Linear(0.0f, 1.0f, 0.0f, 0.0f);
  public float yCurveLength = 1;

  [Header("Rotation")]
  public Vector3 finalRotation;

  [Header("Scale")]
  public AnimationCurve scaleCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

  [Header("Color")]
  public Gradient colorGradient;
  public float colorPhase = 0;
  public float colorFrequency = 1;
  public bool useColor = false;

private int targetIndex = 0;

  #if UNITY_EDITOR  
  void OnValidate() {
    UnityEditor.EditorApplication.delayCall += UpdateChildren;
  }
  #endif

  void UpdateChildren() {
    if (!Application.isEditor) return;
    if (Application.isPlaying) return;
    if (targets == null || targets.Count == 0) return;


    // we need to recycle all objects.
    if (transform.childCount != repeat) {
      while (transform.childCount > 0) {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
      for (int i = 0; i < repeat; i++) {
        GameObject g = Instantiate(targets[targetIndex]);
        targetIndex = (targetIndex + 1) % targets.Count;
        g.name = gameObject.name + " - " + i;
        g.transform.parent = transform;
      }
    }

    for (int i = 0; i < transform.childCount; ++i) {
      float frac = (float)i / repeat;
      GameObject obj = transform.GetChild(i).gameObject;

      obj.transform.localPosition = new Vector3(
        xCurveLength * xCurve.Evaluate(frac),
        yCurveLength * yCurve.Evaluate(frac),
        frac * pathLength);
      float scale = scaleCurve.Evaluate(frac);
      obj.transform.localScale = new Vector3(scale, scale, scale);
      obj.transform.localRotation = Quaternion.Euler(finalRotation * frac);

      if (useColor) {
        var tempMaterial = new Material(obj.GetComponent<Renderer>().sharedMaterial);
        tempMaterial.color = colorGradient.Evaluate((frac*colorFrequency+colorPhase)%1.0f);
        obj.GetComponent<Renderer>().sharedMaterial = tempMaterial;
      }
    }
  }
}
