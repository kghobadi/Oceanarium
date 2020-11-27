using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currents : AudioHandler {
    //player ref
    GameObject player;
    PlayerController pc;
    GravityBody gravBody;
    Transform mainCam;
    CameraController camControl;
    ParticleSystem currentParticles;

    [Tooltip("Normally referenced in code automatically")]
    public PlanetManager planetMan;
    
    //current vars
    [Header("Current Vars")]
    public float currentSpeed;
    [HideInInspector]
    public Transform entrance, endPoint;
    public bool entering, hasEntered;
    [Tooltip("Only needs a ref if this is a 2 way")]
    public CurrentObjectManager cObjectMan;

    [Header("Current Activation")]
    public bool currentActivated;
    [Tooltip("Type of thing which activates this Current")]
    public ActivationType activationType;
    public enum ActivationType
    {
        PLAYER, PLANTERPEARLS, RUINS,
    }
    ParticleSystem ribbons;
    public ParticleSystem explosion;
    public int pearlsNecessary;
    public GameObject[] currentPillars;
    public List<GameObject> activePearls = new List<GameObject>();
    public MeshRenderer currentBubble;
    public Material silentMat, activeMat;
    [Tooltip("Only needs this if current activated by MoveToCurrent Pearls")]
    public TimelinePlaybackManager cinematicManager;

    [Tooltip("Check this to permanently turn off the previous planet")]
    public bool disablesPrevPlanet;

    [HideInInspector]
    public AudioSource ambientSource;
    [Header("Sounds")]
    public AudioClip[] currentAmbience;
    public AudioClip activationSound;
    
    public override void Awake()
    {
        base.Awake();
        ambientSource = GameObject.FindGameObjectWithTag("AmbientAudio").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        gravBody = player.GetComponent<GravityBody>();
        mainCam = Camera.main.transform;
        camControl = mainCam.GetComponent<CameraController>();
        currentParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        currentParticles.Stop();
        currentBubble.material = silentMat;
        ribbons = transform.GetChild(3).GetComponent<ParticleSystem>();

        if(planetMan == null)
            planetMan = GetComponentInParent<PlanetManager>();

        //only fetch if it is not set publicly
        if(cObjectMan == null)
            cObjectMan = GetComponentInChildren<CurrentObjectManager>();    
    }

    void Start()
    {
        entrance = transform.GetChild(0);
        endPoint = transform.GetChild(1);
        for (int i = 0; i < currentPillars.Length; i++)
        {
            currentPillars[i].GetComponent<MeshRenderer>().material = silentMat;
        }
        if (activationType == ActivationType.PLAYER)
        {
            ribbons.Play();
        }
    }

    void Update()
    {
        if (currentActivated)
        {
            //only runs when player tries to enter current. 
            //pulls you to public transform entrance
            if (entering)
            {
                pc.playerRigidbody.velocity = Vector3.zero;
                player.transform.position = Vector3.MoveTowards(player.transform.position, entrance.position, currentSpeed * Time.deltaTime);

                if (Vector3.Distance(player.transform.position, entrance.position) < 0.5f)
                {
                    entering = false;
                    pc.playerRigidbody.velocity = Vector3.zero;
                    hasEntered = true;
                }
            }

            //only if hasEntered has completed
            if (hasEntered)
            {
                pc.transform.position = Vector3.MoveTowards(pc.transform.position, endPoint.position, currentSpeed * Time.deltaTime);

                if (Vector3.Distance(pc.transform.position, endPoint.position) < 5f)
                {
                    hasEntered = false;
                    StartCoroutine(WaitToStopCurrent(player));
                }
            }
        }
        //checks for pearls to activate current 
        else
        {
            if (activationType == ActivationType.PLANTERPEARLS)
            {
                if (activePearls.Count >= pearlsNecessary)
                {
                    ActivateCurrent();
                }
            }
        }
        
    }

    //called by MoveToCurrentPearl
    public void ActivatePillar(GameObject pillar, GameObject pearl)
    {
        pillar.GetComponent<MeshRenderer>().material = activeMat;
        activePearls.Add(pearl);
        Debug.Log("activated a pillar");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (currentActivated)
        {
            if (other.gameObject.tag == "Player")
            {
                EnterCurrent();
            }
        }
        else
        {
            if (activationType == ActivationType.PLAYER)
            {
                if (other.gameObject.tag == "Player")
                {
                    ActivateCurrent();
                }
            }
        }
    }

    //stay check to pull you in once activated 
    public void OnTriggerStay(Collider other)
    {
        if (currentActivated)
        {
            if (other.gameObject.tag == "Player" && !entering && !hasEntered)
            {
                EnterCurrent();
            }
        }
    }

    public void ActivateCurrent()
    {
        Debug.Log("activated current");

        currentParticles.Play();
        currentBubble.material = activeMat;
        currentActivated = true;

        //null check
        if (planetMan)
        {
            if (planetMan.pSelector)
                planetMan.pSelector.UnlockPlanet();
        }
      
        //turn off the ribbons
        if (ribbons)
        {
            if (ribbons.isPlaying)
                ribbons.Stop();
        }
        //play explosion
        if (explosion)
        {
            explosion.Play();
        }

        //activated by MoveToCurrentPearls 
        if(activationType == ActivationType.PLANTERPEARLS)
        {
            //play sound + cinematic
            if(cinematicManager)
                cinematicManager.PlayTimeline();
            PlaySound(activationSound, myAudioSource.volume);
        }
        //also play this sound 
        else if (activationType == ActivationType.PLAYER)
        {
            PlaySound(activationSound, myAudioSource.volume);
        }
    }

    public void EnterCurrent()
    {
        //we are entering the current near the entrance 
        if (Vector3.Distance(player.transform.position, entrance.position) < 25)
        {
            //reset player pos to entrance 
            entering = true;
        }
        //we have entered elsewhere
        else
        {
            hasEntered = true;
        }

        //for 2 way cObj -- set returning if we are on 'next planet' 
        if(cObjectMan.twoWay && cObjectMan.nextPlanet == pc.activePlanet)
        {
            cObjectMan.returning = true;
        }

        //reset velocity
        pc.playerRigidbody.velocity = Vector3.zero;
        gravBody.enabled = false;
        pc.canMove = false;
        pc.animator.SetAnimator("inCurrent");
        //deactivate p cam & activate currentCam
        camControl.LerpFOV(100f, 1f);
    }

    //need a delay before stopping current
    IEnumerator WaitToStopCurrent(GameObject other)
    {
        yield return new WaitForSeconds(0.25f);

        ambientSource.Stop();
        //activate p cam & deactivate currentCam
        camControl.LerpFOV(camControl.originalFOV, 2f);
        pc.canMove = true;
        gravBody.enabled = true;
        pc.animator.SetAnimator("idle");

        //disable previous planet completely
        if (disablesPrevPlanet)
        {
            if(cObjectMan)
                cObjectMan.prevPlanet.gameObject.SetActive(false);
        }
    }
    
    //called to play sounds 
    public void PlaySound(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);
        ambientSource.clip = soundArray[randomSound];

        ambientSource.Play();
    }
}
