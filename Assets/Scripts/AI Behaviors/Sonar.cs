using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : AudioHandler {
    Orbit orbital;
    SphereCollider sphereT;
    PooledObject _pooledObj;

    FishAI fishSender;
    [Header("Sonar Settings")]
    public bool searching;
    public bool canCheckDist;
    public float sightRadius;
    public float distFromStart;
    public float distToReturn = 10f;
    public Vector3 startingPos;
    public LayerMask lookingForMask;
    public Transform foundObject;

    [Header("Sounds")]
    public AudioClip[] sonarSounds;

    public override void Awake()
    {
        base.Awake();

        orbital = GetComponent<Orbit>();
        sphereT = GetComponent<SphereCollider>();
    }

    void Start()
    {
        _pooledObj = GetComponent<PooledObject>();
    }

    void Update()
    { 
        //has been sent on an orbital path
        if (searching)
        {
            //dist from start pos 
            distFromStart = Vector3.Distance(transform.position, startingPos);

            //set can check
            if(distFromStart > distToReturn && canCheckDist == false)
            {
                canCheckDist = true;
            }

            //don't want to pulse too close 
            if (canCheckDist)
            {
                //pulse...
                SonarPulse();
            }

            //orbited planet a full cycle
            if (canCheckDist && distFromStart < distToReturn)
            {
                fishSender.waitingForSonar = false;
                ReturnSonar();
            }
        }
    }

    //uses an overlap sphere to see what's around me
    void SonarPulse()
    {
        //running gravity checks & calcs
        Collider[] visibleObjects = Physics.OverlapSphere(transform.position, sightRadius, lookingForMask);

        //found something? 
        if(visibleObjects.Length > 0)
        {
            //set found obj
            foundObject = visibleObjects[0].transform;
            fishSender.FoundObject(foundObject);
            //turn off
            ReturnSonar();
        }

        //play sonar sound 
        if(myAudioSource.isPlaying == false)
            PlayRandomSoundRandomPitch(sonarSounds, myAudioSource.volume);
    }

    void ReturnSonar()
    {
        searching = false;
        orbital.orbiting = false;
        _pooledObj.ReturnToPool();
    }

    public void SendSonar (FishAI sender, Vector3 pos, Quaternion direction, Transform planet)
    {
        //set starting pos
        transform.position = pos;
        startingPos = pos;
        canCheckDist = false;
        fishSender = sender;

        //set rotation
        transform.rotation = direction;
        orbital.planetToOrbit = planet;
        orbital.SetOrbit(orbital.orbitalSpeed);
        searching = true;
    }

}
