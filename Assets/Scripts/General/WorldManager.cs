using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    GameObject _player;
    PlayerController pc;
    Camera mainCam;

    public List<DeactivateObject> allInactiveObjects = new List<DeactivateObject>();
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
        //if player is moving or meditating
        if (pc.playerRigidbody.velocity.magnitude > 0 || pc.moveState == PlayerController.MoveStates.MEDITATING)
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
                //dist -- meditating
                if (pc.moveState == PlayerController.MoveStates.MEDITATING)
                    distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, mainCam.transform.position);
                //dist -- normal
                else
                    distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, _player.transform.position);

                //individual obj activation val
                float activeCheck = activationDistance + allInactiveObjects[i].individualDistOffset - 5f;

                //is it close enough to activate?
                if (distanceFromPlayer < activeCheck)
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

    public void ActivateObject(DeactivateObject deactivateObj)
    {
        //set object active
        deactivateObj.gameObject.SetActive(true);

        //fade in if has fade sprite comp
        if (deactivateObj.fader != null)
            deactivateObj.fader.FadeIn();

        //remove from list
        allInactiveObjects.Remove(deactivateObj);
    }
}
