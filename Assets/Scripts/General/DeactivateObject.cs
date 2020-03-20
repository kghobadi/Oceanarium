using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour
{
    GameObject _player;
    PlayerController pc;
    WorldManager wm;
    SpriteRenderer sRenderer;
    FadeSprite fader;

    public bool waitToCheck;
    public float individualDistOffset = 10f;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        pc = _player.GetComponent<PlayerController>();
        wm = FindObjectOfType<WorldManager>();
        sRenderer = GetComponent<SpriteRenderer>();

        SetFader();
    }

    void SetFader()
    {
        fader = GetComponent<FadeSprite>();
        if (fader == null)
        {
            fader = gameObject.AddComponent<FadeSprite>();
        }

        fader.worldManage = true;
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
        //check to see if its greater than wm dist
        if (Vector3.Distance(_player.transform.position, transform.position) > (wm.activationDistance + individualDistOffset))
        {
            //null check on sprite renderer
            if(sRenderer != null)
            {
                //check if sprite is visible 
                if (sRenderer.isVisible)
                {
                    //fade out already!
                    if (!fader.fadingOut)
                        fader.FadeOut();
                }
                //just deactivate
                else
                {
                    Deactivate();
                }
            }
            //just deactivate
            else
            {
                Deactivate();
            }
        }
    }

    public void Deactivate()
    {
        //first add to list
        wm.allInactiveObjects.Add(gameObject);
        //then deactivate 
        gameObject.SetActive(false);
    }
}