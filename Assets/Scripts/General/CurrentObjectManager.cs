using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//placed on a BoxTrigger object halfway thru current which turns stuff on and off
public class CurrentObjectManager : MonoBehaviour {
    GameObject player;
    PlayerController pc;

    //activation check
    [Header("Planet Activation")]
    public bool hasActivated;
    float timeToReactivate;
    public PlanetManager prevPlanet, nextPlanet;
    
    [Header("Changes Player Scale")]
    public bool lerpPlayerScale, lerping;
    public Vector3 desiredScale;

    [Header("Changes Jump Force")]
    public bool setJumpForce;
    public float newJumpForce;

    [Header("Changes Music")]
    public bool newTrack;
    AudioSource musicSource;
    bool fadingVolOut, fadingVolIn;
    
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        musicSource = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        timeToReactivate = 1;
	}
	
	void Update () {
        if (hasActivated)
        {
            timeToReactivate -= Time.deltaTime;
            if(timeToReactivate < 0)
            {
                hasActivated = false;
                timeToReactivate = 1;
            }
        }

        if (lerping)
        {
            player.transform.localScale = Vector3.Lerp(player.transform.localScale, desiredScale, Time.deltaTime);

            if(Vector3.Distance(player.transform.localScale, desiredScale) < 0.1f)
            {
                lerping = false;
            }
        }

        if (fadingVolOut)
        {
            musicSource.volume -= Time.deltaTime;

            if(musicSource.volume <= 0)
            {
                musicSource.Stop();
                musicSource.clip = nextPlanet.musicTrack;
                musicSource.Play();

                fadingVolOut = false;
                fadingVolIn = true;
            }
        }

        if (fadingVolIn)
        {
            musicSource.volume += Time.deltaTime;

            if(musicSource.volume >= 1)
            {
                fadingVolIn = false;
            }
        }
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasActivated)
        {
            //turn off old planet, activate new one 
            prevPlanet.DeactivatePlanet();
            nextPlanet.ActivatePlanet();

            if (lerpPlayerScale)
            {
                lerping = true;
            }

            if (setJumpForce)
            {
                pc.jumpForce = newJumpForce;
            }

            if (newTrack)
            {
                fadingVolOut = true;
            }
        }
    }
}
