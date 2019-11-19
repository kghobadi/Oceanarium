using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is used to randomly gen different fog particle types
public class FogGenerator : MonoBehaviour {

    ParticleSystem fogger;
    public Material[] fogMaterials;

	void Start () {
        fogger = GetComponent<ParticleSystem>();

        FogIt();
	}

    //randomly assign one of the fog materials to this particle system
    public void FogIt()
    {
        int randomSmoker = Random.Range(0, fogMaterials.Length);

        fogger.GetComponent<Renderer>().material = fogMaterials[randomSmoker];
    }
	
}
