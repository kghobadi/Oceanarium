using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
    GameObject _player;
    PlayerController pc;

    public List<GameObject> allInactiveObjects = new List<GameObject>();
    public float activationDistance = 75f;
   
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        pc = _player.GetComponent<PlayerController>();
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
            if (allInactiveObjects[i] != null)
            {
                float distanceFromPlayer = Vector3.Distance(allInactiveObjects[i].transform.position, _player.transform.position);

                if (distanceFromPlayer < activationDistance)
                {
                    //set object active
                    allInactiveObjects[i].SetActive(true);

                    //check if it has a sprite renderer 
                    SpriteRenderer sprite = allInactiveObjects[i].GetComponent<SpriteRenderer>();
                    if(sprite = null)
                    {
                        //check is sprite render is vis
                        if (sprite.isVisible)
                        {
                            FadeSprite fade = allInactiveObjects[i].GetComponent<FadeSprite>();
                            //fade in if has fade sprite comp
                            if (fade != null)
                                fade.FadeIn();
                        }
                    }
                    
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
