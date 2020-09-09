using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//a creature with this script also needs:
//Orbit script, 
//a rigidbody (set to Kinematic), 
//a sphereCollider set to Trigger
[RequireComponent(typeof(Orbit))]
public class EdibleCreature : AudioHandler {
    //ref to player and spawner on this planet
    [HideInInspector]
    public CreatureSpawner mySpawner;
    GameObject player;
    PlayerController pc;
    Transform spriteHolder;

    //ref to orbit script (uses this during swimming state)
    Orbit myOrbiter;

    //for fleeing
    float currentDistance;
    public float fleeDistance, fleeSpeed = 25f;
    Vector3 fleePos, originalPos;

    float fleeWaitTimer;

    //dictionary to sort nearby audio sources by distance 
    Dictionary<Transform, float> fleePossibilities = new Dictionary<Transform, float>();

    //set publicly to determine place in ecosystem
    public enum CreatureTypes
    {
        SMALLFISH, CUDDLE, JELLY,
    }

    //this creature's AI states
    public enum States
    {
        SWIMMING, FLEEING, RETURNING, DEATH,
    }

    //actual enum holders
    [Header("Type & States")]
    public CreatureTypes myType;
    public States currentState;

    //creature audio
    [Header("Sounds")]
    public AudioClip[] swimSounds;
    public AudioClip[] fleeSounds;
    public float soundFreq = 1, fleeFreq;
    float soundTimer;

    //creature animation
    CreatureAnimation creatureAnimator;
    [Header("Vis FX")]
    public GameObject eatingChunks;

    public override void Awake() {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        currentState = States.SWIMMING;
        myOrbiter = GetComponent<Orbit>();
        spriteHolder = transform.GetChild(0);
        creatureAnimator = spriteHolder.GetComponent<CreatureAnimation>();
        creatureAnimator.SetAnimator("swimming");
        fleeWaitTimer = Random.Range(0.25f, 0.5f);
        soundTimer = soundFreq;
    }

    void Start()
    {
        SetSwimming();
    }

    //check distance for fleeing
    void Update()
    {
        currentDistance = Vector3.Distance(transform.position, player.transform.position);

        //when player approaches AND I am not fleeing && player is predator, flee!
        if (currentDistance < fleeDistance && currentState != States.FLEEING && currentState != States.DEATH)
        {
            Flee();
        }

        //just for playing creature audio
        if (currentState == States.SWIMMING)
        {
            SoundCountdown(soundFreq);
        }

        //what we do while Fleeing
        if (currentState == States.FLEEING)
        {
            SoundCountdown(fleeFreq);

            //move towards flee point
            transform.position = Vector3.MoveTowards(transform.position, fleePos, fleeSpeed * Time.deltaTime);

            float distanceTilSafe = Vector3.Distance(transform.position, fleePos);

            //if we reached it, set back to swimming state
            if (distanceTilSafe < 0.5f)
            {
                fleeWaitTimer -= Time.deltaTime;

                if (fleeWaitTimer < 0)
                {
                    currentState = States.RETURNING;
                    fleeWaitTimer = Random.Range(0.25f, 0.5f);
                    creatureAnimator.SetAnimator("swimming");
                }

            }
        }

        //return to planet
        if (currentState == States.RETURNING)
        {
            SoundCountdown(soundFreq);

            //move towards return hit
            transform.position = Vector3.MoveTowards(transform.position, originalPos, myOrbiter.orbitalSpeed * Time.deltaTime);

            float distanceTilHome = Vector3.Distance(transform.position, originalPos);

            //if we reached it, set back to swimming state
            if (distanceTilHome < 1f)
            {
                SetSwimming();
            }
        }

        //vortex towards player mouth
        if (currentState == States.DEATH)
        {
            //Vector3 mouthPos = predator.position;

            //transform.position = Vector3.MoveTowards(transform.position, mouthPos, fleeSpeed * 2 * Time.deltaTime);

            //if (Vector3.Distance(transform.position, mouthPos) < 1f)
            //{
                //if (!predator.eating)
                //{
                //    PredatorEatsMe();
                //}
                //else
                //{
                //    Debug.Log("I'm already eating!");
                //}
            //}
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerMouth")
        {
            //if i am small fish
            if (myType == CreatureTypes.SMALLFISH)
            {
                //get eaten when player is cuddle

            }
        }
    }

    void SetSwimming()
    {
        currentState = States.SWIMMING;
        //randomize orbit axis again and turn it back on
        myOrbiter.RandomizeOrbitAxis();
        myOrbiter.orbiting = true;
    }

    //call when player is eating
    //This should only happen if player  is NOT currently eating
    void PredatorEatsMe()
    {
        //play eating animation for this one, eating sound

        //take me out of my spawner list
        mySpawner.activeCreatures.Remove(gameObject);

        //spawn eating chunks
        Instantiate(eatingChunks, transform.position, Quaternion.identity);

        //fucking END ME
        Destroy(gameObject);
    }


    //time to flee!!!
    //this will call multiple times until we find an acceptable swimPos
    void Flee()
    {
        originalPos = transform.position;

        myOrbiter.orbiting = false;

        fleePossibilities.Clear();

        for (int i = 0; i < mySpawner.fleePositions.Length; i++)
        {
            //check distance and add to list
            float distanceAway = Vector3.Distance(mySpawner.fleePositions[i].position, transform.position);
            //add to audiosource and distance to dictionary
            fleePossibilities.Add(mySpawner.fleePositions[i], distanceAway);
        }


        //sort the dictionary by order of ascending distance away
        foreach (KeyValuePair<Transform, float> item in fleePossibilities.OrderBy(key => key.Value))
        {
            fleePos = item.Key.position + Random.insideUnitSphere * 15;
            //Debug.Log(fleePos);
            break;
        }

        currentState = States.FLEEING;
        creatureAnimator.SetAnimator("fleeing");
    }


    void SoundCountdown(float resetTime)
    {
        soundTimer -= Time.deltaTime;
        if (soundTimer < 0)
        {
            PlayRandomSound(swimSounds, 1f);
            soundTimer = resetTime;
        }
    }
}
