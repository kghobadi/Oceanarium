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
        ORBITING, MOVING, WAITING, FOLLOWPLAYER,
    }
    //guardian locations for the current planet 
    public Transform[] guardianLocations;
    public int currentPoint = 0;
    public bool newGalaxy;
    
    [Header("Sounds")]
    public AudioClip [] waitingSounds;
    public AudioClip [] swimmingSounds;

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
                PlayRandomSoundRandomPitch(waitingSounds, 1f);

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
                    
                    //inc point 
                    if (currentPoint < guardianLocations.Length - 1)
                    {
                        SetMove();
                    }
                    //ran out of guardian points, so we are changing planets
                    else
                    {
                        //just waiting 
                    }
                    
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
                PlayRandomSoundRandomPitch(swimmingSounds, 1f);
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
        if(guardianState == GuardianStates.FOLLOWPLAYER)
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

        //either teleport guardian directly to starting point
        if(movement.origSpeed > 0)
            movement.MoveTo(startingPoint, movement.origSpeed * 3);
        else
            movement.MoveTo(startingPoint, 30f);

        guardianState = GuardianStates.MOVING;
    }

    //teleport the guardian and wait at a position
    public void TeleportGuardian(Vector3 point)
    {
        transform.position = point;
        currentPoint = guardianLocations.Length - 1;
        guardianState = GuardianStates.WAITING;
    }

    //attaches guardian to player movement 
    void FollowPlayer()
    {
        transform.SetParent(player.transform);
        grav.enabled = false;
        guardianState = GuardianStates.FOLLOWPLAYER;
    }
}
