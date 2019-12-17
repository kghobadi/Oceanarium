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
	}

	void Update(){
        transform.LookAt(playerCam.transform.position, playerBody.GetUp());
	}

  
}
