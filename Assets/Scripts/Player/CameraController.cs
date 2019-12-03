using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //player ref
    GameObject player;
    PlayerController pc;
    GravityBody gravityBody;
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
    public float maxVerticalLookAngle = 85f;
    public float minVerticalLookAngle = -15f;
    public float yLookPosAdjust = 5f;
    public float camLookSpeed;
    public float heightFromPlayer = 20f;
    public float heightMin, heightMax;
    public float zoomSpeed = 500f;
    public float smoothLook = 0.5f;

    //privately stored temp variables 
    float zoomInput;
    float vRot = 0, hRot;
    Vector3 lastCamRot, currentCamRot;
    Vector3 oldCameraPosition;
    Vector3 smoothCameraVelocity;
    Quaternion targetLook;

    [Header("Masks")]
    public LayerMask spriteFadeMask;
    public LayerMask obstructionMask;

    [Header("FOV")]
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
    }

    void Start()
    {
        //can move at start 
        canMoveCam = true;
        //set start height to middle val 
        heightFromPlayer = heightMin + ((heightMax - heightMin) / 2);
        //calc start pos
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * heightFromPlayer);
        //set new cam pos 
        cameraT.position = futureCameraPosition;

        //get the look rotation
        targetLook = Quaternion.LookRotation((player.transform.position + new Vector3(yLookPosAdjust, 0, 0))
            - transform.position, gravityBody.GetUp());
        //set start look
        transform.rotation = targetLook;
    }

    void Update () {

        if (canMoveCam)
        {
            HorizontalRotation();

            VerticalRotation();

            ZoomInputs();

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

    void HorizontalRotation()
    {
        // Rotate player around Y axis
        hRot = Input.GetAxis("Mouse X") * mouseSensitivityX;
        //set old cam pos
        oldCameraPosition = cameraT.position;

        //rotate player 
        player.transform.RotateAround(player.transform.position, player.transform.up, hRot);
        //reset cam pos 
        cameraT.position = oldCameraPosition;
    }

    void VerticalRotation()
    {
        // Rotate camera around X axis
        // Position player
        vRot += -1 * Input.GetAxis("Mouse Y") * mouseSensitivityY;
        vRot = Mathf.Clamp(vRot, minVerticalLookAngle, maxVerticalLookAngle);
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = player.transform.TransformPoint(toCamera * heightFromPlayer);
        //set new cam pos 
        cameraT.position = Vector3.SmoothDamp(cameraT.position, futureCameraPosition, ref smoothCameraVelocity, .5f);
        
        //get the look rotation
        targetLook = Quaternion.LookRotation((player.transform.position + new Vector3(yLookPosAdjust, 0, 0))
            - transform.position, gravityBody.GetUp());
        //lerp towards new look rotation 
        transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, smoothLook * Time.deltaTime);
    }

    void ZoomInputs()
    {
        zoomInput = Input.GetAxis("Mouse ScrollWheel");

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
    }

    void ZoomOut(float zoom)
    {
        float newHeight = heightFromPlayer + (zoomSpeed * Time.deltaTime * zoom);
        heightFromPlayer = Mathf.Lerp(heightFromPlayer, newHeight, zoomSpeed * Time.deltaTime);
    }

    //detects whether cam is seeing ground in front of player somehw
    void CastToPlayer()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 dir = player.transform.position - transform.position;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //send raycast
        if (Physics.Raycast(transform.position, dir, out hit, dist, obstructionMask))
        {
            //anytime we hit the planet ground, zoome out 
            if (hit.transform.gameObject.layer == 9)
            {
                if (heightFromPlayer < heightMax)
                    ZoomOut(0.05f);
            }
            //if it hits a tree, zoom in 
            if (hit.transform.gameObject.tag == "")
            {
                if (heightFromPlayer > heightMin)
                    ZoomIn(-0.025f);
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
