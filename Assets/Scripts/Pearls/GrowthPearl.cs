using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthPearl : AudioHandler {
   
    //all my private components
    MoveTowards movement;
    MeshRenderer pearlMesh;
    GravityBody grav;
    ParticleSystem fog, popLights;
    GrowthSphere growthSphere;
    
    [Header("Pearl Activation")]
    public bool activated;
    [Tooltip("Put the planet manager of the orb's planet")]
    public PlanetManager myPlanet;
    [Tooltip("Silent = Inactive, Active once player touches")]
    public Material silentMat, activeMat;
    [Tooltip("Every object in this array should have a LerpScale script, with Scale at start checked")]
    public GameObject[] objectsToGrow;
    [Tooltip("Speed at which objects will grow")]
    public float growthSpeed = 0.25f;
    [Tooltip("Planters to activate")]
    public Planter[] planters;
 
    [Header("Sounds")]
    public AudioClip lureSound;
    public AudioClip activationSound;
    public AudioClip travelingSound;
    public AudioClip orbitingSound;

    public override void Awake()
    {
        base.Awake();
        movement = GetComponent<MoveTowards>();
        pearlMesh = GetComponent<MeshRenderer>();
        pearlMesh.material = silentMat;
        grav = GetComponent<GravityBody>();
        fog = transform.GetChild(0).GetComponent<ParticleSystem>();
        popLights = transform.GetChild(1).GetComponent<ParticleSystem>();
        growthSphere = transform.GetComponentInChildren<GrowthSphere>();
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
        //lure player 
        if (!activated)
        {
            //play moving sound
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(lureSound, 1f);
            }
        }

        //turn off once growth sphere is done 
        if(activated && growthSphere.growing == false)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!activated)
            {
                ActivateGrowthPearl(true);
            }
        }
    }

    public void ActivateGrowthPearl(bool sound)
    {
        //stop prev audio
        if (myAudioSource.isPlaying)
            myAudioSource.Stop();
        //sound? && null check?
        if (sound && activationSound)
            PlaySound(activationSound, 1f);
        //set mat
        pearlMesh.material = activeMat;
        //set particles
        fog.Stop();
        fog.Clear();
        popLights.Play();

        //I must bring my environment to LIFE!
        if (objectsToGrow.Length > 0)
        {
            EnableGrowObjects();
        }

        activated = true;
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
