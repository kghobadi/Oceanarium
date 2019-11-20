using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour {
	public bool lockRotation = false;
	public Collider[] planets;
	private Vector3 up = Vector3.zero;

	private Vector3 futureUp = Vector3.zero;
	private Vector3 smoothUp;
	private LayerMask planetMask;
	Rigidbody rb;

	public float smoothMultiplier = 1;

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

	Vector3 GetPlanetNormal(Collider planet) {
		Vector3 rayDirection = (planet.transform.position - transform.position).normalized;
		Ray ray =  new Ray(transform.position, rayDirection);
		return GetNormal(ray);
	}

	void FixedUpdate () {
		Collider[] newPlanets = Physics.OverlapSphere(transform.position, 20, planetMask);
		if (newPlanets.Length > 0) {
			planets = newPlanets;
		}

		Vector3 futureUp = Vector3.zero;
		foreach (Collider planet in planets) {
			futureUp += GetPlanetNormal(planet);
		}
		futureUp = futureUp.normalized;
		rb.AddForce(futureUp * -10f);

		// up = Vector3.SmoothDamp(up, futureUp, ref smoothUp, smoothTime);
        up = Vector3.Slerp(up, futureUp, Time.deltaTime * smoothMultiplier);

		// Orient
		if (lockRotation) {
			transform.rotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
		}
	}

	public Vector3 GetUp() {
		return up;
	}
	public Vector3 GetFutureUp() {
		return futureUp;
	}
}
