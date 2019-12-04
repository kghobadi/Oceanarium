using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put this on an animated object to move it during a trip sequence
public class TripperMovement : MonoBehaviour {
    Transform tripCamera;
    PooledObject pooledObj;
    Vector3 originalPos;
    //supplied by Spawner 
    public Transform centerPoint;

    [Tooltip("Speed the tripper object will move towards the camera")]
    public float forwardSpeed;
    public bool waitToMove;
    public bool moving;
    public MovementPatterns movementPattern;
    public enum MovementPatterns
    {
        ROTATEAROUNDCENTER, TOANDFROMCENTER,
    }

	void Awake ()
    {
        tripCamera = GameObject.FindGameObjectWithTag("TripCam").transform;
	}
    void Start()
    {
        pooledObj = GetComponent<PooledObject>();
    }

    //called by Spawner when it generates Tripper Objects
    public void WaitToMove(float offsetAmount)
    {
        StartCoroutine(WaitForMoving(offsetAmount));
    }

    //wait to start moving the object with chosen pattern 
    IEnumerator WaitForMoving(float time)
    {
        yield return new WaitForSeconds(time);

        moving = true;
    }

    void Update ()
    {
        //moving bool allows us to offset the movement from start 
        if (moving)
        {
            switch (movementPattern)
            {
                case MovementPatterns.ROTATEAROUNDCENTER:
                    RotateAroundCenter();
                    break;
                case MovementPatterns.TOANDFROMCENTER:
                    ToAndFromCenter();
                    break;
            }
        }

        //always move towards camera
        transform.Translate(new Vector3(0, 0, forwardSpeed * Time.deltaTime));

        //check for passing the camera to return to pool 
        if(transform.position.z < tripCamera.position.z - 3)
        {
            pooledObj.ReturnToPool();
        }
      
	}

    //rotates this tripper object around its Generation's center point
    void RotateAroundCenter()
    {

    }

    //moves the object 
    void ToAndFromCenter()
    {

    }

}
