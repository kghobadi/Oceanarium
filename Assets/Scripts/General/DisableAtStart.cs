using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtStart : MonoBehaviour {

	void Start(){
		gameObject.SetActive (false);
	}
}
