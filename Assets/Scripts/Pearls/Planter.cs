using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planter : AudioHandler {
    MeshRenderer myMR;
    CapsuleCollider capColl;
    MoveTowards movement;
    GravityBody grav;
    ParticleSystem lure;
    ParticleSystem tentacleTrail;
    ParticleSystem popLights;

    [Header("Planter Travel")]
    [Tooltip("Determinations destination / activation when it arrives at final point")]
    public PlanterType planterType;
    public enum PlanterType
    {
        CURRENT, RUINS, FREEROAM,
    }
    [Tooltip("Check this to make me invisible on start")]
    public bool invisableOnStart;
    [Tooltip("Planet to add plants to")]
    public PlanetManager planetManager;
    [Tooltip("True once player activates my Homing Pearl")]
    public bool activated;
    [Tooltip("Materials before & after activation")]
    public Material silentMat, activeMat;
    [Tooltip("Dests I will travel to before stopping.")]
    public Transform[] travelPoints;
    int travelPoint = 0;
    bool finalPoint;
    [Tooltip("Current script if I am going to Current")]
    public Currents currents;
    [Tooltip("Ruins script if i am going to Ruins")]
    public Ruins ruin;

    [Header("Plant Spawning")]
    [Tooltip("Plant/prop to spawn")]
    public GameObject spawnObj;
    [Tooltip("Transform to parent spawned objects to ")]
    public Transform plantParent;
    [Tooltip("Controls how often the planter will spawn")]
    public float spawnTimer, spawnFrequency = 0.5f;
    [Tooltip("Size multipliers for spawned objs")]
    public float minSizeMult, maxSizeMult;
   

    [Tooltip("Only raycasts at Planet layer")]
    public LayerMask planetMask;

    [Header("Sounds")]
    public AudioClip lureSound;
    public AudioClip activationSound;
    public AudioClip travelingSound;
    public AudioClip orbitingSound;

    public override void Awake ()
    {
        base.Awake();

        //get all refs
        myMR = GetComponent<MeshRenderer>();
        myMR.material = silentMat;
        //turn invis
        if (invisableOnStart)
        {
            myMR.enabled = false;
        }
        capColl = GetComponent<CapsuleCollider>();
        movement = GetComponent<MoveTowards>();
        grav = GetComponent<GravityBody>();

        lure = transform.GetChild(0).GetComponent<ParticleSystem>();
        tentacleTrail = transform.GetChild(1).GetComponent<ParticleSystem>();
        popLights = transform.GetChild(3).GetComponent<ParticleSystem>();
      
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

            MoveCheck();
        }
        //not activated, luring 
        else
        {
            //play moving sound
            if (myAudioSource.isPlaying == false)
            {
                PlaySound(lureSound, 1f);
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
                ActivatePlanter(true);
            }
        }
    }

    //called by Pearl activation
    public void ActivatePlanter(bool sound)
    {
        //stop prev audio
        if (myAudioSource.isPlaying)
            myAudioSource.Stop();
        //sound?
        if (sound && activationSound != null)
            PlaySoundRandomPitch(activationSound, 1f);
        //set MR
        myMR.material = activeMat;

        //if not free roaming, draw attention with active mat & popLights
        if(!invisableOnStart)
        {
            myMR.enabled = true;
            popLights.Play();
            tentacleTrail.Play();
        }
           
        //change particles
        lure.Stop();
        lure.Clear();
        
        //move 
        SetMove();
        activated = true;
    }

    //called in Update to move
    void MoveCheck()
    {
        //reached dest
        if (movement.moving == false)
        {
            //final point reached, so deactivate
            if (finalPoint)
            {
                DeactivatePlanter();
            }
            //set next point
            else
            {
                SetMove();
            }
        }
    }

    void SetMove()
    {
        //set moveTowards component
        movement.MoveTo(travelPoints[travelPoint].position, movement.moveSpeed);
        //inc point up
        if (travelPoint < travelPoints.Length - 1)
            travelPoint++;
        //set final point (auto does this if only 1 point)
        if (travelPoint == travelPoints.Length - 1)
            finalPoint = true;
    }

    //called when it reaches dest
    void DeactivatePlanter()
    {
        //activat Current's pillar with planter && position me right on toppp
        if(planterType == PlanterType.CURRENT)
        {
            capColl.enabled = false;
            grav.enabled = false;
            transform.position = travelPoints[travelPoint].GetChild(0).position;
            currents.ActivatePillar(travelPoints[travelPoint].gameObject, gameObject);
        }
        //activate ruins, deactivate
        else if (planterType == PlanterType.RUINS)
        {
            ruin.ActivatePillar(travelPoints[travelPoint].gameObject);
            gameObject.SetActive(false);
        }

        //deactivate entirely
        else if (planterType == PlanterType.FREEROAM)
        {
            gameObject.SetActive(false);
        }

        activated = false;
    }

    //called every time spawn freq timer below 0 
    void RaycastToPlanet()
    {
        RaycastHit hit;

        Vector3 castPos = Random.insideUnitSphere * 3 + transform.position;

        if (Physics.Raycast(castPos, -grav.GetUp(), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.tag == "Planet" || hit.transform.gameObject.layer == 9)
            {
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
        //set parent
        plantClone.transform.SetParent(plantParent);
      
        //if it has scaler use that to lerp it up!!
        if(plantClone.GetComponent<LerpScale>())
        {
            plantClone.GetComponent<LerpScale>().origScale = transform.localScale;

            plantClone.transform.localScale *= plantClone.GetComponent<LerpScale>().startMultiplier;

            plantClone.GetComponent<LerpScale>().WaitToSetScale(0.1f, plantClone.GetComponent<LerpScale>().lerpSpeed, plantClone.GetComponent<LerpScale>().origScale);
        }

        //set repulsion direction using planter's gravity
        if (plantClone.GetComponent<Repulsor>())
        {
            plantClone.GetComponent<Repulsor>().direction = grav.GetUp();
        }

        //if it has Orbit, tell it what to orbit!
        if (plantClone.GetComponent<Orbit>())
        {
            plantClone.GetComponent<Orbit>().planetToOrbit = planetManager.transform;
        }

        //trigger growth!
        plantClone.GetComponent<Animator>().SetTrigger("grow");

        //reset spawn timer
        spawnTimer = spawnFrequency;
        //add to planet manager list 
        planetManager.props.Add(plantClone);
    }
}
