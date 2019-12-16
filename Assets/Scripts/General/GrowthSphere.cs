using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthSphere : MonoBehaviour {
    MeshRenderer sRenderer;
    LerpScale scaler;
    public bool growing;
    public float scaleSpeed = 0.5f;
    float objScaleSpeed;
    public float radiusMultiplier = 10f;

    void Awake()
    {
        sRenderer = GetComponent<MeshRenderer>();
        scaler = GetComponent<LerpScale>();
    }

    void Start ()
    {
        sRenderer.enabled = false;
	}
    
    void Update()
    {
        if (growing)
        {
            if(scaler.lerping == false)
            {
                ResetSphere();
            }
        }
    }

    public void GrowObjects(float speedToScaleOthers)
    {
        objScaleSpeed = speedToScaleOthers;
        sRenderer.enabled = true;
        scaler.SetScaler(scaleSpeed, transform.localScale * radiusMultiplier);
        growing = true;
    }

    void ResetSphere()
    {
        transform.localScale = scaler.origScale;
        sRenderer.enabled = false;
        growing = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<LerpScale>())
        {
            LerpScale scalerObj = other.gameObject.GetComponent<LerpScale>();
            if (scalerObj.setScaleAtStart)
            {
                scalerObj.SetScaler(objScaleSpeed, scalerObj.origScale);
                scalerObj.setScaleAtStart = false;
            }
        }
    }

}
