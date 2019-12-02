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
    CreatureSpawner creatureSpawner;
    public List<GameObject> spriteCreatures = new List<GameObject>();
    public List<GameObject> props = new List<GameObject>();
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
            playerHere = true;
        }
        else
        {
            DeactivatePlanet();
        }
    }

    public void ActivatePlanet()
    {
        //set player current planet 
        playerHere = true;
        pc.activePlanet = this;
        pc.activePlanetName = planetName;
        gBehavior.currentPoint = 0;
        gBehavior.guardianLocations = guardianLocations;

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
