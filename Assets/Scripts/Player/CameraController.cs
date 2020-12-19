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
    [HideInInspector] public Rigidbody cRigidbody;

    //control bools
    [Header("Cam Movement Bools")]
    public bool canMoveCam = true;
    public bool isMovingCamPos;
    public bool isMovingCamAngle;

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
    public float heightAvg;

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
    public float overlapSphereRadius = 5f;
    public float forcePush = 5f;
    public LayerMask obstructionMask;
    public List<int> obstructionLayers = new List<int>();
    [HideInInspector] public Transform currentSpeaker;
    public LerpScale seeThruSphere;
    public float sActiveSize = 15f;
    bool sphereActive;

    [Header("FOV")]
    public bool lerpingFOV;
    public float originalFOV;
    public float meditationFOV = 85f;
    public float minFOV =40f, maxFOV = 90f;
    float nextFOV;
    float t;
    float lerpSpeed = 0.5f;
    
    void Awake ()
    {
        cameraT = Camera.main.transform;
        mainCam = cameraT.GetComponent<Camera>();
        originalFOV = mainCam.fieldOfView;
        player = GameObject.FindGameObjectWithTag("Player");
        origPlayer = player;
        pc = player.GetComponent<PlayerController>();
        gravityBody = player.GetComponent<GravityBody>();
        cRigidbody = GetComponent<Rigidbody>();
        origGBody = gravityBody;
        heightAvg = heightMin + ((heightMax - heightMin) / 2);
    }

    void Start()
    {
        //can move at start
        canMoveCam = true;
    }

    public void SetCamPos(float height)
    {
        //set start height to middle val 
        heightFromPlayer = height;
        //calc start pos
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * heightFromPlayer);
        //set new cam pos 
        cameraT.position = futureCameraPosition;
        //get the look rotation
        transform.LookAt(player.transform, gravityBody.GetUp());
    }

    void Update ()
    {
        if (canMoveCam)
        {
            HorizontalRotation();

            VerticalRotation();

            ZoomInputs();

            CheckMoving();

            if(pc.moveState != PlayerController.MoveStates.MEDITATING)
                CastToPlayer();
            else
            {
                if (sphereActive)
                    DeactivateSphere();
            }
        }

        //fade objs when player can move 
        FadeCamObstructions(player.transform, 0.5f);
        //when player cannot move -- this is mostly during mono
        if(currentSpeaker != null)
        {
            FadeCamObstructions(currentSpeaker.transform, 0.1f);
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);

        //checks if input for fading out cam controls image at start
        if (pc.controlsAtStart.Length > 1)
        {
            //check that its active and not already fading out 
            if (pc.controlsAtStart[1].gameObject.activeSelf &&
                pc.controlsAtStart[1].fadeState != FadeSprite.FadeStates.FADINGOUT &&
                pc.controlsAtStart[1].fadeState != FadeSprite.FadeStates.TRANSPARENT)
            {
                //input ? 
                if((vRot != 0 || hRot != 0) && Time.time > 3f)
                    pc.controlsAtStart[1].FadeOut();
            }
        }
    }

    void ZoomInputs()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            zoomInput = inputDevice.DPadY;
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

    public void ZoomDirect(float value)
    {
        heightFromPlayer = value;
    }

    //detects whether cam is seeing ground in front of player somehw
    void CastToPlayer()
    {
        //Debug.Log("casting to player");

        RaycastHit hit = new RaycastHit();
        Vector3 dir = player.transform.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);
       
        //send raycast
        if (Physics.Raycast(transform.position, dir, out hit, dist , obstructionMask))
        {
            //anytime we hit the planet ground -- turn on sphere
            ActivateSphere();
            return;
        }

        //overlap sphere 
        Collider[] obstructions = Physics.OverlapSphere(transform.position, overlapSphereRadius, obstructionMask);
        if (obstructions.Length > 0)
        {
            ActivateSphere();
            return;
        }

        //this is always getting called whenever activate is..
        if (sphereActive)
        {
            DeactivateSphere();
        }
    }

    void ActivateSphere()
    {
        if (sphereActive)
            return;

        //only on mazeworld or cube planet
        if(pc.activePlanet.planetName == "Mazeworld" || pc.activePlanet.planetName == "CubePlanet")
        {
            seeThruSphere.SetScaler(2.5f, new Vector3(sActiveSize, sActiveSize, sActiveSize));
            sphereActive = true;
        }
        
        //Debug.Log("activated sphere");
    }

    void DeactivateSphere()
    {
        seeThruSphere.SetScaler(5f, Vector3.zero);
        sphereActive = false;

        //Debug.Log("deactivating sphere!");
    }

    //checks if cam is moving 
    void CheckMoving()
    {
        //save rot 
        currentCamRot = cameraT.localEulerAngles;

        //check if cam rot has changed
        if (Vector3.Distance(currentCamRot, lastCamRot) > 0.1f)
        {
            isMovingCamAngle = true;
        }
        else
        {
            isMovingCamAngle = false;
        }

        //check if cam pos
        if (Vector3.Distance(cameraT.position, oldCameraPosition) > 0.1f)
        {
            isMovingCamPos = true;
        }
        else
        {
            isMovingCamPos = false;
        }

        lastCamRot = cameraT.localEulerAngles;
    }

    //shoots rays towards players and fades opacities of sprite s
    void FadeCamObstructions(Transform objectToCheck, float fadeOutAmount)
    {
        RaycastHit hit = new RaycastHit();
        Vector3 dir = player.transform.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //send raycast
        if (Physics.SphereCast(transform.position, fadeRadius, dir, out hit, dist, spriteFadeMask))
        {
            FadeForCamera ffCam = hit.transform.GetComponent<FadeForCamera>();
            if (ffCam)
            {
                ffCam.Fade(fadeOutAmount);
                //Debug.Log("fading " + ffCam.gameObject.name + " for cam");
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
        //Debug.Log("somemthing called Lerp FOV!");
    }

}
