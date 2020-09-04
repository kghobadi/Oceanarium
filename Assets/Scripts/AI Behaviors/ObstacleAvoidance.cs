using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour {
    MoveTowards movement;
    Orbit orbital;
    Rigidbody rBody;
    GravityBody grav;
    
    public Transform travelDest;
    public LayerMask obstacles;
    public float elevationSpeed = 12f;
    public float maxHeight = 25f;

    public bool usesFwdThrust;
    
    void Awake ()
    {
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        rBody = GetComponent<Rigidbody>();
        grav = GetComponent<GravityBody>();
	}
	
	void Update ()
    {
        //avoid in accordance w move towards
        if (movement)
        {
            if (movement.moving)
            {
                CheckForward();
            }
        }

        //avoid in accordance w orbital
        if (orbital)
        {
            if (orbital.orbiting)
            {
                CheckForward();
            }
        }
	}

    //shoots ray forward looking for obstacles 
    void CheckForward()
    {
        Vector3 direction;
        if (travelDest)
            direction = travelDest.position - transform.position;
        else
            direction = movement.destination - transform.position;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1f, direction, out hit, 10f, obstacles))
        {
            //elevate if hit    
            //Debug.Log("hit");
            if (grav.distanceFromPlanet < maxHeight - 2f)
            {
                if (usesFwdThrust)
                    ForwardThrust();
                else
                    Elevate();
            }
            else if (grav.distanceFromPlanet > maxHeight + 2f)
            {
                SideStep();
            }

        }
    }

    //called when ray forward hits obstacle
    void Elevate()
    {
        rBody.AddForce(grav.GetUp() * elevationSpeed);

        Debug.Log("Elevating");
    }

    //called when ray forward hits obstacle
    void ForwardThrust()
    {
        Vector3 fwd = transform.forward * elevationSpeed;
        rBody.AddForce(fwd);

        Debug.Log("fwd thrusting");
    }

    //called when ray forward hits obstacle
    void SideStep()
    {
        Vector3 right = transform.right * elevationSpeed;
        rBody.AddForce(right);
        
        Debug.Log("side stepping");
    }
}
