using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMaterial : MonoBehaviour {
    [Header("Material values")]
    [Tooltip("MeshRenderer containing lerp material")]
    public MeshRenderer mRenderer;
    [Tooltip("Mat/shader to lerp")]
    public Material lerpMat;
    [Tooltip("Name of float to lerp in mat/shader")]
    public string floatToLerp = "_Teleport";
   
    [Header("Lerp values")]
    [Tooltip("Check this to lerp on start")]
    public bool lerpOnStart;
    [Tooltip("True when lerping mat val")]
    public bool lerpingMat;
    [Tooltip("Check this to disable renderer at the end")]
    public bool disableOnFinish;
    [Tooltip("Start value = floatToLerp when Lerp is called")]
    public float startValue;
    [Tooltip("Value actively lerping to set mats float towards end value")]
    public float lerpValue;
    [Tooltip("Set publicly or when passed when Lerp is called")]
    public float endValue;
    [Tooltip("Speed of lerping value ")]
    public float lerpSpeed = 0.5f;


    //do we want to lerp on start?
    void Start ()
    {
        if (lerpOnStart)
        {
            Lerp(endValue, lerpSpeed);
        }
	}

    //called to reset script values and then begin lerping
    public void SetLerpValues(MeshRenderer renderer, Material mat, string floatName)
    {
        mRenderer = renderer;
        lerpMat = mat;
        floatToLerp = floatName;

        lerpingMat = true;
    }

    //call to begin lerp 
    public void Lerp(float desiredValue, float speed)
    {
        lerpMat = mRenderer.material;
        startValue = lerpMat.GetFloat(floatToLerp);
        endValue = desiredValue;
        lerpSpeed = speed;
        

        lerpingMat = true;
    }
	
	void Update ()
    {
        //lerp is under way!
        if (lerpingMat)
        {
            //lerp mat Tp value 
            lerpValue = Mathf.Lerp(lerpValue, endValue, Time.deltaTime * lerpSpeed);
            //set float to lerp val
            lerpMat.SetFloat(floatToLerp, lerpValue);
            //dist by abs value of lerpVal subtracted from end val
            float dist = Mathf.Abs(endValue - lerpValue);

            //close enough, let's finish im
            if (dist < 0.1f)
            {
                //hard set float to end value
                lerpMat.SetFloat(floatToLerp, endValue);
                //stop lerping
                lerpingMat = false;
                //disable renderer when we finish!
                if (disableOnFinish)
                {
                    DisableRenderer();
                }
            }
        }

    }
    
    //can be called to disable renderer
    public void DisableRenderer()
    {
        mRenderer.enabled = false;
    }
}
