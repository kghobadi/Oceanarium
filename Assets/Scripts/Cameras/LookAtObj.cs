using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//makes an object look at a transform with proper gravity up
[RequireComponent(typeof(GravityBody))]
public class LookAtObj : MonoBehaviour {
    PlayerController pc;
    public Transform lookAt;
    public bool lookAtPlayer;
    GravityBody gravBody;

    public bool usesGrav;
    
	void Awake ()
    {
        pc = FindObjectOfType<PlayerController>();
        gravBody = GetComponent<GravityBody>();

        if (lookAtPlayer)
        {
            lookAt = pc.transform;
        }
	}
	
	void Update ()
    {
        if (usesGrav)
            transform.LookAt(lookAt, gravBody.GetUp());
        else
            transform.LookAt(lookAt);
	}
}
