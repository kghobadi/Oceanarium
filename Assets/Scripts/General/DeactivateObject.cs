using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour
{
    GameObject _player;
    PlayerController pc;
    WorldManager wm;

    public bool waitToCheck;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        pc = _player.GetComponent<PlayerController>();
        wm = FindObjectOfType<WorldManager>();
    }

    void Start()
    {
        if(!waitToCheck)
            DistCheck();
    }

    void Update()
    {
        //wm null check
        if(wm != null)
        {
            //if player is moving 
            if (pc.playerRigidbody.velocity.magnitude > 0)
            {
                DistCheck();
            }
        }
    }

    //deactivate object when it's far enough away from player 
    void DistCheck()
    {
        if (Vector3.Distance(_player.transform.position, transform.position) > (wm.activationDistance + 10f))
        {
            //first add to list
            wm.allInactiveObjects.Add(gameObject);
            //then deactivate 
            gameObject.SetActive(false);
        }
    }
}