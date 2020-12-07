using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    GameObject _player;
    PlayerController pc;
    Camera mainCam;

    public List<GameObject> allInactiveObjects = new List<GameObject>();
    public float activationDistance = 75f;
    float distanceFromPlayer;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        pc = _player.GetComponent<PlayerController>();
        mainCam = Camera.main;
    }

    void Update ()
    {
        //if player is moving 
        if (pc.playerRigidbody.velocity.magnitude > 0)
        {
            WasteManagement();
        }
	}

    void WasteManagement()
    {
        //loop through all objects and check distances from player
        for (int i = 0; i < allInactiveObjects.Count; i++)
        {
            //does the object exist?
            if (allInactiveObjects[i] != null)
            {
                //dist calc -- normal
                if(pc.moveState != PlayerController.MoveStates.MEDITATING)
                    distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, _player.transform.position);
                //dist -- meditating
                else
                    distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, mainCam.transform.position);

                //is it close enough to activate?
                if (distanceFromPlayer < activationDistance)
                {
                    //activate it 
                    ActivateObject(allInactiveObjects[i]);

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

    public void ActivateObject(GameObject objToActivate)
    {
        //set object active
        objToActivate.SetActive(true);

        //get DeactivateObj
        DeactivateObject deactivate = objToActivate.GetComponent<DeactivateObject>();

        //fade in if has fade sprite comp
        if (deactivate.fader != null)
            deactivate.fader.FadeIn();

        //remove from list
        allInactiveObjects.Remove(objToActivate);
    }
}
