using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScale : MonoBehaviour {
    [Header("Lerp scale")]
    public bool lerping;
    public float lerpSpeed;
    public Vector3 desiredScale;
    [HideInInspector]
    public Vector3 origScale;
    [Header("Scale at start")]
    public bool setScaleAtStart;
    public float startMultiplier;

    void Start()
    {
        origScale = transform.localScale;

        if (setScaleAtStart)
        {
            transform.localScale *= startMultiplier;
        }
    }

    void Update () {
        if (lerping)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * lerpSpeed);

            if (Vector3.Distance(transform.localScale, desiredScale) < 0.1f)
            {
                lerping = false;
            }
        }
    }

    //can be called from anywhere 
    public void SetScaler(float speed, Vector3 newScale)
    {
        desiredScale = newScale;
        lerpSpeed = speed;
        lerping = true;
    }
}
