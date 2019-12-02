using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPearl : AudioHandler {
    [Header("Pearl Activation")]
    public bool activated;
    public Transform currentStream;
    MoveTowards movement;
    Orbit orbital;
    Currents currentScript;
    MeshRenderer pearlMesh;
    GravityBody grav;
    public Material silentMat, activeMat;
    ParticleSystem fog, popLights;

    [Header("Sounds")]
    public AudioClip activationSound;
    public AudioClip travelingSound;
    public AudioClip orbitingSound;

    public override void Awake()
    {
        base.Awake();
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        currentScript = currentStream.GetComponentInParent<Currents>();
        pearlMesh = GetComponent<MeshRenderer>();
        pearlMesh.material = silentMat;
        grav = GetComponent<GravityBody>();
        fog = transform.GetChild(0).GetComponent<ParticleSystem>();
        popLights = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        //activated, traveling to current stream
        if(activated && movement.moving)
        {
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(travelingSound, 1f);
            }
        }

        //activated, but has arrived at currentStream   
        if(activated && movement.moving == false)
        {
            if(orbital.orbiting == false)
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
            if(myAudioSource.isPlaying == false)
            {
                PlaySound(orbitingSound, 1f);
            }
            //turn off pop lights once current activated 
            if(currentScript.currentActivated && popLights.isPlaying)
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
                movement.MoveTo(currentStream.position, movement.moveSpeed);
                activated = true;
                PlaySound(activationSound, 1f);
                pearlMesh.material = activeMat;
                fog.Stop();
                fog.Clear();
                popLights.Play();
            }
        }
    }
}
