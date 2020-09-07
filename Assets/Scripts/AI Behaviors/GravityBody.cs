using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour {
	public bool lockRotation = false;
    public bool usesGravity = true;
	public Collider[] planets;
	private Vector3 up = Vector3.zero;
    private Vector3 futureUp = Vector3.zero;
    private Vector3 smoothUp;
    public float distanceFromPlanet;
	private LayerMask planetMask;
	Rigidbody rb;

	public float smoothMultiplier = 1;
    public float overlapSphereRadius = 20f;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		if (lockRotation) {
			rb.constraints = RigidbodyConstraints.FreezeRotation;
		}
		planetMask = LayerMask.GetMask("Planet");
	}

	Vector3 GetNormal(Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000000, planetMask)) {
			return hit.normal;
		}
		return Vector3.zero;
	}

	public Vector3 GetPlanetNormal(Collider planet) {
		Vector3 rayDirection = (planet.transform.position - transform.position).normalized;
		Ray ray =  new Ray(transform.position, rayDirection);
		return GetNormal(ray);
	}

    public float GetDistFromPlanet(Collider planet)
    {
        float tempDist = Vector3.Distance(planet.transform.position, transform.position);
        RaycastHit hit;
        Vector3 rayDirection = (planet.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, rayDirection, out hit, 1000000, planetMask))
        {
            float planetRadius = Vector3.Distance(hit.point, planet.transform.position);

            return tempDist - planetRadius;
        }
        else
        {
            return 0;
        }
    }

	void FixedUpdate () {

        //running gravity checks & calcs
        Collider[] newPlanets = Physics.OverlapSphere(transform.position, overlapSphereRadius, planetMask);
        if (newPlanets.Length > 0)
        {
            planets = newPlanets;
        }

        //calc future Up & total Dist
        futureUp = Vector3.zero;
        float totalDist = 0;
        foreach (Collider planet in planets)
        {
            futureUp += GetPlanetNormal(planet);
            totalDist += GetDistFromPlanet(planet);
        }
        futureUp = futureUp.normalized;

        //gets avg dist from planets
        distanceFromPlanet = totalDist / planets.Length;

        //actually under the force of gravity
        if (usesGravity)
        {
            rb.AddForce(futureUp * -10f);

            // up = Vector3.SmoothDamp(up, futureUp, ref smoothUp, smoothTime);
            up = Vector3.Slerp(up, futureUp, Time.deltaTime * smoothMultiplier);

            // Orient
            if (lockRotation)
            {
                transform.rotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
            }
        }
    }

	public Vector3 GetUp() {
		return up;
	}
	public Vector3 GetFutureUp() {
		return futureUp;
	}
}
