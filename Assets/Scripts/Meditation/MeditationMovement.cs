using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;
using InControl;
using Cameras;

public class MeditationMovement : MonoBehaviour
{
    //player and maincam
    PlayerController pc;
    Camera mainCam;
    
    //for viewing
    InputDevice inputDevice;

    [Header("Old FPS movement")]
    public bool isActive;
    public Transform thirdEyeParent;
    CharacterController thirdBody;
    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float c_sensitivityX = 3f;
    public float c_sensitivityY = 3f;
    public bool invertX, invertY;

    [Header("Astral Body Movement")]
    public float moveSpeed = 10f;
    public float fovSpeed = 1f;
    public float origSpeed;

    void Awake()
    {
        pc = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;
        origSpeed = moveSpeed;
        
        //for fps 
        if (thirdEyeParent)
        {
            thirdBody = thirdEyeParent.GetComponent<CharacterController>();
        }
    }

    void Update()
    { 
        if (isActive)
        {
            //get input
            inputDevice = InputManager.ActiveDevice;

            //only call camera rotation during fp meditation 
            CameraRotation();

            WASDmovement();

            FovControls();
        }
    }
    
    //first person on
    public void ActivateFPS()
    {
        //set transforms
        thirdEyeParent.SetParent(null);
        transform.SetParent(thirdEyeParent);
        thirdBody.enabled = true;
        
        //fully active
        isActive = true;
    }

    //first person off 
    public void DeactivateFPS()
    {
        //set transforms and body
        transform.SetParent(null);
        thirdEyeParent.SetParent(transform);
        thirdEyeParent.localPosition = Vector3.zero;
        thirdEyeParent.localRotation = Quaternion.identity;
        thirdBody.enabled = false;

        //fully inactive
        isActive = false;
    }
    
    void WASDmovement()
    {
        //create empty force vector for this frame 
        Vector3 force = Vector3.zero;
        float horizontalMovement;
        float forwardMovement;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            // 3 axes 
            horizontalMovement = inputDevice.LeftStickX;
            forwardMovement = inputDevice.LeftStickY;
        }
        //mouse & keyboard
        else
        {
            // 3 axes 
            horizontalMovement = Input.GetAxis("Horizontal");
            forwardMovement = Input.GetAxis("Vertical");
        }
        
        //FORWARD force checks
        if (forwardMovement > 0)
        {
            //add forward force 
            force += transform.forward * forwardMovement;
        }
        if (forwardMovement < 0)
        {
            //add backward force
            force += transform.forward * forwardMovement;
        }

        //HORIZONTAL force checks
        if (horizontalMovement > 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }
        if (horizontalMovement < 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }

        //actual move command 
        if (thirdBody.enabled)
            thirdBody.Move(force * moveSpeed);
    }

    void FovControls()
    {
        //scroll mousewheel to adjust FOV
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCam.fieldOfView += fovSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCam.fieldOfView -= fovSpeed;
        }
    }
    
    //only camera rotation 
    void CameraRotation()
    {
        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            hRot = c_sensitivityX * inputDevice.RightStickX;
            vRot = c_sensitivityY * inputDevice.RightStickY;
        }
        //mouse
        else
        {
            hRot = sensitivityX * Input.GetAxis("Mouse X");
            vRot = sensitivityY * Input.GetAxis("Mouse Y");
        }

        //neg value 
        if (invertX)
            hRot *= -1f;
        //neg value 
        if (invertY)
            vRot *= -1f;

        //Rotates Player on "X" Axis Acording to Mouse Input
        //Rotates Player on "Y" Axis Acording to Mouse Input
        transform.Rotate(vRot, hRot, 0);
    }

    
    private void OnDisable()
    {
        if (isActive)
        {
            DeactivateFPS();
        }
    }
}
