using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomMaterial : MonoBehaviour {

    public Material[] materialOptions;
    MeshRenderer mRender;

    void Awake()
    {
        mRender = GetComponent<MeshRenderer>();
        if(mRender == null)
        {
            mRender = GetComponentInChildren<MeshRenderer>();
        }
    }

    void Start ()
    {
        AssignMaterial(materialOptions);
	}
	
	public void AssignMaterial(Material[] mats)
    {
        int randomMat = Random.Range(0, mats.Length);

        mRender.material = mats[randomMat];
    }
}
