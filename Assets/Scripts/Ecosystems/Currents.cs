using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currents : MonoBehaviour {
    //player ref
    GameObject player;
    PlayerController pc;
    GravityBody gravBody;
    Transform mainCam;
    CameraController camControl;
    ParticleSystem currentParticles;
    public GameObject currentCamera;

    //current vars
    [Header("Current Vars")]
    public float currentSpeed;
    [HideInInspector]
    public Transform entrance, endPoint;
    public bool entering, hasEntered;

    [Header("Pearl Activation")]
    public bool currentActivated;
    [Tooltip("Check if player can activate directly")]
    public bool playerActivates;
    ParticleSystem ribbons;
    public int pearlsNecessary;
    public List<GameObject> activePearls = new List<GameObject>();
    public MeshRenderer currentBubble;
    public Material silentMat, activeMat;

    [HideInInspector]
    public AudioSource ambientSource;
    [Header("Sounds")]
    public AudioClip[] currentAmbience;

    void Awake()
    {
        ambientSource = GameObject.FindGameObjectWithTag("AmbientAudio").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        gravBody = player.GetComponent<GravityBody>();
        mainCam = Camera.main.transform;
        camControl = mainCam.GetComponent<CameraController>();
        currentParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
        currentParticles.Stop();
        currentBubble.material = silentMat;
        if (playerActivates)
        {
            ribbons = transform.GetChild(3).GetComponent<ParticleSystem>();
        }
    }

    void Start()
    {
        entrance = transform.GetChild(0);
        endPoint = transform.GetChild(1);
        if (playerActivates)
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
            if (!playerActivates)
            {
                if (activePearls.Count >= pearlsNecessary)
                {
                    ActivateCurrent();
                }
            }
        }
        
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
            if (playerActivates)
            {
                if (other.gameObject.tag == "Player")
                {
                    ActivateCurrent();
                }
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (currentActivated)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!ambientSource.isPlaying)
                    PlaySound(currentAmbience);

                if (!hasEntered)
                {
                    EnterCurrent();
                }
            }
        }
      
    }

    void ActivateCurrent()
    {
        currentParticles.Play();
        currentBubble.material = activeMat;
        currentActivated = true;
        if(playerActivates)
            ribbons.Stop();
    }

    void EnterCurrent()
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

        //reset velocity
        pc.playerRigidbody.velocity = Vector3.zero;
        gravBody.enabled = false;
        pc.canMove = false;
        pc.animator.SetAnimator("inCurrent");
        //deactivate p cam & activate currentCam
        currentCamera.SetActive(true);
        camControl.canMoveCam = false;
    }

    //need a delay before stopping current
    IEnumerator WaitToStopCurrent(GameObject other)
    {
        yield return new WaitForSeconds(0.25f);

        ambientSource.Stop();
        //activate p cam & deactivate currentCam
        currentCamera.SetActive(false);
        camControl.canMoveCam = true;
        camControl.LerpFOV(camControl.originalFOV, 2f);
        pc.canMove = true;
        gravBody.enabled = true;
        pc.animator.SetAnimator("idle");
    }

    //called to play sounds 
    public void PlaySound(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);
        ambientSource.clip = soundArray[randomSound];

        ambientSource.Play();
    }

   
}
