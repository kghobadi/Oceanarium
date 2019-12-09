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

    public FadeUI tripFader;
    public FadeUI pressToTrip;

    [Tooltip("Player must be this close to start trip")]
    public float necessaryDistance = 15f;

    public GameObject tripCamera;
    public GameObject trip;
    public bool tripping;

    MusicFader mFader;
    public AudioClip tripMusic;
    AudioClip planetMusic;
    public AudioMixerSnapshot trippingSnap;
    public AudioMixerSnapshot overWorld;
    ParticleSystem tripperParticles;

    void Awake () {
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        gravBod = player.GetComponent<GravityBody>();
        mainCam = Camera.main.transform.gameObject;
        camControl = mainCam.GetComponent<CameraController>();
        //my refs
        mFader = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicFader>();
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
            if (Input.GetKeyDown(KeyCode.Space) && !tripping && !tripFader.fadingIn && !tripFader.fadingOut)
            {
                StartTrip();
            }

            //fade in press to trip 
            if(pressToTrip)
                pressToTrip.FadeIn();

            //play trip particles
            if(tripperParticles.isPlaying == false)
            {
                tripperParticles.Play();
            }
        }
        //too far
        else if(distFromPlayer > necessaryDistance + 3f)
        {
            //fade out press to trip
            if (pressToTrip)
                pressToTrip.FadeOut();

            //stop trip particles
            if (tripperParticles.isPlaying)
            {
                tripperParticles.Stop();
            }
        }

        //press space && not tripping // converting 
        if (Input.GetKeyDown(KeyCode.Space) && tripping && !tripFader.fadingIn && !tripFader.fadingOut)
        {
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
        camControl.LerpFOV(15f, 0.2f);
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

        //deactivate player stuff
        pc.canMove = false;
        pc.canJump = false;
        mainCam.SetActive(false);

        //fade out black 
        tripFader.FadeOut();

        tripping = true;
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
        camControl.LerpFOV(camControl.originalFOV, 0.05f);

        //deactivate trip stuff 
        tripCamera.SetActive(false);
        trip.SetActive(false);
        
        //fade out black 
        tripFader.FadeOut();

        yield return new WaitUntil(() => tripFader.fadingOut == false);
        //no longer tripping once fade out is finished 
        tripping = false;
        camControl.canMoveCam = true;

        Debug.Log("Ended trip");
    }
}
