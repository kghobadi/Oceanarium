using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public GameObject squidPrefab;
    public bool spawnAtStart;

    Orbit[] squidOrbits;

	void Start ()
    {
        if(spawnAtStart)
            SpawnSquids();
	}
	
	public void SpawnSquids()
    {
        GameObject squidClone = Instantiate(squidPrefab, transform);

        squidClone.transform.localPosition = Vector3.zero;

        squidClone.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        //set squid orbits 
        squidOrbits = new Orbit[squidClone.transform.childCount];

        for(int i = 0; i < squidOrbits.Length; i++)
        {
            squidOrbits[i] = squidClone.transform.GetChild(i).GetComponent<Orbit>();

            squidOrbits[i].planetToOrbit = transform;

            squidOrbits[i].SetOrbit(squidOrbits[i].orbitalSpeed);
        }
    }
}
