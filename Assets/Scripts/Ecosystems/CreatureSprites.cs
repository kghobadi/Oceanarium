using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//any edible Creature or moving creature will need this to correct it's sprite in the world 
public class CreatureSprites : MonoBehaviour {
    SpriteRenderer mySR;
    EdibleCreature edibleCreature;
    Animator creatureAnimator;
    Camera playerCam;
    CameraController camControl;

    float checkXtimer;
    Vector3 lastViewPos;

    void Awake ()
    {
        //player refs
        playerCam = Camera.main;
        camControl = playerCam.GetComponent<CameraController>();
        //creature refs
        edibleCreature = GetComponent<EdibleCreature>();
        creatureAnimator = GetComponent<Animator>();
        creatureAnimator.enabled = false;
        //set my vars
        mySR = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        checkXtimer = 0.1f;

        StartCoroutine(StartAnimator());
    }

    void Update () {
        //if i am visible to the camera AND i am a moving creature
        if (mySR.isVisible && !camControl.isMovingCam)
        {
            checkXtimer -= Time.deltaTime;

            if (checkXtimer < 0)
            {
                CheckXDirection();
            }
        }
    }

    //called when the Renderer comp becomes visible to any camera
    void OnBecameVisible()
    {
        CheckXDirection();
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
        checkXtimer = 0.1f;
    }

    IEnumerator StartAnimator()
    {
        float randomWait = Random.Range(0.1f, 0.5f);
        yield return new WaitForSeconds(randomWait);
        creatureAnimator.enabled = true;
    }
}
