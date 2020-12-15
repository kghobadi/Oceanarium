using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour
{
    GameObject _player;
    PlayerController pc;
    Camera playerCam;
    WorldManager wm;
    Animator animator;

    //so worldmanager can access these 
    [HideInInspector]
    public SpriteRenderer sRenderer;
    [HideInInspector]
    public FadeSprite fader;
    
    [Tooltip("Added to WorldMan dist")]
    public float individualDistOffset = 10f;
    public float distFromPlayer;
    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        pc = _player.GetComponent<PlayerController>();
        wm = FindObjectOfType<WorldManager>();
        sRenderer = GetComponent<SpriteRenderer>();
        playerCam = Camera.main;
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        //preserve anim state
        if (animator)
        {
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        //assure sprite renderer and fade sprite exist 
        if (sRenderer == null)
        {
            sRenderer = GetComponentInChildren<SpriteRenderer>();

            if (sRenderer)
            {
                fader = sRenderer.GetComponent<FadeSprite>();
                if (fader == null)
                {
                    fader = sRenderer.gameObject.AddComponent<FadeSprite>();
                }
            }
        }
        //sprite renderer found on this transform
        else
        {
            fader = GetComponent<FadeSprite>();
            if (fader == null)
            {
                fader = gameObject.AddComponent<FadeSprite>();
            }
        }

        //set fader
        if (fader)
        {
            fader.wm = this;
            fader.worldManage = true;
        }
    }

    void Update()
    {
        //wm null check
        if(wm != null)
        {
            //if player is moving or meditating 
            if (pc.playerRigidbody.velocity.magnitude > 0 || pc.moveState == PlayerController.MoveStates.MEDITATING)
            {
                DistCheck();
            }
        }
    }

    //deactivate object when it's far enough away from player 
    void DistCheck()
    {
        //normal
        if(pc.moveState != PlayerController.MoveStates.MEDITATING)
            distFromPlayer = Vector3.Distance(_player.transform.position, transform.position);
        //meditating
        else
            distFromPlayer = Vector3.Distance(playerCam.transform.position, transform.position);

        //check to see if its greater than wm dist
        if (distFromPlayer > (wm.activationDistance + individualDistOffset))
        {
            //null check on sprite renderer
            if(fader != null)
            {
                //fade out! -- if not already. Fader will Deactivate once it is transparent
                if (fader.fadeState != FadeSprite.FadeStates.FADINGOUT && 
                    fader.fadeState != FadeSprite.FadeStates.TRANSPARENT)
                {
                    fader.FadeOut();
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
        wm.allInactiveObjects.Add(this);
       
        //then deactivate 
        gameObject.SetActive(false);
    }
}