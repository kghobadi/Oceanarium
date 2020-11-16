using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLookAt : MonoBehaviour {

	public Transform lookAt;
	
	void Update () 
	{
		if(lookAt)
			transform.LookAt(lookAt);
	}
}
