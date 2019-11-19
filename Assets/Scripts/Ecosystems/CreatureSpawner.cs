using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use this to spawn creatures for a specific ecosystem
public class CreatureSpawner : MonoBehaviour {
    PlanetManager planetMan;

    public GameObject[] creaturesToSpawn;
    public int desiredAmount;
    //list to monitor ecosystem activity
    public List<GameObject> activeCreatures = new List<GameObject>();
    public Transform[] spawnPoints, fleePositions;
   
    void Awake()
    {
        planetMan = GetComponent<PlanetManager>();

        for (int i = 0; i < desiredAmount; i++)
        {
            SpawnCreature();
        }
    }

    void Start () {
		
	}
	
	void Update () {
        //when active creatures drops below desired amount, spawn a new one
		if(activeCreatures.Count < desiredAmount)
        {
            SpawnCreature();
        }
	}

    //spawns a new creature...
    void SpawnCreature()
    {
        //randomize spawn type and location
        int randomCreature = Random.Range(0, creaturesToSpawn.Length);
        int randomSpawnPoint = Random.Range(0, spawnPoints.Length);

        Vector3 spawnPos = spawnPoints[randomSpawnPoint].position + Random.insideUnitSphere * 5;

        //spawn and add to activeCreatures list, 
        GameObject creatureClone = Instantiate(creaturesToSpawn[randomCreature], spawnPos, Quaternion.identity);
        activeCreatures.Add(creatureClone);
        planetMan.spriteCreatures.Add(creatureClone);

        //randomize scale a little
        float randomScaleMult = Random.Range(0.5f, 1);
        creatureClone.transform.localScale *= randomScaleMult;

        //give info to creature
        creatureClone.GetComponent<EdibleCreature>().mySpawner = this;
        creatureClone.GetComponent<Orbit>().planetToOrbit = transform;
        creatureClone.GetComponent<Orbit>().randomAxis = true;

        //Debug.Log("spawned a" + creatureClone.name);
    }
}
