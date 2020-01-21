using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : AudioHandler {
    MeshRenderer myMR;
    CapsuleCollider capColl;
    MoveTowards movement;
    Orbit orbital;
    GravityBody grav;
    ParticleSystem fog;
    ParticleSystem popLights;

    [Header("Plant spawning")]
    [Tooltip("Planet to add plants to")]
    public PlanetManager planetManager;
    [Tooltip("True once player activates my Homing Pearl")]
    public bool activated;
    [Tooltip("Dest I will travel to before stopping.")]
    public Transform pillarDest;
    public Ruins ruin;
    [Tooltip("Plant/prop to spawn")]
    public GameObject spawnObj;
    [Tooltip("Transform to parent spawned objects to ")]
    public Transform plantParent;
    public float spawnTimer, spawnFrequency = 0.5f;
    [Tooltip("Size multipliers for spawned objs")]
    public float minSizeMult, maxSizeMult;
    [Tooltip("Materials before & after activation")]
    public Material silentMat, activeMat;

    [Tooltip("Only raycasts at Planet layer")]
    public LayerMask planetMask;

    [Header("Sounds")]
    public AudioClip travelingSound;
    public AudioClip orbitingSound;
    public AudioClip activationSound;

    public override void Awake ()
    {
        base.Awake();

        //get all refs
        myMR = GetComponent<MeshRenderer>();
        myMR.material = silentMat;
        capColl = GetComponent<CapsuleCollider>();
        movement = GetComponent<MoveTowards>();
        orbital = GetComponent<Orbit>();
        grav = GetComponent<GravityBody>();
        
        popLights = transform.GetChild(0).GetComponent<ParticleSystem>();
        fog = transform.GetChild(1).GetComponent<ParticleSystem>();
    }
	
	void Update ()
    {
        if (activated)
        {
            //play moving sound
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(travelingSound, 1f);
            }

            //always countdown spawn timer
            spawnTimer -= Time.deltaTime;
            //raycast in attempt to spawn
            if(spawnTimer < 0)
            {
                RaycastToPlanet();
            }

            //reached dest
            if(movement.moving == false)
            {
                DeactivatePlanter();
            }
        }
	}

    //trigger to activate!
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!activated)
            {
                activated = true;
                PlaySound(activationSound, 1f);
                myMR.material = activeMat;
                fog.Stop();
                fog.Clear();
                popLights.Play();

                ActivatePlanter();
            }
        }
    }

    //called by Pearl activation
    public void ActivatePlanter()
    {
        if (pillarDest)
        {
            transform.SetParent(null);
            movement.MoveTo(pillarDest.position, movement.moveSpeed);
            myMR.enabled = true;
            activated = true;
        }
    }

    //called when it reaches dest
    void DeactivatePlanter()
    {
        ruin.ActivatePillar(pillarDest.gameObject);
        myMR.enabled = false;
        activated = false;
    }

    //called every time spawn freq timer below 0 
    void RaycastToPlanet()
    {
        RaycastHit hit;

        Vector3 castPos = Random.insideUnitSphere * 3 + transform.position;

        if (Physics.Raycast(castPos, -grav.GetUp(), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Planet")
            {
                //Debug.Log("spawning " + hit.point);
                SpawnPlant(hit.point);
            }
        }
    }

    //called when raycast hits planet surface
    void SpawnPlant(Vector3 pos)
    {
        //spawn and set parent
        GameObject plantClone = Instantiate(spawnObj, pos, Quaternion.identity);
        //set scale
        plantClone.transform.localScale = new Vector3(1, 1, 1);
        //randomize scale
        float sizeMult = Random.Range(minSizeMult, maxSizeMult);
        plantClone.transform.localScale *= sizeMult;

        plantClone.transform.SetParent(plantParent);
      
        //if it has scaler use that to lerp it up!!
        if(plantClone.GetComponent<LerpScale>())
        {
            plantClone.transform.localScale *= 0.01f;

            plantClone.GetComponent<LerpScale>().SetScaler(0.5f, plantClone.GetComponent<LerpScale>().origScale);
        }

        //reset spawn timer
        spawnTimer = spawnFrequency;
        //add to planet manager list 
        planetManager.props.Add(plantClone);
    }
}
