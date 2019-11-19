using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public List<GameObject> allInactiveObjects = new List<GameObject>();
    public float activationDistance = 75f;
    GameObject _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update () {
        //loop through all objects and check distances from player
		for(int i = 0; i < allInactiveObjects.Count; i++)
        {
            if(allInactiveObjects[i] != null)
            {
                float distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, _player.transform.position);

                if (distanceFromPlayer < activationDistance)
                {
                    //set object active
                    allInactiveObjects[i].SetActive(true);
                    //remove from list
                    allInactiveObjects.Remove(allInactiveObjects[i]);
                    //move i back once to account for change in list index
                    i--;
                }
            }
            //obj is destroyed
            else
            {
                //remove from list
                allInactiveObjects.Remove(allInactiveObjects[i]);
                //move i back once to account for change in list index
                i--;
            }
            
        }
	}
}
