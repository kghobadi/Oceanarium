using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour {
    //player ref
    GameObject player;
    ThirdPersonController tpc;

    //all the effect variables
    public int currentPoint = -1;
    public float boatStartSpeed, boatChaseSpeed, boatEndSpeed;
    float boatMoveSpeed;
    public Transform[] movePoints;
    public bool moving, lerpingFOV, posOrNegLerp;
    float desiredFOV;
    public Transform departurePoint;

    //audio
    AudioSource boatAudio;
    public AudioClip boatNormal, boatChorus, boatScary;

    //finale stuff
    bool hasScreamed;
    public DialogueText[] cuttleScreams;
    public GameObject currentBubble;
    public GameObject cuttle, currentToLeave, lightDialogueTrigger;
    public DialogueText cuttlesFirstDialogue;
    bool turningOff;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        tpc = player.GetComponent<ThirdPersonController>();

        boatAudio = GetComponent<AudioSource>();
        boatMoveSpeed = boatStartSpeed;
        cuttle.SetActive(false);
	}

    void FixedUpdate()
    {
        //actual movement code -- when reach point, SetMove() to next point
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoints[currentPoint].position, boatMoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoints[currentPoint].position) < 5)
            {
                SetMovement();
            }
        }
    }

    void Update () {
       

        //change look rotation so player can look up at boat 
        if(currentPoint >= 0 && currentPoint <2)
        {
            tpc.minVerticalLookAngle = -50;
            tpc.transform.position = Vector3.MoveTowards(tpc.transform.position, departurePoint.position, 5 * Time.deltaTime);
        }

        //stop fish
        if(currentPoint == 2)
        {
            tpc.minVerticalLookAngle = -10;
            tpc.fishSwimSpeed = 0;
            //set animation state to SPRINT
            tpc.animator.SetBool("sprinting", false);
            //for the school surrounding you
            for (int i = 0; i < tpc.extraFishAnimators.Length; i++)
            {
                tpc.extraFishAnimators[i].SetBool("sprinting", false);
            }
            boatMoveSpeed = 100;

            //activate cuttle screams and cuttle eat
            //queue ending dialogue and cuttle's appearance
            if (!hasScreamed)
            {
                for(int i = 0; i < cuttleScreams.Length; i++)
                {
                    cuttleScreams[i].EnableDialogue();
                }

                cuttle.SetActive(true);

                hasScreamed = true;

                StartCoroutine(WaitToTurnOff());
            }
        }

        //for lerping cam fov
        if (lerpingFOV)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, desiredFOV, 5 * Time.deltaTime);

            //when to stop lerp
            if (posOrNegLerp)
            {
                if (Camera.main.fieldOfView > desiredFOV - 1)
                {
                    lerpingFOV = false;
                }
            }
            else
            {
                if (Camera.main.fieldOfView < desiredFOV + 1)
                {
                    lerpingFOV = false;
                }
            }
            
        }
	}

    public void SetMovement()
    {
        //move to next point
        if(currentPoint < movePoints.Length)
        {
            currentPoint++;
        }
        //turn off
        else
        {
            StartCoroutine(WaitToTurnOff());
        }

        //set boat start speed
        if(currentPoint == 0)
        {
            boatMoveSpeed = 15;
        }
        
        //rotate boat as it traverses the planet
        if(currentPoint >= 1)
        {
            transform.Rotate(0, 0, -25);
        }
        //make fish look at boat
        if(currentPoint >= 1 && currentPoint <= 3)
        {
            tpc.transform.LookAt(transform.position);
            tpc.transform.localEulerAngles = new Vector3(-32, 45, 118);
        }

        //change boat sound
        if (currentPoint == 3)
        { 
            boatAudio.Stop();
            boatAudio.clip = boatChorus;
            boatAudio.Play();
            boatMoveSpeed = boatChaseSpeed;
            
        }
        //change to final scary sound effect
        if (currentPoint == 5)
        {
            boatAudio.Stop();
            boatAudio.clip = boatScary;
            boatAudio.Play();
            boatMoveSpeed = boatEndSpeed;
        }
        

        moving = true;
    }

    IEnumerator WaitToTurnOff()
    {

        yield return new WaitForSeconds(1f);

        //activate Cuttle's only line of dialogue
        cuttlesFirstDialogue.EnableDialogue();
        currentToLeave.SetActive(true);
       
        currentBubble.SetActive(true);
        
        lightDialogueTrigger.SetActive(true);

        tpc.audioSpectrum.GetComponent<PostProcessor>().ResetPostProcessing();
        tpc.audioSpectrum.SetActive(false);


        gameObject.SetActive(false);
    }
}
