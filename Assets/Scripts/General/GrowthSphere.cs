using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the growth sphere effect which emerges from Homing Pearls when they are activated 
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

    //grows sphere outward and on impact will grow other objects for the Homing Pearl parent 
    public void GrowObjects(float speedToScaleOthers)
    {
        objScaleSpeed = speedToScaleOthers;
        sRenderer.enabled = true;
        scaler.SetScaler(scaleSpeed, transform.localScale * radiusMultiplier);
        growing = true;
    }

    //resets growth sphere to original size
    void ResetSphere()
    {
        transform.localScale = scaler.origScale;
        sRenderer.enabled = false;
        growing = false;
    }
    
    void OnTriggerEnter(Collider other)
    { 
        //if object is a prop 
        if (other.gameObject.tag == "Prop")
        {
            //if prop has a Lerp scale component for growing 
            if (other.gameObject.GetComponent<LerpScale>())
            {
                LerpScale scalerObj = other.gameObject.GetComponent<LerpScale>();
                if (scalerObj.setScaleAtStart)
                {
                    scalerObj.SetScaler(objScaleSpeed, scalerObj.origScale);
                    scalerObj.setScaleAtStart = false;
                }
            }
            //trigger grow anim
            if (other.gameObject.GetComponent<Animator>())
            {
                other.gameObject.GetComponent<Animator>().SetTrigger("grow");
            }
        }
    }

}
