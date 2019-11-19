using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photon : MonoBehaviour {

    public bool dancing;
    public float photonSpeed = 50f;
    public float currentDistance, minDistance, maxDistance;

    public ParticleSystem particles;

    void Start()
    {
        minDistance = Vector3.Distance(transform.position, transform.parent.position);
        maxDistance = minDistance + 2500;
        particles = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    void Update () {
        currentDistance = Vector3.Distance(transform.position, transform.parent.position);

        if (dancing)
        {
            if(currentDistance < maxDistance)
                MoveFaster();

            if (!particles.isPlaying)
                particles.Play();
        }
        else
        {
            if(currentDistance > minDistance)
                MoveBack();

            if (particles.isPlaying)
                particles.Stop();
        }
	}

    void MoveFaster()
    {
        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 10);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, newPos, photonSpeed);
    }

    void MoveBack()
    {
        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 10);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, newPos, photonSpeed);
    }

}
