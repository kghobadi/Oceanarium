using UnityEngine;
using System.Collections;

//This script makes any Sprite object look at the player's camera with the correct orientation from Gravity 
public class CameraFacingBillboard : MonoBehaviour
{
    GameObject player;
    PlayerController pc;
    private GravityBody playerBody;
    Camera playerCam;
    CameraController camControl;
    SpriteRenderer sr;
   

    [Tooltip("Has gravity body set up")]
    public bool hasGravityBody;
    [Tooltip("Freezes position by default (for static objs).")]
    public bool freezePosition = true;
    GravityBody gravBody;
    Rigidbody rBody;

    void Awake(){
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        if (player!= null)
        {
            playerBody = player.GetComponent<GravityBody>();
            pc = player.GetComponent<PlayerController>();
        }
      
        //cam refs
        playerCam = Camera.main;
        camControl = playerCam.GetComponent<CameraController>();

        sr = GetComponent<SpriteRenderer>();

        //gets and uses own grav body 
        if (hasGravityBody)
        {
            gravBody = GetComponent<GravityBody>();

            if(gravBody == null)
            {
                gravBody = gameObject.AddComponent<GravityBody>();
            }

            rBody = GetComponent<Rigidbody>();

            //set rBody position constraints
            if (freezePosition)
            {
                rBody.constraints = RigidbodyConstraints.FreezePosition;
            }
        }
	}

	void Update(){
      
        //uses own gravity for up axis
        if (hasGravityBody)
        {
            if(sr.isVisible)
                transform.LookAt(playerCam.transform.position, gravBody.GetUp());
        }
        //normal, uses player gravity body for Look at function 
        else
        {
            //NORMAL MOVEMENT 
            if(pc.moveState != PlayerController.MoveStates.MEDITATING)
            {
                if (player)
                    transform.LookAt(playerCam.transform.position, playerBody.GetUp());
                else
                    transform.LookAt(playerCam.transform.position, Vector3.up);
            }
            //MEDITATING
            else if (pc.moveState == PlayerController.MoveStates.MEDITATING)
            {
                //fp -- look at cam
                transform.LookAt(playerCam.transform.position, playerCam.transform.up);
            }
        }
	}

    private void OnBecameVisible()
    {
        if (hasGravityBody)
        {
            if (gravBody.enabled)
                gravBody.enabled = false;
        }
    }

    private void OnBecameInvisible()
    {
        if (hasGravityBody)
        {
            if (!gravBody.enabled)
                gravBody.enabled = true;
        }
    }

}
