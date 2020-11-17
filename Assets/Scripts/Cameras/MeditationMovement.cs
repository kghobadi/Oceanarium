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
    Transform player;
    PlayerController pc;
    Camera mainCam;
    
    //for viewing
    Vector2 mouseLook;
    Vector2 smoothV;

    [Header("Old FPS movement")]
    public bool isActive;
    public Transform thirdEyeParent;
    GravityBody thirdGravity;
    CharacterController thirdBody;
    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public bool invertX, invertY;

    [Header("Astral Body Movement")]
    public float moveSpeed = 10f;
    public float fovSpeed = 1f;

    [Header("Meditation Layers")]
    public MeditationLayers meditationLayer;
    public enum MeditationLayers
    {
        PLANAR = 0,
        SANCTUM = 1,
        GALACTIC = 2,
        ABYSSAL = 3,
    }
    public float meditationTimer = 0f;
    float meditationStart;
    public float meditationIncrements = 30f;
    public UnityEvent enteredNewLayer;
    public UnityEvent endedMeditation;

    void Awake()
    {
        pc = FindObjectOfType<PlayerController>();
        player = pc.transform;
        mainCam = Camera.main;
        
        //for fps 
        if (thirdEyeParent)
        {
            thirdBody = thirdEyeParent.GetComponent<CharacterController>();
            thirdGravity = thirdEyeParent.GetComponent<GravityBody>();
        }

        //start layer at 0
        meditationLayer = MeditationLayers.PLANAR;
    }

    void Update()
    { 
        if (isActive)
        {
            //only call camera rotation during fp meditation 
            CameraRotation();

            WASDmovement();

            FovControls();

            Ascendancy();
        }
    }
    
    //first person on
    public void ActivateFPS()
    {
        //set transforms
        thirdEyeParent.SetParent(null);
        transform.SetParent(thirdEyeParent);
        thirdBody.enabled = true;

        //enter sanctum
        meditationLayer = MeditationLayers.SANCTUM;
        meditationStart = Time.time;
        enteredNewLayer.Invoke();
        
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

        //return to planar realm
        meditationLayer = MeditationLayers.PLANAR;
        meditationTimer = 0;
        endedMeditation.Invoke();

        //fully inactive
        isActive = false;
    }
    
    void WASDmovement()
    {
        //create empty force vector for this frame 
        Vector3 force = Vector3.zero;
        float horizontalMovement;
        float forwardMovement;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;

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
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            hRot = sensitivityX * inputDevice.RightStickX;
            vRot = sensitivityY * inputDevice.RightStickY;
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

    void Ascendancy()
    {
        //time meditation
        meditationTimer = Time.time - meditationStart;

        //if timer is greater than next increment and we have not reached final layer
        if (meditationTimer > meditationIncrements * (int)meditationLayer
            && meditationLayer < MeditationLayers.ABYSSAL)
        {
            //ascend
            meditationLayer++;
            enteredNewLayer.Invoke();
        }
    }
    
    private void OnDisable()
    {
        if (isActive)
        {
            DeactivateFPS();
        }
    }
}
