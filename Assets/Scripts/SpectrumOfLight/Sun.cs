using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour {

    GameObject planet;

	void Start () {
        planet = GameObject.FindGameObjectWithTag("Planet");
	}

	void Update () {
        transform.LookAt(planet.transform.position);
	}
}
