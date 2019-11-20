using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //player ref
    GameObject player;
    PlayerController pc;

    //control bools
    [Header("Cam Movement Bools")]
    public bool canMoveCam = true;
    public bool isMovingCam;

    [Header("Camera Movement Vars")]
    public float mouseSensitivityX = 20;
    public float mouseSensitivityY = 20;
    public float maxVerticalLookAngle = 75f;
    public float minVerticalLookAngle = -75f;
    public float yLookPosAdjust = 5f;
    public float camLookSpeed;
    float cameraDistance = 20f;
    float vRot = 0, hRot;
    Vector3 lastCamRot, currentCamRot;
    Vector3 oldCameraPosition;

    Transform cameraT;
    Camera mainCam;
    Vector3 smoothCameraVelocity;
    GravityBody gravityBody;

    public LayerMask spriteFadeMask;

    public bool lerpingFOV;
    public float originalFOV;
    float nextFOV;
    float startTime;

    void Awake () {
        cameraT = Camera.main.transform;
        mainCam = cameraT.GetComponent<Camera>();
        originalFOV = mainCam.fieldOfView;
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        gravityBody = player.GetComponent<GravityBody>();
        canMoveCam = true;
    }
	
	void Update () {

        if (canMoveCam)
        {
            // Rotate player around Y axis
            hRot = Input.GetAxis("Mouse X") * mouseSensitivityX;
            oldCameraPosition = cameraT.position;

            //rotate player 
            player.transform.RotateAround(player.transform.position, player.transform.up, hRot);
            cameraT.position = oldCameraPosition;

            // Rotate camera around X axis
            // Position player
            vRot += -1 * Input.GetAxis("Mouse Y") * mouseSensitivityY;
            vRot = Mathf.Clamp(vRot, minVerticalLookAngle, maxVerticalLookAngle);
            Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
            Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * cameraDistance);
            //set new cam pos 
            cameraT.position = Vector3.SmoothDamp(cameraT.position, futureCameraPosition, ref smoothCameraVelocity, .5f);
            //look at player with gravity Up
            cameraT.LookAt(player.transform, gravityBody.GetUp());

            CheckMoving();
        }

        //fade objs when player can move 
        if (pc.canMove)
        {
            FadeCamObstructions();
        }

        //smoothly changing FOV
        if (lerpingFOV)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, nextFOV, Time.time - startTime);
            //stop once t value = 1
            if(Time.time - startTime >= 1)
            {
                lerpingFOV = false;
            }
        }
    }

    //checks if cam is moving 
    void CheckMoving()
    {
        //save rot 
        currentCamRot = cameraT.localEulerAngles;

        //check if cam pos or cam rot has changed
        if (Vector3.Distance(cameraT.position, oldCameraPosition) > 0.1f ||
            Vector3.Distance(currentCamRot, lastCamRot) > 0.1f)
        {
            isMovingCam = true;
        }
        else
        {
            isMovingCam = false;
        }

        lastCamRot = cameraT.localEulerAngles;
    }

    //shoots rays towards players and fades opacities of sprite s
    void FadeCamObstructions()
    {
        Ray camRay = new Ray(cameraT.position, cameraT.TransformDirection(Vector3.forward));
        RaycastHit camHit;

        if (Physics.Raycast(camRay, out camHit, 100f, spriteFadeMask))
        {
            camHit.transform.GetComponent<CameraFacingBillboard>().Fade();
        }
    }

    //called to lerp cam fov after transitions 
    public void LerpFOV(float desiredFOV)
    {
        startTime = Time.time;
        nextFOV = desiredFOV;
        lerpingFOV = true;
    }
}
