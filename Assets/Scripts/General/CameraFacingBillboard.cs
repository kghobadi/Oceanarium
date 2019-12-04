using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    SpriteRenderer mySR;
    Vector3 lastViewPos;
    PlayerController pc;
    private GravityBody playerBody;
    Camera playerCam;
    CameraController camControl;
    EdibleCreature edibleCreature;
    Animator creatureAnimator;

    float checkXtimer;
    public bool movingCreature;
    public bool fadingBack;

	void Awake(){
        //player refs
		playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<GravityBody>();
        pc = playerBody.GetComponent<PlayerController>();
        playerCam = Camera.main;
        camControl = playerCam.GetComponent<CameraController>();
        //set my vars
        mySR = GetComponent<SpriteRenderer>();
        checkXtimer = 0.1f;

        if (movingCreature )
        {
            edibleCreature = GetComponent<EdibleCreature>();
            creatureAnimator = GetComponent<Animator>();
            creatureAnimator.enabled = false;
            
            StartCoroutine(StartAnimator());
        }
	}

	void Update(){
        transform.LookAt(playerCam.transform.position, playerBody.GetUp());

        //if i am visible to the camera AND i am a moving creature
        if (mySR.isVisible && movingCreature && !camControl.isMovingCam)
        {
            checkXtimer -= Time.deltaTime;

            if (checkXtimer < 0)
            {
                CheckXDirection();
            }
        }

        //lerps alpha back after Fade for obstruction
        if (fadingBack)
        {
            Color alphaVal = GetComponent<SpriteRenderer>().color;
            alphaVal.a = Mathf.Lerp(alphaVal.a, 1, Time.deltaTime);
            GetComponent<SpriteRenderer>().color = alphaVal;

            if(alphaVal.a > 0.99f)
            {
                fadingBack = false;
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
        if(creatureAnimator)
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

    //called by camera on Objects which are stagnant in env and might block view of player
    public void Fade()
    {
        StopAllCoroutines();
        fadingBack = false;

        Color alphaVal = GetComponent<SpriteRenderer>().color;
        alphaVal.a = 0.2f;
        GetComponent<SpriteRenderer>().color = alphaVal;
        GetComponent<BoxCollider>().isTrigger = true;

        StartCoroutine(FadeForCamera());
    }

    IEnumerator StartAnimator()
    {
        float randomWait = Random.Range(0.1f, 0.5f);
        yield return new WaitForSeconds(randomWait);
        creatureAnimator.enabled = true;
    }

    //fades object back to normal alpha when no longer obstructing view of player
    IEnumerator FadeForCamera()
    {
        yield return new WaitForSeconds(1);

        fadingBack = true;

        GetComponent<BoxCollider>().isTrigger = false;
    }
}
