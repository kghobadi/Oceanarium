using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//placed on a BoxTrigger object halfway thru current which turns stuff on and off
public class CurrentObjectManager : MonoBehaviour {
    GameObject player;
    PlayerController pc;
    LerpScale playerScaler;
    Currents currentParent;

    //activation check
    [Header("Planet Activation")]
    public bool hasActivated;
    float timeToReactivate;
    public PlanetManager prevPlanet, nextPlanet;

    [Header("Changes Player Scale")]
    public bool lerpPlayerScale;
    public Vector3 desiredScale;
    public float scaleSpeed;

    [Header("Changes Jump Force")]
    public bool setJumpForce;

    [Header("Changes Music")]
    public bool newTrack;
    MusicFader mFader;
    
	void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        playerScaler = player.GetComponent<LerpScale>();
        currentParent = GetComponentInParent<Currents>();
        mFader = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicFader>();
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
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasActivated)
        {
            //turn off old planet, activate new one 
            prevPlanet.DeactivatePlanet();

            nextPlanet.ActivatePlanet(currentParent.endPoint.position);

            if (lerpPlayerScale)
            {
                playerScaler.SetScaler(scaleSpeed, desiredScale);
            }

            if (setJumpForce)
            {
                pc.jumpForce = nextPlanet.newJumpForce;
            }

            if (newTrack)
            {
                mFader.FadeTo(nextPlanet.musicTrack);
            }

            //Debug.Log("transitioned to " + nextPlanet.planetName);
        }
    }
}
