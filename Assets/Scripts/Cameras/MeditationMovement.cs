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
    public GameCamera freeLook;
    CharacterController astralBody;
    Animator astralAnimator;
    SpriteRenderer astralRenderer;  

    [Header("Old FPS movement")]
    public bool isActive;
    public Transform thirdEyeParent;
    CharacterController thirdBody;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float smoothing = 2.0f;

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
            thirdBody = thirdEyeParent.GetComponent<CharacterController>();
    }

    void Update()
    { 
        if (isActive)
        {
            CameraRotation();
            //RotateCamera();

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
        //zero rotation
        var newRotate = new Vector2(0, 0);

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            newRotate = new Vector2(inputDevice.RightStickX, inputDevice.RightStickY);
        }
        //mouse
        else
        {
            newRotate = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        //multiply inputs by sensitivity
        newRotate = Vector2.Scale(newRotate, new Vector2(sensitivityX * smoothing, sensitivityY * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, newRotate.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, newRotate.y, 1f / smoothing);

        //smooth mouse look
        mouseLook += smoothV;

        //actually set rotations
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        thirdEyeParent.localRotation = Quaternion.AngleAxis(mouseLook.x, thirdEyeParent.up);
    }

    ////meditation camera controls 
    //Vector3 horizontalRotation;
    //Vector3 verticalRotation;
    //Vector3 xLook;
    //Vector3 yLook;
    //Vector3 targetMove;
    //Quaternion targetLook;
    //float heightFromPlayer;
    //float distanceFromPlayer;
    //[Header("Camera movement")]
    //public float heightMin = 5f;
    //public float heightMax = 20f;
    //public float xLookSpeed = 5f;
    //public float yLookSpeed = 5f;
    //public float smoothLook = 5f, mSmoothLook = 5f;
    //public float smoothMove = 5f, mSmoothMove = 5f;
    //public float cameraRotationSpeedX = 5f, cameraRotationSpeedY = 5f;
    //public float mCameraRotationSpeedX = 5f, mCameraRotationSpeedY = 5f;

    ////allows you to look up and down, left and right 
    //void RotateCamera()
    //{
    //    //get input device 
    //    var inputDevice = InputManager.ActiveDevice;

    //    //lets set up right analogue stick to enable us to rotate the camera around player and redirect motion as we do so
    //    horizontalRotation = Vector3.zero;
    //    verticalRotation = Vector3.zero;

    //    //set target move 
    //    targetMove = astralEye.position + (astralEye.rotation * new Vector3(0, heightFromPlayer, -distanceFromPlayer));

    //    //using ps4 controller
    //    if (inputDevice.DeviceClass == InputDeviceClass.Controller)
    //    {
    //        //cam pos
    //        transform.position = Vector3.Lerp(transform.position, targetMove, smoothMove * Time.deltaTime);
    //        //cam inputs
    //        horizontalRotation = new Vector3(inputDevice.RightStickX * cameraRotationSpeedX, 0, 0);
    //        verticalRotation = new Vector3(0, inputDevice.RightStickY * cameraRotationSpeedY, 0);
    //    }
    //    //using mouse and WASD
    //    else
    //    {
    //        //cam pos
    //        transform.position = Vector3.Lerp(transform.position, targetMove, mSmoothMove * Time.deltaTime);
    //        //cam inputs
    //        horizontalRotation = new Vector3(Input.GetAxis("Mouse X") * mCameraRotationSpeedX, 0, 0);
    //        verticalRotation = new Vector3(0, Input.GetAxis("Mouse Y") * mCameraRotationSpeedY, 0);
    //    }

    //    //calc x look
    //    xLook = Vector3.Lerp(xLook, astralEye.right * horizontalRotation.x, Time.deltaTime * xLookSpeed);

    //    //calc y look
    //    yLook = Vector3.Lerp(yLook, astralEye.up * verticalRotation.y, Time.deltaTime * yLookSpeed);

    //    //add yLook to the player pos, then subtract cam pos to get the forward look
    //    targetLook = Quaternion.LookRotation((astralEye.position + xLook + yLook) - transform.position);

    //    //using ps4 controller
    //    if (inputDevice.DeviceClass == InputDeviceClass.Controller)
    //    {
    //        transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);
    //    }
    //    //mouse 
    //    else
    //    {
    //        transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, mSmoothLook * Time.deltaTime);
    //    }

    //    //clamp height from player 
    //    heightFromPlayer = Mathf.Clamp(heightFromPlayer, heightMin, heightMax);
    //}

    private void OnDisable()
    {
        DeactivateFPS();
    }
}
