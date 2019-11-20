using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//a creature with this script also needs:
//Orbit script, 
//a rigidbody (set to Kinematic), 
//a sphereCollider set to Trigger

public class EdibleCreature : MonoBehaviour {
    //ref to player and spawner on this planet
    public CreatureSpawner mySpawner;
    public GameObject player;
    public Transform playerMouth;
    ThirdPersonController tpc;

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
        SMALLFISH, CUDDLE, FAIRYDRAGON,
    }

    //this creature's AI states
    public enum States
    {
        SWIMMING, FLEEING, RETURNING, DEATH,
    }

    //actual enum holders
    public CreatureTypes myType;
    public ThirdPersonController.CreatureType predatorType;
    public States currentState;

    //creature audio
    AudioSource creatureAudio;
    public AudioClip[] swimSounds, fleeSounds;
    public float soundFreq = 1, fleeFreq;
    float soundTimer;
    public GameObject swimEmphNormal, swimEmphFlipped;

    //creature animation
    Animator creatureAnimator;
    public GameObject eatingChunks;

    //essenz tracking
    public int essenzCount;
    public GameObject essenzPrefab;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMouth = GameObject.FindGameObjectWithTag("PlayerMouth").transform;
        tpc = player.GetComponent<ThirdPersonController>();
        currentState = States.SWIMMING;
        //setting this for now
        predatorType = ThirdPersonController.CreatureType.CUDDLE;
        myOrbiter = GetComponent<Orbit>();
        creatureAudio = GetComponent<AudioSource>();
        creatureAnimator = GetComponent<Animator>();
        creatureAnimator.SetBool("fleeing", false);
        creatureAnimator.SetBool("swimming", true);
        fleeWaitTimer = Random.Range(0.25f, 0.5f);
        soundTimer = soundFreq;
    }

    //check distance for fleeing
    void Update()
    {
        currentDistance = Vector3.Distance(transform.position, player.transform.position);
        
        //when player approaches AND I am not fleeing && player is predator, flee!
        if (currentDistance < fleeDistance && currentState != States.FLEEING && currentState != States.DEATH
            && tpc.currentCreature == predatorType)
        {
            Flee();
        }

        //just for playing creature audio
        if(currentState == States.SWIMMING)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer < 0)
            {
                PlaySound(swimSounds);
                soundTimer = soundFreq;
            }
        }
        
        //what we do while Fleeing
        if(currentState == States.FLEEING)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer < 0)
            {
                PlaySound(swimSounds);
                soundTimer = fleeFreq;
            }

            //move towards flee point
            transform.position = Vector3.MoveTowards(transform.position, fleePos, fleeSpeed * Time.deltaTime);

            float distanceTilSafe = Vector3.Distance(transform.position, fleePos);

            //if we reached it, set back to swimming state
            if (distanceTilSafe < 0.5f)
            {
                fleeWaitTimer -= Time.deltaTime;

                if(fleeWaitTimer < 0)
                {
                    currentState = States.RETURNING;
                    fleeWaitTimer = Random.Range(0.25f, 0.5f);
                    creatureAnimator.SetBool("fleeing", false);
                    creatureAnimator.SetBool("swimming", true);
                }
               
            }
        }

        //return to planet
        if(currentState == States.RETURNING)
        {
            soundTimer -= Time.deltaTime;
            if (soundTimer < 0)
            {
                PlaySound(swimSounds);
                soundTimer = soundFreq;
            }

            //move towards return hit
            transform.position = Vector3.MoveTowards(transform.position, originalPos, myOrbiter.orbitalSpeed * Time.deltaTime);

            float distanceTilHome = Vector3.Distance(transform.position, originalPos);

            //if we reached it, set back to swimming state
            if (distanceTilHome < 1f)
            {
                currentState = States.SWIMMING;
                //randomize orbit axis again and turn it back on
                myOrbiter.RandomizeOrbitAxis();
                myOrbiter.orbiting = true;
            }
        }

        //vortex towards player mouth
        if(currentState == States.DEATH)
        {
            Vector3 mouthPos = playerMouth.position;

            transform.position = Vector3.MoveTowards(transform.position, mouthPos, fleeSpeed * 2 * Time.deltaTime);

            if(Vector3.Distance(transform.position, mouthPos) < 1f)
            {
                if (!tpc.eating)
                {
                    PlayerEatsMe();
                }
                else
                {
                    Debug.Log("I'm already eating!");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerMouth")
        {
            //if i am small fish
            if (myType == CreatureTypes.SMALLFISH)
            {
                //get eaten when player is cuddle
                if (tpc.currentCreature == ThirdPersonController.CreatureType.CUDDLE)
                {
                    currentState = States.DEATH;
                }
            }
        }
    }

    //call when player is eating
    //This should only happen if player  is NOT currently eating
    void PlayerEatsMe()
    {
        //play eating animation for this one, eating sound
        tpc.animator.SetTrigger("eat");
        tpc.PlaySound(tpc.eatingSounds);
        //Debug.Log("eating");

        //take me out of my spawner list
        mySpawner.activeCreatures.Remove(gameObject);

        //spawn eating chunks
        Instantiate(eatingChunks, playerMouth.position, Quaternion.identity);

        //spawn essenz
        for(int i = 0; i < essenzCount; i++)
        {
            Vector3 randomSpawnPos = transform.position + Random.insideUnitSphere * 3;
            Instantiate(essenzPrefab, randomSpawnPos, Quaternion.identity);
        }

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
        creatureAnimator.SetBool("fleeing", true);
        creatureAnimator.SetBool("swimming", false);
    }

    //called to play sounds 
    public void PlaySound(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);
        creatureAudio.PlayOneShot(soundArray[randomSound]);
    }

}
