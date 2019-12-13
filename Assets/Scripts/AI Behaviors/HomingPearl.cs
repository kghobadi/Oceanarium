using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPearl : AudioHandler {
   
    //all my private components
    MoveTowards movement;
    Orbit orbital;
    Currents currentScript;
    MeshRenderer pearlMesh;
    GravityBody grav;
    ParticleSystem fog, popLights;
    GrowthSphere growthSphere;

    [Header("Pearl Activation")]
    public bool activated;
    [Tooltip("Put the current transform here if you'd like the pearl to travel to a current when activated")]
    public Transform currentStream;
    [Tooltip("Put the planet manager of the orb's planet")]
    public PlanetManager myPlanet;
    [Tooltip("Silent = Inactive, Active once player touches")]
    public Material silentMat, activeMat;
    [Tooltip("Every object in this array should have a LerpScale script, with Scale at start checked")]
    public GameObject[] objectsToGrow;
    [Tooltip("Speed at which objects will grow")]
    public float growthSpeed = 0.25f;

    [Header("Sounds")]
    public AudioClip activationSound;
    public AudioClip travelingSound;
    public AudioClip orbitingSound;

    public override void Awake()
    {
        base.Awake();
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        pearlMesh = GetComponent<MeshRenderer>();
        pearlMesh.material = silentMat;
        grav = GetComponent<GravityBody>();
        fog = transform.GetChild(0).GetComponent<ParticleSystem>();
        popLights = transform.GetChild(1).GetComponent<ParticleSystem>();
        growthSphere = transform.GetComponentInChildren<GrowthSphere>();
        if (currentStream != null)
        {
            currentScript = currentStream.GetComponentInParent<Currents>();
        }
    }

    void Start()
    {
        StartCoroutine(WaitToDeactivate(0.1f));
    }

    IEnumerator WaitToDeactivate(float time)
    {
        yield return new WaitForSeconds(time);

        DeactivateGrowthObjects();
    }

    void DeactivateGrowthObjects()
    {
        for(int i = 0; i < objectsToGrow.Length; i++)
        {
            objectsToGrow[i].SetActive(false);
        }
    }

    void Update()
    {
        if(currentStream != null)
            TravelToCurrent();
    }

    void TravelToCurrent()
    {
        //activated, traveling to current stream
        if (activated && movement.moving)
        {
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(travelingSound, 1f);
            }
        }

        //activated, but has arrived at currentStream   
        if (activated && movement.moving == false)
        {
            if (orbital.orbiting == false)
            {
                //grav.enabled = false;
                orbital.planetToOrbit = currentStream;
                orbital.orbiting = true;
                currentScript.activePearls.Add(gameObject);
                RandomizePitch(pitchRange.x, pitchRange.y);
            }
        }

        //while orbiting play orbital sound
        if (orbital.orbiting)
        {
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(orbitingSound, 1f);
            }
            //turn off pop lights once current activated 
            if (currentScript.currentActivated && popLights.isPlaying)
            {
                popLights.Stop();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!activated)
            {
                activated = true;
                PlaySound(activationSound, 1f);
                pearlMesh.material = activeMat;
                fog.Stop();
                fog.Clear();
                popLights.Play();

                //i must go to the current!
                if(currentStream != null)
                {
                    movement.MoveTo(currentStream.position, movement.moveSpeed);
                }
                //I must bring my environment to LIFE!
                if(objectsToGrow.Length > 0)
                {
                    EnableGrowObjects();
                }
            }
        }
    }

    void EnableGrowObjects()
    {
        for(int i = 0; i < objectsToGrow.Length; i++)
        {
            //activate 
            objectsToGrow[i].SetActive(true);
            //add to planet man 
            if(!myPlanet.props.Contains(objectsToGrow[i]))
                myPlanet.props.Add(objectsToGrow[i]);
        }

        growthSphere.GrowObjects(growthSpeed);
    }
    
}
