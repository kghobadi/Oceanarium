using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : AudioHandler {
    //player vars 
    GameObject player;
    PlayerController pc;
    float distFromPlayer, lastDist;
    public float necDist = 10f;
    Vector3 lastPos, currentPos;

    //my behaviors  
    SpriteRenderer gRenderer;
    GuardianAnimation gAnimation;
    MoveTowards movement;
    Orbit orbital;
    GravityBody grav;
    TripActivation tripper;
    [Header("Guardian Behavior")]
    public GuardianStates guardianState;
    public enum GuardianStates
    {
        ORBITING, MOVING, WAITING, CHANGINGPLANETS,
    }
    //guardian locations for the current planet 
    public Transform[] guardianLocations;
    public int currentPoint = 0;
    public bool newGalaxy;
    
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
        gRenderer = GetComponent<SpriteRenderer>();
        gAnimation = GetComponent<GuardianAnimation>();
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        grav = GetComponent<GravityBody>();
        tripper = GetComponent<TripActivation>();
    }

    void Start () {
        guardianState = GuardianStates.WAITING;
	}
	

	void Update () {
        //dist calc
        currentPos = transform.position;
        distFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //WAITING
		if(guardianState == GuardianStates.WAITING)
        {
            //look towards player
            if(gAnimation.Animator.GetBool("swimAway"))
                gAnimation.Animator.SetBool("swimAway", false);

            //do nothing, play waiting sound 
            if (!myAudioSource.isPlaying)
                PlaySoundRandomPitch(waitingSound, 1f);

            //player close now
            if(distFromPlayer < necDist)
            {
                //go to new galaxy
                if (newGalaxy)
                {
                    //player rides me 
                }
                //go to next point
                else
                {
                    SetMove();
                }
            }
        }

        //MOVING
        if(guardianState == GuardianStates.MOVING)
        {
            //swim away 
            if (!gAnimation.Animator.GetBool("swimAway"))
                gAnimation.Animator.SetBool("swimAway", true);

            // play swimming sound 
            if (!myAudioSource.isPlaying)
                PlaySoundRandomPitch(swimmingSound, 1f);
            //movement running
            if (movement.moving == false)
            {
                if(grav.enabled == false)
                {
                    grav.enabled = true;
                }
                guardianState = GuardianStates.WAITING;
            }
        }

        //CHANGING PLANETS
        if(guardianState == GuardianStates.CHANGINGPLANETS)
        {
            //swim away 
            if (!gAnimation.Animator.GetBool("swimAway"))
                gAnimation.Animator.SetBool("swimAway", true);
        }

        //store last dist& pos
        lastPos = transform.position;
        lastDist = distFromPlayer;
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

    //called to immediately move to a spot 
    public void MoveToLocationAndWaitForTrip(Transform location)
    {
        movement.MoveTo(location.position, movement.moveSpeed);
        guardianState = GuardianStates.MOVING;
        tripper.canTrip = true;
        newGalaxy = true;
    }

    //called from planet manager when a planet is activated 
    public void ResetGuardianLocation(Vector3 startingPoint, Transform[] locations, Collider[] planets)
    {
        currentPoint = -1;
        transform.SetParent(null);

        guardianLocations = locations;
        grav.enabled = false;
        grav.planets = planets;
        movement.MoveTo(startingPoint, movement.origSpeed * 3);
        guardianState = GuardianStates.MOVING;
    }

    //attaches guardian to player movement for Current transition 
    void ChangePlanets()
    {
        transform.SetParent(player.transform);
        grav.enabled = false;
        guardianState = GuardianStates.CHANGINGPLANETS;
    }
}
