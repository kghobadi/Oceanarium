using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeScale : MonoBehaviour {
    Vector3 origScale;
    public float scaleMin, scaleMax;

	void Start ()
    {
        origScale = transform.localScale;

        RandomScale();
	}
	
	public void RandomScale()
    {
        float randomScale = Random.Range(scaleMin, scaleMax);
        transform.localScale *= randomScale;
    }

    public void SetToOrigScale()
    {
        transform.localScale = origScale;
    }
}
