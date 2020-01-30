using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : AudioHandler {
    Orbit orbital;
    SphereCollider sphereT;
    PooledObject _pooledObj;

    [Header("Sonar Settings")]
    FishAI fishSender;
    public bool searching;
    public float sightRadius;
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
            SonarPulse();
        }
    }

    //uses an overlap sphere to see what's around me
    void SonarPulse()
    {
        //running gravity checks & calcs
        Collider[] visibleObjects = Physics.OverlapSphere(transform.position, sightRadius, lookingForMask);

        if(visibleObjects.Length > 0)
        {
            foundObject = visibleObjects[0].transform;
            searching = false;
            fishSender.objectOfInterest = foundObject;
            _pooledObj.ReturnToPool();
        }

        //play sonar sound 
        PlayRandomSoundRandomPitch(sonarSounds, myAudioSource.volume);
    }

    public void SendSonar (Vector3 pos, Quaternion direction)
    {
        //set starting pos
        transform.position = pos;
        startingPos = pos;

        //set rotation
        transform.rotation = direction;
        orbital.SetOrbit(orbital.orbitalSpeed);

    }
}
