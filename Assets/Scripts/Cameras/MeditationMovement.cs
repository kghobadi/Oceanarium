using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using InControl;

public class MeditationMovement : MonoBehaviour
{
    //player and maincam
    PlayerController pc;
    Camera mainCam;
    //third eye body 
    CharacterController thirdBody;
    //for viewing
    Vector2 mouseLook;
    Vector2 smoothV;

    [Tooltip("Main Cam will be parented to this while meditating")]
    public Transform thirdEyeParent;

    public bool isActive;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float smoothing = 2.0f;
    
    public float moveSpeed = 10f;
    public float fovSpeed = 1f;
    
    void Awake()
    {
        pc = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;

        thirdBody = thirdEyeParent.GetComponent<CharacterController>();
        thirdBody.enabled = false;
    }

    void Update()
    { 
        if (isActive)
        {
            CameraRotation();

            ClickMovement();

            FovControls();
        }
    }

    public void Activate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        thirdEyeParent.localPosition = Vector3.zero;
        thirdEyeParent.localRotation = Quaternion.identity;
        thirdEyeParent.SetParent(null);
        transform.SetParent(thirdEyeParent);
        thirdBody.enabled = true;
        isActive = true;
    }

    public void Deactivate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        transform.SetParent(null);
        thirdEyeParent.SetParent(transform);
        thirdBody.enabled = false;
        isActive = false;
    }

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

    void ClickMovement()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        
        //left click to float camera forward through space
        if (Input.GetMouseButton(0) || inputDevice.DPadY > 0)
        {
            Vector3 forward = transform.forward * moveSpeed;
            thirdBody.Move(forward);
        }
        //right click to float camera backward through space
        if (Input.GetMouseButton(1) || inputDevice.DPadY < 0)
        {
            Vector3 backward = -transform.forward * moveSpeed ;
            thirdBody.Move(backward);
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
}
