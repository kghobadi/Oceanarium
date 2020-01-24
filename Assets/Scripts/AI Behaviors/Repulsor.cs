using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulsor : MonoBehaviour {
    GravityBody gravBody;
    Rigidbody rBody;
    Orbit orbit;
    DistanceAnimatorParameter distanceFromPlayer;
 
    public bool repulsing;
    public bool repulseOnStart;
    public float repulsionForce;
    public float maxSpeed;
    public float desiredDistFromPlanet = 30f;

	void Awake ()
    {
        gravBody = GetComponent<GravityBody>();
        rBody = GetComponent<Rigidbody>();
        orbit = GetComponent<Orbit>();
        distanceFromPlayer = GetComponent<DistanceAnimatorParameter>();
    }
	
	void FixedUpdate () {
        if (repulsing)
        {
            Repulse();
        }
	}

    void Repulse ()
    {
        if(rBody.velocity.magnitude < maxSpeed)
            rBody.AddForce(-gravBody.GetFutureUp() * repulsionForce);

        //reached desired dist from planet 
        if(gravBody.distanceFromPlanet > desiredDistFromPlanet)
        {
            rBody.velocity = Vector3.zero;
            repulsing = false;

            //activate orbit
        }
    }
}
