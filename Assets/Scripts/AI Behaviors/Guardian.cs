using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : AudioHandler {
    //player vars 
    GameObject player;
    PlayerController pc;
    float distFromPlayer, lastDist;
    float distFromNextPoint;
    float distFromPlayerToNextPoint;
    public float necDist = 10f;
    Vector3 lastPos, currentPos;

    //my behaviors  
    Transform gSpriteHolder;
    SpriteRenderer gRenderer;
    GuardianAnimation gAnimation;
    MoveTowards movement;
    Orbit orbital;
    GravityBody grav;
    ObstacleAvoidance obstacleAvoidance;
    Rigidbody rBody;
    TripActivation tripper;
    MonologueManager monoManager;
    MonologueTrigger monoTrigger;

    [Header("Guardian Behavior")]
    public GuardianStates guardianState;
    public enum GuardianStates
    {
        ORBITING, MOVING, WAITING, FOLLOWPLAYER,
    }
    //guardian locations for the current planet 
    public Transform[] guardianLocations;
    public bool[] guardianMonoChecks;
    public int[] guardianMonoIndeces;
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
        gSpriteHolder = transform.GetChild(0);
        gRenderer = gSpriteHolder.GetComponent<SpriteRenderer>();
        gAnimation = gSpriteHolder.GetComponent<GuardianAnimation>();
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        grav = GetComponent<GravityBody>();
        tripper = FindObjectOfType<TripActivation>();
        monoManager = GetComponent<MonologueManager>();
        monoTrigger = GetComponentInChildren<MonologueTrigger>();
        rBody = GetComponent<Rigidbody>();
        obstacleAvoidance = GetComponent<ObstacleAvoidance>();
    }

    void Start ()
    {
        SetWaiting();
	}
	
	void Update () {
        //dist calc
        currentPos = transform.position;
        //dist from guardian to player
        distFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //check so that we dont get indexOutOfRange
        if(currentPoint + 1 < guardianLocations.Length - 1)
        {
            //calcs dist from guardian to next point in array
            distFromNextPoint = Vector3.Distance(transform.position, guardianLocations[currentPoint + 1].position);
            //calc dist from player to my next point 
            distFromPlayerToNextPoint = Vector3.Distance(player.transform.position, guardianLocations[currentPoint + 1].position);
        }

        //WAITING
        if (guardianState == GuardianStates.WAITING)
        {
            //look towards player
            if(gAnimation.Animator.GetBool("swimAway"))
                gAnimation.Animator.SetBool("swimAway", false);

            //do nothing, play waiting sound 
            if (!myAudioSource.isPlaying)
                PlayRandomSoundRandomPitch(waitingSounds, 1f);

            //only split mono check if we are at least at 0
            if(currentPoint >= 0)
            {
                //monologue
                if (guardianMonoChecks[currentPoint] == true)
                {
                    //only set move once mono activated && monoMan is not active
                    if (monoTrigger.hasActivated && monoManager.inMonologue == false)
                    {
                        //move
                        SetMove();

                        //could add more logic to have reset for mono -- like is this a repeating hint?
                        //if so, we should attach set move conditions to whatever hint is regarding 
                        // i.e. whatever goal guardian is waiting for player to complete!
                    }

                    //distance checks
                    //not in monologue, has not talked, plus dist checks require me to go to next 
                    else if (monoManager.inMonologue == false && monoTrigger.hasActivated == false &&
                    (distFromPlayer > distFromNextPoint && distFromPlayerToNextPoint < distFromPlayer))
                    {
                        //set trigger off
                        if (currentPoint + 1 < guardianLocations.Length)
                        {
                            monoTrigger.hasActivated = true;
                        }

                        SetMove();
                    }
                }
                //no monologue -- simply wait for player to get near me 
                else
                {
                    DistFromPlayer();
                }
            }
            else
            {
                DistFromPlayer();
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
                SetWaiting();
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

    //checks dist from player and sets move 
    void DistFromPlayer()
    {
        //player close now
        if (distFromPlayer < necDist)
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
        //player is further from guardian than what is required to travel to next point 
        // && player is closer to my next point than i am to the player
        else if (distFromPlayer > distFromNextPoint && distFromPlayerToNextPoint < distFromPlayer)
        {
            SetMove();
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
            if(obstacleAvoidance)
                obstacleAvoidance.travelDest = guardianLocations[currentPoint];
            monoTrigger.hasActivated = true;
            guardianState = GuardianStates.MOVING;
        }
    }

    //when guardian reaches location, this is called
    void SetWaiting()
    {
        //reenable gravity 
        if (grav.enabled == false)
        {
            grav.enabled = true;
        }

        //set mono system 
        if(guardianMonoChecks.Length > 0 && currentPoint >= 0)
        {
            //only if we have a mono 
            if (guardianMonoChecks[currentPoint] == true)
            {
                //set mono system
                monoManager.SetMonologueSystem(guardianMonoIndeces[currentPoint]);

                //reset mono trigger
                monoTrigger.Reset();
            }
            //make it so you can't trigger anything by accidente
            else
            {
                monoTrigger.hasActivated = true;
            }
        }
        
        //set waiting state 
        guardianState = GuardianStates.WAITING;
    }

    //called to immediately move to a spot 
    public void MoveToLocationAndWaitForTrip(Transform location)
    {
        movement.MoveTo(location.position, movement.moveSpeed);
        guardianState = GuardianStates.MOVING;
        monoTrigger.gameObject.SetActive(false);
        tripper.canTrip = true;
        newGalaxy = true;
    }

    //called from planet manager when a planet is activated 
    public void ResetGuardianLocation(Vector3 startingPoint, GuardianBehaviorSets[] gBehaviors, Collider[] planets)
    {
        currentPoint = -1;
        transform.SetParent(null);

        //set array lengths
        guardianLocations = new Transform[gBehaviors.Length];
        guardianMonoChecks = new bool[gBehaviors.Length];
        guardianMonoIndeces = new int[gBehaviors.Length];

        //fill in g behavior arrays 
        for(int i = 0; i < gBehaviors.Length; i++)
        {
            guardianLocations[i] = gBehaviors[i].guardianLocation;
            guardianMonoChecks[i] = gBehaviors[i].hasMonologue;
            guardianMonoIndeces[i] = gBehaviors[i].monologueIndex;
        }

        //set grav
        grav.enabled = false;
        grav.planets = planets;

        //either teleport guardian directly to starting point
        if(movement.origSpeed > 0)
            movement.MoveTo(startingPoint, movement.origSpeed * 3);
        else
            movement.MoveTo(startingPoint, 50f);

        guardianState = GuardianStates.MOVING;
    }

    //teleport the guardian and wait at a position
    public void TeleportGuardian(Vector3 point)
    {
        transform.position = point;
        currentPoint = guardianLocations.Length - 1;

        SetWaiting();
    }

    //attaches guardian to player movement 
    void FollowPlayer()
    {
        transform.SetParent(player.transform);
        grav.enabled = false;
        guardianState = GuardianStates.FOLLOWPLAYER;
    }
}
