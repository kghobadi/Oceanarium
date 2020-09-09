using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//any edible Creature or moving creature will need this to correct it's sprite in the world 
public class CreatureSprites : MonoBehaviour {
    SpriteRenderer mySR;
    Animator creatureAnimator;
    Camera playerCam;
    CameraController camControl;
    Vector3 lastViewPos;

    float checkTimer, checkTotal = 0.1f;

    public bool checksX = true;
    public bool checksY;

    void Awake ()
    {
        //player refs
        playerCam = Camera.main;
        camControl = playerCam.GetComponent<CameraController>();
        //creature refs
        creatureAnimator = GetComponent<Animator>();
        creatureAnimator.enabled = false;
        //set my vars
        mySR = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        StartCoroutine(StartAnimator());
    }

    void Update () {
        //if i am visible to the camera AND camera angle is not being moved by player
        if (mySR.isVisible && camControl.isMovingCamAngle == false)
        {
            checkTimer -= Time.deltaTime;

            if(checkTimer < 0)
            {
                if (checksX)
                    CheckXDirection();
                if (checksY)
                    CheckYDirection();

                checkTimer = checkTotal;
            }
        }
    }

    //called when the Renderer comp becomes visible to any camera
    void OnBecameVisible()
    {
        //make animation wait a sec before starting 
        if (creatureAnimator)
            StartCoroutine(StartAnimator());
    }


    //called when the Renderer comp becomes visible to any camera
    void OnBecameInvisible()
    {
        //stop animator
        if (creatureAnimator)
            creatureAnimator.enabled = false;
    }

    //called to flip sprite based on creatures movement
    void CheckXDirection()
    {
        Vector3 viewPos = playerCam.WorldToViewportPoint(transform.position);

        //we are moving to the right on the view plane
        if (viewPos.x > lastViewPos.x)
        {
            mySR.flipX = false;
        }
        //we are moving to the left on the view plane
        else if (viewPos.x < lastViewPos.x)
        {
            mySR.flipX = true;
        }

        //reset lastViewPos and timer
        lastViewPos = viewPos;
    }

    //called to flip sprite based on creatures movement
    void CheckYDirection()
    {
        Vector3 viewPos = playerCam.WorldToViewportPoint(transform.position);

        //we are moving to the top on the view plane
        if (viewPos.y > lastViewPos.y)
        {
            mySR.flipY = false;
        }
        //we are moving to the bottom on the view plane
        else if (viewPos.y < lastViewPos.y)
        {
            mySR.flipY = true;
        }

        //reset lastViewPos and timer
        lastViewPos = viewPos;
    }

    IEnumerator StartAnimator()
    {
        float randomWait = Random.Range(0.1f, 0.5f);
        yield return new WaitForSeconds(randomWait);
        creatureAnimator.enabled = true;
    }
}
