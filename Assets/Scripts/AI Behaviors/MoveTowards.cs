using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour {
    public float moveSpeed;
    public float origSpeed;

    public bool moving;
    public Vector3 destination;
    public float necessaryDist = 1f;

    void Start()
    {
        origSpeed = moveSpeed;
    }

    void Update () {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

            if(Vector3.Distance(transform.position, destination) < necessaryDist)
            {
                moving = false;
                moveSpeed = origSpeed;
            }
        }
	}

    public void MoveTo(Vector3 dest, float speed)
    {
        destination = dest;
        moveSpeed = speed;
        moving = true;
    }
}
