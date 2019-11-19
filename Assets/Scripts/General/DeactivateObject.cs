using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour
{
    GameObject _player;
    ThirdPersonController tpc;
    WorldManager wm;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        tpc = _player.GetComponent<ThirdPersonController>();
        wm = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<WorldManager>();
    }

    void Update()
    {
        //if player is moving 
        if (tpc.playerRigidbody.velocity.magnitude > 0)
        {
            //Debug.Log(wm);
            //deactivate object when it's far enough away from player 
            if (Vector3.Distance(_player.transform.position, transform.position) > (wm.activationDistance + 10f))
            {
                //first add to list
                wm.allInactiveObjects.Add(gameObject);
                //then deactivate 
                gameObject.SetActive(false);
            }
        }
    }
}