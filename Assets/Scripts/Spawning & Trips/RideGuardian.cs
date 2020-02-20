using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple 2d wasd movement for the guardian during trips 
public class RideGuardian : MonoBehaviour {

    public float moveSpeed;
    public float xMin, xMax;
    public float yMin, yMax;

	
	void Update ()
    {
        MoveInputs();
	}

    void MoveInputs()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");


        //set target pos 
        Vector3 targetPos = new Vector3(transform.position.x + horizontalMovement, transform.position.y + verticalMovement, transform.position.z);

        //x bound
        if (targetPos.x > xMax)
            targetPos.x = xMax;
        if (targetPos.x < xMin)
            targetPos.x = xMin;
        //y bound
        if (targetPos.y > yMax)
            targetPos.y = yMax;
        if (targetPos.y < yMin)
            targetPos.y = yMin;

        //move towards 
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
