using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : AudioHandler {
    //player vars 
    GameObject player;
    PlayerController pc;
    float distFromPlayer;
    public float necDist = 10f;

    //my behaviors  
    SpriteRenderer fRenderer;
    CreatureSprites cSprites;
    CreatureAnimation fAnimation;
    MoveTowards movement;
    Orbit orbital;
    GravityBody grav;
    Rigidbody rBody;

    [Header("Fish Behaviors")]
    public FishStates currentState;
    public enum FishStates
    {
        ORBITING, MOVING, IDLE, FOLLOWING
    }

    [Header("Move To")]
    //pool of sonars
    public bool waitingForSonar;
    public ObjectPooler sonarPool;
    public Transform objectOfInterest;
    public LayerMask obstacles;
    public float elevationSpeed = 25f;

    [Header("Following Herd")]
    //for Following herd
    public Transform currentHerdObj;
    public float herdRadius = 10f;

    //creature audio
    [Header("Sounds")]
    public AudioClip[] swimSounds;
    public AudioClip[] fleeSounds;
    public float soundFreq = 1, fleeFreq;
    float soundTimer;

   
    public override void Awake()
    {
        base.Awake();
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();

        //visual component refs 
        fRenderer = GetComponentInChildren<SpriteRenderer>();
        cSprites = GetComponentInChildren<CreatureSprites>();
        fAnimation = GetComponentInChildren<CreatureAnimation>();

        //ai component refs 
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        grav = GetComponent<GravityBody>();
        rBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        SetIdle();
    }

    void Update ()
    {
        distFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //just for playing creature audio
        if (currentState == FishStates.ORBITING)
        {
            SoundCountdown(soundFreq, swimSounds);
        }

        //IDLE searching
        if(currentState == FishStates.IDLE)
        {
            if (!waitingForSonar)
            {
                SonarSearch();
            }
        }

        //MOVING
        if (currentState == FishStates.MOVING)
        {
            //look at point of interest with grav up
            transform.LookAt(objectOfInterest.position, grav.GetUp());
            //send rays looking for obstacles so i can swim over them
            CheckForward();
            //sound 
            SoundCountdown(soundFreq, swimSounds);

            //movement has finished -- IDLE
            if (movement.moving == false)
            {
                SetIdle();
            }
        }

        //FOLLOWING
        if(currentState == FishStates.FOLLOWING)
        {
            float distFromHerdObj = Vector3.Distance(transform.position, currentHerdObj.transform.position);

            //dist greater than 
            if(distFromHerdObj > herdRadius && !movement.moving)
            {
                Vector3 point = currentHerdObj.position + Random.insideUnitSphere * 5;
                movement.MoveTo(point, movement.moveSpeed);
            }
        }
    }

    //sets fish to idle
    void SetIdle()
    {
        currentState = FishStates.IDLE;
        grav.enabled = true;
    }

    //use sonar search to found next obj of interest 
    void SonarSearch()
    {
        transform.Rotate(0f, 15f, 0f);
        GameObject sonar = sonarPool.GrabObject();
        sonar.GetComponent<Sonar>().SendSonar(this, transform.position, transform.rotation, orbital.planetToOrbit);
        waitingForSonar = true;
    }

    //shoots ray forward looking for obstacles 
    void CheckForward()
    {
        Vector3 direction = objectOfInterest.position - transform.position;
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 1f, direction, out hit, 15f, obstacles))
        {
            //elevate if hit    
            //Debug.Log("hit");
            Elevate();
        }
    }

    //called when ray forward hits obstacle
    void Elevate()
    {
        rBody.AddForce(grav.GetUp() * elevationSpeed);
    }

    //fish will simply orbit current planet 
    void SetOrbitSwim()
    {
        currentState = FishStates.ORBITING;
        grav.enabled = false;
        //randomize orbit axis again and turn it back on
        orbital.RandomizeOrbitAxis();
        orbital.orbiting = true;
    }

    //called when sonar finds something 
    public void FoundObject(Transform obj)
    {
        objectOfInterest = obj;
        SetMoveTo(objectOfInterest.position);
        waitingForSonar = false;
    }

    //use grav + move towards to take fish to a specific location
    void SetMoveTo(Vector3 point)
    {
        currentState = FishStates.MOVING;
        grav.enabled = true;
        movement.MoveTo(point, movement.moveSpeed);
    }

    //use this to make fish follow a herd object's orders
    void SetFollowing(Transform herdObj)
    {
        currentState = FishStates.FOLLOWING;
        grav.enabled = true;

        Vector3 point = herdObj.position + Random.insideUnitSphere * 5;
        movement.MoveTo(point, movement.moveSpeed);
    }

    //counts down to play sounds at specific frequencies 
    void SoundCountdown(float resetTime, AudioClip[] sounds)
    {
        soundTimer -= Time.deltaTime;
        if (soundTimer < 0)
        {
            PlayRandomSound(sounds, 1f);
            soundTimer = resetTime;
        }
    }
}
