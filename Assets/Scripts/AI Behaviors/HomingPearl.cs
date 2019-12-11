﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPearl : AudioHandler {
    [Header("Pearl Activation")]
    public bool activated;
    public Transform currentStream;
    public PlanetManager myPlanet;
    MoveTowards movement;
    Orbit orbital;
    Currents currentScript;
    MeshRenderer pearlMesh;
    GravityBody grav;
    public Material silentMat, activeMat;
    ParticleSystem fog, popLights;

    public GameObject[] objectsToGrow;

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
                    GrowObjects();
                }
              
            }
        }
    }

    void GrowObjects()
    {
        for(int i = 0; i < objectsToGrow.Length; i++)
        {
            //activate 
            objectsToGrow[i].SetActive(true);
            //scale up obj for growth
            LerpScale scaler = objectsToGrow[i].GetComponent<LerpScale>();
            scaler.SetScaler(0.25f, scaler.origScale);
            //add to planet man 
            if(!myPlanet.props.Contains(objectsToGrow[i]))
                myPlanet.props.Add(objectsToGrow[i]);
        }
    }
}
