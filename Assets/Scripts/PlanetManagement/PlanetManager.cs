using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    GameObject player;
    PlayerController pc;
    GameObject guardian;
    Guardian gBehavior;
    MusicFader mFader;

    public string planetName;
    public bool playerHere, startingPlanet;
    [Tooltip("If starting planet checked, player will start at this Transform")]
    public Transform playerStartingPoint;
    [Tooltip("This planet's colliders")]
    public Collider[] planetColliders;
    CreatureSpawner creatureSpawner;
    [Tooltip("Any sort of creature that moves with code")]
    public List<GameObject> spriteCreatures = new List<GameObject>();
    [Tooltip("Any stagnant, animated object on the planet")]
    public List<GameObject> props = new List<GameObject>();
    [Tooltip("Movement points for the Guardian AI")]
    public Transform[] guardianLocations;
    public AudioClip musicTrack;

    [Header("Player Movement Settings")]
    public float newSwimSpeed = 25f;
    public float newMaxSpeed = 15f;
    [Tooltip("Use this if the player needs a new elevation speed (for changing height)")]
    public float newElevationSpeed = 50f;
    public float newMaxDistFromPlanet = 50f;
    public float newJumpForce = 1000f;

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        creatureSpawner = GetComponent<CreatureSpawner>();
        guardian = GameObject.FindGameObjectWithTag("Guardian");
        gBehavior = guardian.GetComponent<Guardian>();
        mFader = FindObjectOfType<MusicFader>();
	}

    void Start()
    {
        if (startingPlanet)
        {
            //teleport player 
            pc.transform.position = playerStartingPoint.position;
            //teleport guardian 
            if(Vector3.Distance(pc.transform.position, gBehavior.transform.position) > 50f)
                gBehavior.TeleportGuardian(playerStartingPoint.position);
            //activate planet 
            ActivatePlanet(guardianLocations[0].position);
            //fade to music
            mFader.FadeTo(musicTrack);
        }
        else
        {
            DeactivatePlanet();
        }
    }

    public void ActivatePlanet(Vector3 guardianPos)
    {
        //set player current planet 
        playerHere = true;
        pc.activePlanet = this;
        pc.activePlanetName = planetName;

        //reset guardian AI for this planet 
        Collider[] planet = GetComponents<Collider>();
        //only set guardian if not first planet
        gBehavior.ResetGuardianLocation(guardianPos, guardianLocations, planet);

        //update player's movement settings 
        pc.elevateSpeed = newElevationSpeed;
        pc.swimSpeed = newSwimSpeed;
        pc.maxSpeed = newMaxSpeed;
        pc.distMaxFromPlanet = newMaxDistFromPlanet;
        pc.jumpForce = newJumpForce;

        for (int i = 0; i < spriteCreatures.Count; i++)
        {
            spriteCreatures[i].SetActive(true);
        }
        for (int i = 0; i < props.Count; i++)
        {
            props[i].SetActive(true);
        }
    }

    public void DeactivatePlanet()
    {
        playerHere = false;
        for (int i = 0; i < spriteCreatures.Count; i++)
        {
            spriteCreatures[i].SetActive(false);
        }
        for (int i = 0; i < props.Count; i++)
        {
            props[i].SetActive(false);
        }
    }

}
