using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //astral eye body 
    public Transform astralEye;
    CharacterController astralBody;
    Animator astralAnimator;
    SpriteRenderer astralRenderer;  

    [Header("Old FPS movement")]
    public bool isActive;
    public bool fp;
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

    void Awake()
    {
        pc = FindObjectOfType<PlayerController>();
        player = pc.transform;
        mainCam = Camera.main;

        //set astralbody
        if (astralEye)
        {
            //char controller
            astralBody = astralEye.GetComponent<CharacterController>();
            astralBody.enabled = false;
            //animation
            astralAnimator = astralEye.GetComponentInChildren<Animator>();
            astralRenderer = astralEye.GetComponentInChildren<SpriteRenderer>();
            //disable at start 
            astralEye.gameObject.SetActive(false);
        }

        //for fps 
        if (thirdEyeParent)
        {
            thirdBody = thirdEyeParent.GetComponent<CharacterController>();
            thirdGravity = thirdEyeParent.GetComponent<GravityBody>();
        }
    }

    void Update()
    { 
        if (isActive)
        {
            //only call camera rotation during fp meditation 
            if(fp)
                CameraRotation();

            WASDmovement();

            FovControls();
        }
    }

    public void Activate()
    {
        //liberate astral body 
        astralEye.gameObject.SetActive(true);
        astralEye.SetParent(null);
        astralBody.enabled = true;

        //enable animation
        astralRenderer.enabled = true;
        astralAnimator.enabled = true;
        //freeLook.gameObject.SetActive(true);

        fp = false;
        isActive = true;
    }

    public void Deactivate()
    {
        //reset to body
        astralEye.SetParent(player);
        astralEye.localPosition = Vector3.zero;
        astralEye.localRotation = Quaternion.identity;   
        astralBody.enabled = false;
        astralEye.gameObject.SetActive(false);

        //disable animation
        astralRenderer.enabled = false;
        astralAnimator.enabled = false;
        //freeLook.gameObject.SetActive(false);

        isActive = false;
    }

    //first person on
    public void ActivateFPS()
    {
        thirdEyeParent.SetParent(null);
        transform.SetParent(thirdEyeParent);
        thirdBody.enabled = true;

        fp = true;
        isActive = true;
    }

    //first person off 
    public void DeactivateFPS()
    {
        transform.SetParent(null);
        thirdEyeParent.SetParent(transform);
        thirdEyeParent.localPosition = Vector3.zero;
        thirdEyeParent.localRotation = Quaternion.identity;
        thirdBody.enabled = false;
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
        if(astralBody.enabled)
            astralBody.Move(force * moveSpeed);
        if (thirdBody.enabled)
            thirdBody.Move(force * moveSpeed);
    }

    void ClickMovement()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        
        //left click to float camera forward through space
        if (Input.GetMouseButton(0) || inputDevice.DPadY > 0)
        {
            Vector3 forward = transform.forward * moveSpeed;
            astralBody.Move(forward);
        }
        //right click to float camera backward through space
        if (Input.GetMouseButton(1) || inputDevice.DPadY < 0)
        {
            Vector3 backward = -transform.forward * moveSpeed ;
            astralBody.Move(backward);
        }
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
    
    private void OnDisable()
    {
        if (isActive)
        {
            if (fp)
                DeactivateFPS();
            else
                Deactivate();
        }
    }
}
