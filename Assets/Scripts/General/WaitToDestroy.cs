using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script just allows you to destroy an object after waitTime
public class WaitToDestroy : MonoBehaviour {

    public float waitTime;

	void Start () {
        StartCoroutine(WaitToDestroyThis());	
	}

    public IEnumerator WaitToDestroyThis()
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }
}
