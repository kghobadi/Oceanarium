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
                CheckDest();
            }
        }

        //avoid in accordance w orbital
        if (orbital)
        {
            if (orbital.orbiting)
            {
                //pos X
                if(orbital.xyOrzAxis == Orbit.Axes.X && orbital.orbitalSpeed > 0)
                    CheckDirection(transform.right);
                //neg X
                else if (orbital.xyOrzAxis == Orbit.Axes.X && orbital.orbitalSpeed < 0)
                    CheckDirection(-transform.right);
                //pos Z
                if (orbital.xyOrzAxis == Orbit.Axes.Z && orbital.orbitalSpeed > 0)
                    CheckDirection(transform.forward);
                //neg Z
                else if (orbital.xyOrzAxis == Orbit.Axes.Z && orbital.orbitalSpeed < 0)
                    CheckDirection(-transform.forward);
                //pos Y
                if (orbital.xyOrzAxis == Orbit.Axes.Y && orbital.orbitalSpeed > 0)
                    CheckDirection(transform.up);
                //neg Y
                else if (orbital.xyOrzAxis == Orbit.Axes.Y && orbital.orbitalSpeed < 0)
                    CheckDirection(-transform.up);
            }
        }
	}

    //shoots ray forward looking for obstacles 
    void CheckDest()
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
            Debug.Log(gameObject.name + " found obstacles");
            if (grav.distanceFromPlanet < maxHeight - 2f)
            {
                if (usesFwdThrust)
                    ForwardThrust();
                else
                    Elevate();
            }
            else if (grav.distanceFromPlanet > maxHeight + 2f)
            {
                SideStepRight();
            }
        }
        //hit nothing, try forward shot
        else
        {
            CheckDirection(transform.forward);
        }
    }

    //shoots ray forward looking for obstacles 
    void CheckDirection(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1f, dir, out hit, 10f, obstacles))
        {
            //elevate if hit    
            Debug.Log(gameObject.name + " found obstacles");
            if (grav.distanceFromPlanet < maxHeight - 2f)
            {
                if (usesFwdThrust)
                    ForwardThrust();
                else
                    Elevate();
            }
            else if (grav.distanceFromPlanet > maxHeight + 2f)
            {
                SideStepRight();
            }
        }
    }

    //called when ray forward hits obstacle
    void Elevate()
    {
        if(grav)
            rBody.AddForce(grav.GetUp() * elevationSpeed);
        else
            rBody.AddForce(transform.up * elevationSpeed);
        Debug.Log(gameObject.name + " Elevating");
    }

    //called when ray forward hits obstacle
    void Descend()
    {
        rBody.AddForce(-transform.up * elevationSpeed);

        Debug.Log(gameObject.name + " Descending");
    }

    //called when ray forward hits obstacle
    void ForwardThrust()
    {
        Vector3 fwd = transform.forward * elevationSpeed;
        rBody.AddForce(fwd);

        Debug.Log(gameObject.name + " fwd thrusting");
    }

    //called when ray forward hits obstacle
    void BackwardThrust()
    {
        Vector3 bkwd = -transform.forward * elevationSpeed;
        rBody.AddForce(bkwd);

        Debug.Log(gameObject.name + " backward thrusting");
    }

    //called when ray forward hits obstacle
    void SideStepRight()
    {
        Vector3 right = transform.right * elevationSpeed;
        rBody.AddForce(right);
        
        Debug.Log(gameObject.name + " side stepping right");
    }

    //called when ray forward hits obstacle
    void SideStepLeft()
    {
        Vector3 left = -transform.right * elevationSpeed;
        rBody.AddForce(left);

        Debug.Log(gameObject.name + " side stepping left");
    }
}
