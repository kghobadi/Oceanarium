using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthPearl : AudioHandler {

    //all my private components=
    MoveTowards movement;
    MeshRenderer pearlMesh;
    GravityBody grav;
    ParticleSystem lure, popLights;
    Vector3 origLureScale;
    GrowthSphere growthSphere;
    SaveSystem saveSystem;
    //the pearl was activated in a previous game
    bool previouslyActivated;

    [Header("Pearl Activation")]
    public bool activated;
    public bool playerActivates;
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
        saveSystem = FindObjectOfType<SaveSystem>();
        saveSystem.returningGame.AddListener(NewGame);
        saveSystem.startNewGame.AddListener(NewGame);
        pearlMesh = GetComponent<MeshRenderer>();
        pearlMesh.material = silentMat;
        if (myPlanet == null)
            myPlanet = GetComponentInParent<PlanetManager>();
        grav = GetComponent<GravityBody>();
        lure = transform.GetChild(0).GetComponent<ParticleSystem>();
        origLureScale = lure.transform.localScale;
        popLights = transform.GetChild(1).GetComponent<ParticleSystem>();
        growthSphere = transform.GetComponentInChildren<GrowthSphere>();
    }

    void NewGame()
    {
        StartCoroutine(WaitToDeactivate(0.1f));
    }

    void LoadGame()
    {
        //get saved planet name 
        string savedPlanet = PlayerPrefs.GetString("ActivePlanet");
        //get pref
        if (PlayerPrefs.GetString(myPlanet.planetName + " Pearl " + gameObject.name) == "Activated")
        {
            previouslyActivated = true;

            StartCoroutine(WaitToActivate(0.1f));
        }
        //deactivate those plants!
        else
        {
            StartCoroutine(WaitToDeactivate(0.1f));
        }
    }

    IEnumerator WaitToActivate(float wait)
    {
        yield return new WaitForSeconds(wait);

        ActivateGrowthPearl(false);
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
        if (playerActivates)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!activated)
                {
                    ActivateGrowthPearl(true);
                }
            }
        }
    }

    //this can be called by Pearl meditation finishing :-)
    public void ActivateGrowthPearl(bool sound)
    {
        //already activated
        if (activated)
            return;

        //stop prev audio
        if (myAudioSource.isPlaying)
            myAudioSource.Stop();
        //sound? && null check?
        if (sound && activationSound)
            PlaySound(activationSound, 1f);
        //set mat
        pearlMesh.material = activeMat;
        //set particles
        lure.Stop();
        lure.Clear();
        popLights.Play();

        //loading
        if (previouslyActivated)
        {
            //check for objs
            if(objectsToGrow.Length > 0)
            {
                GrowAllObjects();
            }
        }
        //new
        else
        {
            //I must bring my environment to LIFE!
            if (objectsToGrow.Length > 0)
            {
                EnableGrowObjects();
            }
        }
        
        //set active
        activated = true;
        //set pref
        PlayerPrefs.SetString(myPlanet.planetName + " Pearl " + gameObject.name, "Activated");
    }

    //enables grow objects & starts growth sphere effect
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

        //start effect
        growthSphere.GrowObjects(growthSpeed);
    }

    //immediately grow all objects without producing sphere
    void GrowAllObjects()
    {
        for (int i = 0; i < objectsToGrow.Length; i++)
        {
            //activate 
            objectsToGrow[i].SetActive(true);

            //set lerp scale
            LerpScale lerp = objectsToGrow[i].GetComponent<LerpScale>();
            //if prop has a Lerp scale component for growing 
            if (lerp)
            {
                //set growth
                if (lerp.setScaleAtStart)
                {
                    lerp.SetScaler(growthSpeed, lerp.origScale);
                    lerp.setScaleAtStart = false;
                }
            }

            //get anim
            Animator animator = objectsToGrow[i].GetComponent<Animator>();
            //trigger grow anim
            if (animator)
            {
                //check for param
                if(HasParameter("grow", animator))
                    animator.SetTrigger("grow");
            }

            //add to planet man 
            if (!myPlanet.props.Contains(objectsToGrow[i]))
                myPlanet.props.Add(objectsToGrow[i]);
        }
    }


    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    //for attracting player to pearl during meditation
    public void SetMeditationLure()
    {
        if (activated == false)
        {
            lure.transform.localScale = origLureScale * 10f;
        }
    }

    //after meditation reset lure scale
    public void ResetLure()
    {
        lure.transform.localScale = origLureScale;
    }
}
