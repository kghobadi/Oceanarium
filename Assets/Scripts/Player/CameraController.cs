using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class CameraController : MonoBehaviour {
    //player ref
    GameObject player, origPlayer;
    PlayerController pc;
    [HideInInspector] public GravityBody gravityBody, origGBody;
    //camera refs
    Transform cameraT;
    Camera mainCam;
   
    //control bools
    [Header("Cam Movement Bools")]
    public bool canMoveCam = true;
    public bool isMovingCam;

    [Header("Camera Movement Vars")]
    public float mouseSensitivityX = 20;
    public float mouseSensitivityY = 20;
    public float controllerSensitivityX = 20;
    public float controllerSensitivityY = 20;
    public bool invertX, invertY;
    public bool invertXc, invertYc;
    public float maxVerticalLookAngle = 85f;
    public float minVerticalLookAngle = -15f;
    public float yLookPosAdjust = 5f;
    public float smoothMove = 0.5f;
    public float smoothLook = 0.5f;

    [Header("Meditation Values")]
    public float m_SensitivityX = 3f;
    public float m_SensitivityY = 1f;
    public float m_maxVerticalLookAngle = 180f;
    public float m_minVerticalLookAngle = -180f;
    public float m_yLookPosAdjust = 0f;
    float origSensitivityX;
    float origSensitivityY;
    float origMaxLook, origMinLook;
    float origYLookAdjust;

    [Header("Zoom Vars")]
    public float heightFromPlayer = 20f;
    public float heightMin, heightMax;
    public float zoomSpeed = 500f;

    //privately stored temp variables 
    float zoomInput;
    float vRot = 0, hRot;
    Vector3 lastCamRot, currentCamRot;
    Vector3 oldCameraPosition;
    Vector3 smoothCameraVelocity;
    Quaternion targetLook;

    [Header("Masks")]
    public LayerMask spriteFadeMask;
    public float fadeRadius = 1f;
    public LayerMask obstructionMask;
    public List<int> obstructionLayers = new List<int>();  

    [Header("FOV")]
    public bool lerpingFOV;
    public float originalFOV;
    public float meditationFOV = 85f;
    public float minFOV =40f, maxFOV = 90f;
    float nextFOV;
    float t;
    float lerpSpeed = 0.5f;

    [Header("Astral Body")]
    public GameObject astralBody;
    GravityBody astralGravity;
    

    void Awake () {
        cameraT = Camera.main.transform;
        mainCam = cameraT.GetComponent<Camera>();
        originalFOV = mainCam.fieldOfView;
        player = GameObject.FindGameObjectWithTag("Player");
        origPlayer = player;
        pc = player.GetComponent<PlayerController>();
        gravityBody = player.GetComponent<GravityBody>();
        origGBody = gravityBody;
        astralGravity = astralBody.GetComponent<GravityBody>();
    }

    void Start()
    {
        //can move at start
        canMoveCam = true;
        SetCamPos();
    }

    public void SetCamPos()
    {
        //set start height to middle val 
        heightFromPlayer = heightMin + ((heightMax - heightMin) / 2);
        //calc start pos
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * heightFromPlayer);
        //set new cam pos 
        cameraT.position = futureCameraPosition;
        //get the look rotation
        transform.LookAt(player.transform, gravityBody.GetUp());
    }

    void Update () {

        if (canMoveCam)
        {
            HorizontalRotation();

            VerticalRotation();

            ZoomInputs();

            CheckMoving();

            CastToPlayer();
        }

        //fade objs when player can move 
        if (pc.canMove)
        {
            FadeCamObstructions();
        }

        //smoothly changing FOV
        if (lerpingFOV)
        {
            //actual lerp 
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, nextFOV, t);
            mainCam.fieldOfView = Mathf.Clamp(mainCam.fieldOfView, minFOV, maxFOV);
            //interpolate!
            t += Time.deltaTime * lerpSpeed;
            //stop once t value = 1
            if (t > 1.0f)
            {
                lerpingFOV = false;
            }
        }
    }

    void HorizontalRotation()
    { 
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            // Rotate player around Y axis        
            hRot = inputDevice.RightStickX * controllerSensitivityX;
            //invert
            if (invertXc)
                hRot *= -1;
        }
        //mouse & keyboard
        else
        {
            // Rotate player around Y axis        
            hRot = Input.GetAxis("Mouse X") * mouseSensitivityX;
            //invert
            if (invertX)
                hRot *= -1;
        }
          
       
        //set old cam pos
        oldCameraPosition = cameraT.position;

        //rotate player 
        player.transform.RotateAround(player.transform.position, player.transform.up, hRot);
        //reset cam pos 
        cameraT.position = oldCameraPosition;
    }

    void VerticalRotation()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        // Rotate camera around X axis  

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            //invert
            if (invertYc)
                vRot += inputDevice.RightStickY * controllerSensitivityY * -1f;
            //normal
            else
                vRot += inputDevice.RightStickY * controllerSensitivityY;
        }
        //mouse & keyboard
        else
        {
            //invert
            if (invertY)
                vRot += Input.GetAxis("Mouse Y") * mouseSensitivityY * -1f;
            //normal
            else
                vRot += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        }
      
        // Position player
        vRot = Mathf.Clamp(vRot, minVerticalLookAngle, maxVerticalLookAngle);
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * heightFromPlayer);
        //set new cam pos 
        cameraT.position = Vector3.SmoothDamp(cameraT.position, futureCameraPosition, ref smoothCameraVelocity, smoothMove);
        
        //get the look rotation
        targetLook = Quaternion.LookRotation((player.transform.position + (player.transform.up * yLookPosAdjust))
            - transform.position, gravityBody.GetUp());
        //lerp towards new look rotation 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);
    }

    void ZoomInputs()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            //positive
            if (inputDevice.RightTrigger)
            {
                zoomInput = Mathf.Lerp(zoomInput, 1, Time.deltaTime);
            }
            //negative
            else if (inputDevice.LeftTrigger)
            {
                zoomInput = Mathf.Lerp(zoomInput, -1, Time.deltaTime);
            }
            //return to 0
            else if(inputDevice.RightTrigger.IsPressed == false && inputDevice.LeftTrigger.IsPressed == false)
            {
                zoomInput = Mathf.Lerp(zoomInput, 0, Time.deltaTime);
            }
        }
        //mouse & keyboard
        else
        {
            zoomInput = Input.GetAxis("Mouse ScrollWheel");
        }
       

        //zoom in
        if (zoomInput < 0 && heightFromPlayer > heightMin)
        {
            //Debug.Log("zoom in");
            ZoomIn(zoomInput);
        }
        //and out
        if (zoomInput > 0 && heightFromPlayer < heightMax)
        {
            //Debug.Log("zoom out");
            ZoomOut(zoomInput);
        }
    }

    void ZoomIn(float zoom)
    {
        float newHeight = heightFromPlayer + (zoomSpeed * Time.deltaTime * zoom);
        heightFromPlayer = Mathf.Lerp(heightFromPlayer, newHeight, zoomSpeed * Time.deltaTime);
        heightFromPlayer = Mathf.Clamp(heightFromPlayer, heightMin, heightMax);
    }

    void ZoomOut(float zoom)
    {
        float newHeight = heightFromPlayer + (zoomSpeed * Time.deltaTime * zoom);
        heightFromPlayer = Mathf.Lerp(heightFromPlayer, newHeight, zoomSpeed * Time.deltaTime);
        heightFromPlayer = Mathf.Clamp(heightFromPlayer, heightMin, heightMax);
    }

    //detects whether cam is seeing ground in front of player somehw
    void CastToPlayer()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 dir = player.transform.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //send raycast
        if (Physics.SphereCast(transform.position, 1f, dir, out hit, dist , obstructionMask))
        {
            //Debug.Log("hit ground, zoomng in");
            //anytime we hit the planet ground, zoome out 
            if (heightFromPlayer > heightMin)
                ZoomIn(-0.05f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //zoom in if camera is collding with stuff we dont like
        if (obstructionLayers.Contains(other.gameObject.layer))
        {
            ZoomIn(-0.05f);
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
        RaycastHit hit = new RaycastHit();
        Vector3 dir = player.transform.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //send raycast
        if (Physics.SphereCast(transform.position, fadeRadius, dir, out hit, dist, spriteFadeMask))
        {
            if(hit.transform.GetComponent<FadeForCamera>())
            {
                hit.transform.GetComponent<FadeForCamera>().Fade(0.5f);
            }
        }
    }
    
    //called to lerp cam fov after transitions 
    public void LerpFOV(float desiredFOV, float lerpLength)
    {
        nextFOV = desiredFOV;
        t = 0;
        lerpSpeed = lerpLength;
        lerpingFOV = true;
        Debug.Log("somemthing called Lerp FOV!");
    }

    //called when meditating
    public void SetAstralBody()
    {
        astralBody.SetActive(true);
        player = astralBody;
        gravityBody = astralGravity;

        //zoom out a bit 
        ZoomOut(15f);
        
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            origSensitivityX = controllerSensitivityX;
            origSensitivityY = controllerSensitivityY;

            controllerSensitivityX = m_SensitivityX;
            controllerSensitivityY = m_SensitivityY;
        }
        //m keyboard
        else
        {
            origSensitivityX = mouseSensitivityX;
            origSensitivityY = mouseSensitivityY;

            mouseSensitivityX = m_SensitivityX;
            mouseSensitivityY = m_SensitivityY;
        }

        //set originals
        origMinLook = minVerticalLookAngle;
        origMaxLook = maxVerticalLookAngle;
        origYLookAdjust = yLookPosAdjust;
        //set new values for astral projection
        minVerticalLookAngle = m_minVerticalLookAngle;
        maxVerticalLookAngle = m_maxVerticalLookAngle;
        yLookPosAdjust = m_yLookPosAdjust;
    }

    //called when returning to body from meditation
    public void DisableAstralBody()
    {
        astralBody.SetActive(false);
        player = origPlayer;
        gravityBody = origGBody;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            controllerSensitivityX = origSensitivityX;
            controllerSensitivityY = origSensitivityY;
        }
        //m keyboard
        else
        {
            mouseSensitivityX = origSensitivityX;
            mouseSensitivityY = origSensitivityY;
        }

        //return values to original for swimming 
        minVerticalLookAngle = origMinLook;
        maxVerticalLookAngle = origMaxLook;
        yLookPosAdjust = origYLookAdjust;
    }
}
