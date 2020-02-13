using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repulsor : MonoBehaviour {
    GameObject player;
    PlayerController pc;

    Rigidbody rBody;
    GravityBody gravBody;
    Orbit orbit;
    DistanceAnimatorParameter distanceFromPlayer;
 
    public bool repulsing;
    public bool slowingDown;
    public bool repulseOnStart;
    [Tooltip("Force multiplier for repulsion")]
    public float repulsionForce;
    [Tooltip("Direction set by planter which spawns this, or player if player activates")]
    public Vector3 direction;
    [Tooltip("Max speed repulsor object will reach")]
    public float maxSpeed;

    [Tooltip("Desired distance at which repulsion stops, begins orbiting planet")]
    public float desiredDistFromPlanet;
    [Tooltip("Min and Max for desiredDist")]
    public float desiredDistFromPlanetMin = 30f, desiredDistFromPlanetMax = 60;

    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player)
            pc = player.GetComponent<PlayerController>();

        rBody = GetComponent<Rigidbody>();
        gravBody = GetComponent<GravityBody>();
        orbit = GetComponent<Orbit>();
        distanceFromPlayer = GetComponent<DistanceAnimatorParameter>();
    }

    void Start()
    {
        desiredDistFromPlanet = Random.Range(desiredDistFromPlanetMin, desiredDistFromPlanetMax);

        if(repulseOnStart)
        {
            SetRepulsor(direction);
        }
    }

    public void SetRepulsor(Vector3 dir)
    {
        direction = dir;
        slowingDown = false;
        repulsing = true;
    }

    void Update () {
        //repulsion
        if (repulsing)
        {
            Repulse();
        }
        //slow down logic 
        if (slowingDown)
        {
            rBody.velocity = Vector3.MoveTowards(rBody.velocity, Vector3.zero, repulsionForce * Time.deltaTime);

            if(Vector3.Distance(rBody.velocity, Vector3.zero) < 1f)
            {
                rBody.velocity = Vector3.zero;
                slowingDown = false;
            }
        }
        //if not repulse on start, waiting for player to repulse
        if (!repulseOnStart)
        {
            if(Vector3.Distance(transform.position, player.transform.position) < 10 && !repulsing)
            {
                SetRepulsor(player.GetComponent<GravityBody>().GetUp());
            }
        }
	}

    void Repulse ()
    {
        //actual force applied 
        if(rBody.velocity.magnitude < maxSpeed)
        {
            Vector3 force = direction * repulsionForce;
            
            rBody.AddForce(force);

            //Debug.Log("force = " + force);
        }
        
        //reached desired dist from planet 
        if (gravBody.distanceFromPlanet > desiredDistFromPlanet)
        {
            //activate orbit
            if (orbit)
            {
                orbit.SetOrbit(orbit.orbitalSpeed + Random.Range(0, 5f));
            }

            repulsing = false;
            slowingDown = true;
        }
    }
}
