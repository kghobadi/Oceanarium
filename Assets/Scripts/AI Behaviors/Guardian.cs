using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : AudioHandler {
    //player vars 
    GameObject player;
    PlayerController pc;
    float distFromPlayer;
    public float necDist = 10f;

    //my behaviors
    MoveTowards movement;
    Orbit orbital;
    GravityBody grav;
    [Header("Guardian Behavior")]
    public GuardianStates guardianState;
    public enum GuardianStates
    {
        ORBITING, MOVING, WAITING, CHANGINGPLANETS,
    }
    //guardian locations for the current planet 
    public Transform[] guardianLocations;
    public int currentPoint = 0;
    
    [Header("Sounds")]
    public AudioClip waitingSound;
    public AudioClip swimmingSound;

    public override void Awake()
    {
        base.Awake();
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();

        //ai component refs 
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        grav = GetComponent<GravityBody>();
    }

    void Start () {
        guardianState = GuardianStates.WAITING;
	}
	

	void Update () {
        //dist calc
        distFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //WAITING
		if(guardianState == GuardianStates.WAITING)
        {
            //do nothing, play waiting sound 
            if(!myAudioSource.isPlaying)
                PlaySoundRandomPitch(waitingSound, 1f);

            //player close now
            if(distFromPlayer < necDist || pc.animator.Animator.GetBool("inCurrent"))
            {
                SetMove();
            }
        }

        //MOVING
        if(guardianState == GuardianStates.MOVING)
        {  
            // play swimming sound 
            if (!myAudioSource.isPlaying)
                PlaySoundRandomPitch(swimmingSound, 1f);
            //movement running
            if (movement.moving == false)
            {
                guardianState = GuardianStates.WAITING;
            }
        }

        //CHANGING PLANETS
        if(guardianState == GuardianStates.CHANGINGPLANETS)
        {
            //current has ended
            if(currentPoint == 0 && pc.canMove == true)
            {
                transform.SetParent(null);
                grav.planets = player.GetComponent<GravityBody>().planets;
                grav.enabled = true;
                movement.MoveTo(guardianLocations[currentPoint].position, movement.moveSpeed);
                guardianState = GuardianStates.MOVING;
            }
        }
	}

    //sets move to next point in guardian locations 
    void SetMove()
    {
        //inc point 
        if(currentPoint < guardianLocations.Length - 1)
        {
            currentPoint++;
            movement.MoveTo(guardianLocations[currentPoint].position, movement.moveSpeed + currentPoint);
            guardianState = GuardianStates.MOVING;
        }
        //ran out of guardian points, so we are changing planets
        else
        {
            ChangePlanets();
        }
    }

    //attaches guardian to player movement for Current transition 
    void ChangePlanets()
    {
        transform.SetParent(player.transform);
        grav.enabled = false;
        guardianState = GuardianStates.CHANGINGPLANETS;
    }
}
