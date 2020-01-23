using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TripActivation : MonoBehaviour {
    GameObject player;
    PlayerController pc;
    GravityBody gravBod;
    GameObject mainCam;
    CameraController camControl;
    LoadSceneAsync sceneLoader;
    Vector3 origPos;

    public FadeUI tripFader;
    public FadeUI pressToTrip;

    [Tooltip("Player must be this close to start trip")]
    public float necessaryDistance = 15f;
    public GuardianAnimation guardAnim;
    public GameObject tripCamera;
    public GameObject trip;
    public bool canTrip = true;
    public bool tripping;
    public bool loadsNewScene;

    MusicFader mFader;
    public AudioClip tripMusic;
    AudioClip planetMusic;
    public AudioMixerSnapshot trippingSnap;
    public AudioMixerSnapshot overWorld;
    public ParticleSystem tripperParticles;
    [Header("Camera Transition")]
    public float fovIn = 15f;
    public float lerpTimeIn = 0.2f, lerpTimeOut = 0.05f;

    void Awake () {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        gravBod = player.GetComponent<GravityBody>();
        mainCam = Camera.main.transform.gameObject;
        camControl = mainCam.GetComponent<CameraController>();
        sceneLoader = GetComponent<LoadSceneAsync>();
        //my refs
        mFader = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicFader>();
        if(tripperParticles == null)
            tripperParticles = transform.GetChild(0).GetComponent<ParticleSystem>();

        if (tripCamera == null)
            tripCamera = GameObject.FindGameObjectWithTag("TripCam");
    }

    void Update()
    {
        //calc dist
        float distFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        //is it within range to trip?
        if (distFromPlayer < necessaryDistance)
        {
            //press space && not tripping // converting to trip 
            if (Input.GetKeyDown(KeyCode.Space) && canTrip && !tripping && !tripFader.fadingIn && !tripFader.fadingOut)
            {
                StartTrip();
            }

            //fade in press to trip 
            if(pressToTrip)
                pressToTrip.FadeIn();

            //play trip particles
            if(tripperParticles != null)
            {
                if (tripperParticles.isPlaying == false)
                {
                    tripperParticles.Play();
                }
            }
            
        }
        //too far
        else if(distFromPlayer > necessaryDistance + 3f)
        {
            //fade out press to trip
            if (pressToTrip)
                pressToTrip.FadeOut();

            //stop trip particles
            if(tripperParticles != null)
            {
                if (tripperParticles.isPlaying)
                {
                    tripperParticles.Stop();
                }
            }
            
        }

        //press space && not tripping // converting 
        if (Input.GetKeyDown(KeyCode.Space) && tripping && !tripFader.fadingIn && !tripFader.fadingOut)
        {
            if (loadsNewScene)
            {
                //everything happens in LoadSceneAsync now...
            }
            else
                EndTrip();
        }
    }

    //fade in black back and begin trip sequence 
    public void StartTrip()
    {
        //sound
        planetMusic = mFader.musicTrack;
        mFader.FadeTo(tripMusic);
        trippingSnap.TransitionTo(2f);
        //camera
        camControl.canMoveCam = false;
        camControl.transform.LookAt(transform, gravBod.GetUp());
        camControl.LerpFOV(fovIn, lerpTimeIn);
        //set fade
        tripFader.FadeIn();
        //activate
        StartCoroutine(ActivateTrip());
    }

    IEnumerator ActivateTrip()
    {
        //once black background fully faded in
        yield return new WaitUntil(() => tripFader.fadingIn == false);

        //activate trip stuff 
        tripCamera.SetActive(true);
        trip.SetActive(true);

        //ride guardian if can
        if (guardAnim)
        {
            guardAnim.SetAnimator("ride");
            origPos = transform.position;
            //parent guardian to camera & move to be visible
            transform.SetParent(tripCamera.transform);
            transform.localPosition += new Vector3(0, 0, 10);
            gameObject.layer = 14;
        }

        //start load 
        if (loadsNewScene)
        {
            //check to see if already preparing
            if(sceneLoader.loadPreparesOnStart == false)
                sceneLoader.Load();

        }

        //deactivate player stuff
        pc.canMove = false;
        pc.canJump = false;
        mainCam.SetActive(false);

        //fade out black 
        tripFader.FadeOut();

        tripping = true;
        canTrip = false;
        Debug.Log("Started trip");
    }

    //ends trip while tripping 
    public void EndTrip()
    {
        //fade
        tripFader.FadeIn();
        //sound
        mFader.FadeTo(planetMusic);
        overWorld.TransitionTo(2f);
        //deactivate
        StartCoroutine(DeactivateTrip());
    }

    IEnumerator DeactivateTrip()
    {
        //once black background fully faded in
        yield return new WaitUntil(() => tripFader.fadingIn == false);

        //activate player stuff
        pc.canMove = true;
        pc.canJump = true;
        mainCam.SetActive(true);
        camControl.LerpFOV(camControl.originalFOV, lerpTimeOut );
        
        //deactivate trip stuff 
        tripCamera.SetActive(false);
        trip.SetActive(false);
        
        //fade out black 
        tripFader.FadeOut();

        yield return new WaitUntil(() => tripFader.fadingOut == false);
        //no longer tripping once fade out is finished 
        tripping = false;
        camControl.canMoveCam = true;

        StartCoroutine(WaitToResetTrip(10f));

        Debug.Log("Ended trip");
    }

    IEnumerator WaitToResetTrip(float time)
    {
        yield return new WaitForSeconds(time);

        canTrip = true;
    }
}
