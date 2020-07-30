using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FollowCamera : MonoBehaviour
{
    Transform target;
    [HideInInspector]
    public PlayerController player;
  
    [Range(0f, 1f)]
    public float heightCenter = 0.4f;
    [SerializeField] float height;
    public Vector3 lookOffset;
    [Range(0, 1)]
    public float lookSpeed = 0.2f;
    public bool recenterAngle = false, recenterHeight = false;

    public float angle, radius;
    public float angleCenter = 0f;
    public float angleCenterSpeed = 0.01f;

    [SerializeField] Transform m_angleTarget;
    public Transform angleTarget
    {
        get
        {
            return m_angleTarget;
        }
        set
        {
            m_angleTarget = value;
        }
    }
    
    float realHeightCenter;
    float minHeight = -0.2f, maxHeight = 2f;

    [Header("Cam Speeds")]
    public float camSpeedX;
    public float camSpeedY;
    public float camFollowSpeed;
    public bool changingPlanet;
    float distanceFromPlayer;
    
    //Gravity variables
    Quaternion gravityAlignment = Quaternion.identity;
    Vector3 gravDir;
    float gravDirLerpVal, followSpeed;

    //need to figure out what this RemapNRB thing is - lines 53, 77, 123 

    private void Start()
    {
        //realHeightCenter = heightCenter.RemapNRB(0, 1, minHeight, maxHeight);
        height = realHeightCenter;
    }
    
    private void Update()
    {
        if (player == null)
            return;

        target = player.transform;

        //sometimes things get messed up if angle number gets to big so this way it stays within the 360 range
        if (angle > 360f) angle -= 360f;
        if (angle < 0) angle += 360f;

        Vector3 delta = Vector3.zero;//this is the actual swipe direction that we use to move the camera around
        //if (input check)
        {
           // delta = input * Time.deltaTime;
        }
        
        angle -= delta.x * camSpeedX * 0.01f;//angle moves the camera around horizontally
        height -= delta.y * camSpeedY * 0.01f;//height adjusts the camera's height in relation to the player
        height = Mathf.Clamp(height, minHeight, maxHeight);
        //realHeightCenter = heightCenter.RemapNRB(0, 1, minHeight, maxHeight);
        radius = distanceFromPlayer;//radius is how far away cam is from player
    }

    void FixedUpdate()
    {
        if (target == null) return;

        //this is mainly for a multiple planets setup, so camera is super smooth when changing gravity sources
        gravDirLerpVal = Mathf.Lerp(gravDirLerpVal, 0.2f, 0.1f);
        if (changingPlanet)
            gravDirLerpVal = 0.05f;

        gravDir = Vector3.Lerp(gravDir, -target.up, gravDirLerpVal);//gets grav dir (smoothed so its all nice)

        //get the quaternion for this gravity direction, so that camera up = player up
        gravityAlignment = Quaternion.FromToRotation(gravityAlignment * Vector3.up, -gravDir) * gravityAlignment;

        if (recenterHeight)
        {
            if (player.moveState != PlayerController.MoveStates.IDLE)
                height = Mathf.Lerp(height, realHeightCenter, 0.006f);//recenter with time
            else
                height = Mathf.Lerp(height, realHeightCenter, 0.0007f);
        }

        if (recenterAngle || (angleTarget != null && angleTarget != target))
        {
            float newAng = angleCenter;
            if (angleTarget != null)
            {
                //angle the camera to circle around player but aim towards angleTarget.
                //sorry for this complicated line- math is hard to explain but it just works ok?
                newAng = Vector3.SignedAngle(gravityAlignment * Vector3.left, Vector3.ProjectOnPlane(angleTarget.position - target.position, gravDir), gravDir);
            }
            angle = Mathf.LerpAngle(angle, newAng, angleCenterSpeed);
        }

        //this big complicated chunk of code calculates the camera's position
        float ang = angle * Mathf.Deg2Rad;
        Vector3 aa = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
        aa = gravityAlignment * aa;
        //Debug.DrawLine(target.transform.position, target.transform.position + (aa.normalized * 5f), Color.magenta, 10f);
        float distFromTarg = radius * height; //.Remap(minHeight, maxHeight, 0.4f, 1.6f);
        distFromTarg = Mathf.Max(distFromTarg, 15f);
        Vector3 pos = target.transform.position + (aa + target.transform.up * height).normalized * distFromTarg;
        followSpeed = Mathf.Lerp(followSpeed, Mathf.Clamp01(camFollowSpeed), 0.1f);
        if (changingPlanet)
            followSpeed = Mathf.Clamp01(camFollowSpeed) / 4f;
        transform.position = Vector3.Lerp(transform.position, pos, followSpeed);
        //Debug.DrawLine(target.transform.position, target.transform.position + (zDir + target.transform.up * height).normalized * radius, Color.green, 10f);

        Vector3 targetLook = target.transform.position + transform.TransformVector(lookOffset);
        Quaternion lookAtT = Quaternion.LookRotation(targetLook - transform.position, -gravDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtT, lookSpeed);
    }

    public void clearAngleTarget() { angleTarget = null; }
}