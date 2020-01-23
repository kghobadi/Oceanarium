using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScale : MonoBehaviour {
    [Header("Lerp scale")]
    [Tooltip("Check this to use linear movetowards instead of Lerp")]
    public bool moveTowardsOrLerp;
    [Tooltip("These 3 variables get set when the Set Scaler function is called, do not set them")]
    public bool lerping;
    public float lerpSpeed;
    public Vector3 desiredScale;
    [Tooltip("Once size is this close to desired scale, stops")]
    public float distNecToStopLerp = 0.01f;
    [HideInInspector]
    public Vector3 origScale;
    [Header("Scale at start")]
    public bool setScaleAtStart;
    [Tooltip("This will scale the object by this factor, i.e. scale x 0.1f")]
    public float startMultiplier;
    [HideInInspector]
    public float distLeft;

    void Start()
    {
        origScale = transform.localScale;

        if (setScaleAtStart)
        {
            transform.localScale *= startMultiplier;
        }
    }

    void Update ()
    {
        if (lerping)
        {
            //move towards
            if (moveTowardsOrLerp)
            {
                //actually moving towards scale 
                transform.localScale = Vector3.MoveTowards(transform.localScale, desiredScale, Time.deltaTime * lerpSpeed);
            }
            //lerp
            else
            {
                //actually lerping scale 
                transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * lerpSpeed);
            }

            //makes this visible to other scripts 
            distLeft = Vector3.Distance(transform.localScale, desiredScale);

            //close enough to stop lerp 
            if (distLeft < distNecToStopLerp)
            {
                lerping = false;
                transform.localScale = desiredScale;
            }
        }
    }

    //for calling scale at start from another script 
    public void WaitToSetScale(float wait, float speed, Vector3 newScale)
    {
        StartCoroutine(WaitToScale(wait, speed, newScale));
    }

    //waits then sets scaler
    IEnumerator WaitToScale(float wait, float speed, Vector3 newScale)
    {
        yield return new WaitForSeconds(wait);

        SetScaler(speed, newScale);
    }

    //can be called from anywhere 
    public void SetScaler(float speed, Vector3 newScale)
    {
        desiredScale = newScale;
        lerpSpeed = speed;
        lerping = true;
    }
}
