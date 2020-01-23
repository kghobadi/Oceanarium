using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    GameObject player;
    PlayerController pc;
    GameObject guardian;
    Guardian gBehavior;

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

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        creatureSpawner = GetComponent<CreatureSpawner>();
        guardian = GameObject.FindGameObjectWithTag("Guardian");
        gBehavior = guardian.GetComponent<Guardian>();
	}

    void Start()
    {
        if (startingPlanet)
        {
            pc.transform.position = playerStartingPoint.position;
            ActivatePlanet(guardianLocations[0].position);
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
        gBehavior.ResetGuardianLocation(guardianPos, guardianLocations, planet);

        for(int i = 0; i < spriteCreatures.Count; i++)
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
