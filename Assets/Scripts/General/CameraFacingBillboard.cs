using UnityEngine;
using System.Collections;

//This script makes any Sprite object look at the player's camera with the correct orientation from Gravity 
public class CameraFacingBillboard : MonoBehaviour
{
    PlayerController pc;
    private GravityBody playerBody;
    Camera playerCam;
    CameraController camControl;
    //fades sprite when in front of pcam
    FadeForCamera cameraFader;

    [Tooltip("Has gravity body set up")]
    public bool hasGravityBody;
    GravityBody gravBody;

	void Awake(){
        //player refs
		playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<GravityBody>();
        pc = playerBody.GetComponent<PlayerController>();
        playerCam = Camera.main;
        camControl = playerCam.GetComponent<CameraController>();
        //add camera fader if not on object already 
        cameraFader = GetComponent<FadeForCamera>();
        if(cameraFader == null)
        {
            cameraFader = gameObject.AddComponent<FadeForCamera>();
        }
        //gets and uses own grav body 
        if (hasGravityBody)
        {
            gravBody = GetComponent<GravityBody>();
        }
	}

	void Update(){
      
        //uses own gravity for up axis
        if (hasGravityBody)
        {
            transform.LookAt(playerCam.transform.position, gravBody.GetUp());
        }
        //normal, uses player gravity body for Look at function 
        else
        {
            transform.LookAt(playerCam.transform.position, playerBody.GetUp());
        }
	}

  
}
