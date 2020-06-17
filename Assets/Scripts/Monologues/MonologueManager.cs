using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Cameras;
using Cinemachine;

public class MonologueManager : MonoBehaviour
{
    //player refs
    PlayerController player;
    CameraController camController; 

    //npc management refs 
    [HideInInspector]
    public WorldMonologueManager wmManager;
    CameraManager camManager;
    CinematicsManager cineManager;
    [HideInInspector]
    public LooperAI looperAI;
    MonologueReader monoReader;
    LoadSceneAsync sceneLoader;

    [Tooltip("if there is a background for speaking text")]
    public FadeUI textBack;
    AnimateUI animateTextback;
    //text component and string array of its lines
    public int currentMonologue;
    [Tooltip("Fill this with all the individual monologues the character will give")]
    public List<Monologue> allMyMonologues = new List<Monologue>();

    [Tooltip("True while monologue system is active & character speaking")]
    public bool inMonologue;
    [HideInInspector]
    public MonologueTrigger mTrigger;

    [Tooltip("Check to Enable monologue at index 0 at start")]
    public bool enableOnStart;
    [Tooltip("If there is a specific spot the player should be teleported to for Monologue")]
    public Transform playerSpot;
    
    [Header("Camera Settings")]
    [Tooltip("Moves camera left (-) or right (+) for player to look at char")]
    public float cameraXPos = 10f;
    [Tooltip("Moves camera look pos up or down. I recommend between 0 & 10")]
    public float cameraYLook = 3f;
    [Tooltip("Character or thing to look at, if this is empty we default to hostObj (the speaker)")]
    public Transform lookAtObj;
    [Tooltip("Can use a cinemachine camera instead, just place it here and it will override other camera settings")]
    public GameCamera speakerCam;

    void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        camController = FindObjectOfType<CameraController>();

        if (textBack)
            animateTextback = textBack.GetComponent<AnimateUI>();

        wmManager = FindObjectOfType<WorldMonologueManager>();
        cineManager = FindObjectOfType<CinematicsManager>();
        camManager = FindObjectOfType<CameraManager>();
        monoReader = GetComponentInChildren<MonologueReader>();
        monoReader.hostObj = gameObject;
        monoReader.monoManager = this;
        sceneLoader = FindObjectOfType<LoadSceneAsync>();
    }

    void Start()
    {
        //set text to first string in my list of monologues 
        if(allMyMonologues.Count > 0)
            SetMonologueSystem(0);

        //play mono 0 
        if (enableOnStart)
        {
            EnableMonologue();
        }
    }

    //sets monologue system to values contained in Monologue[index]
    public void SetMonologueSystem(int index)
    {
        //set current monologue
        currentMonologue = index;

        //set mono reader text lines 
        monoReader.textLines = (allMyMonologues[currentMonologue].monologue.text.Split('\n'));

        //set current to 0 and end to length 
        monoReader.currentLine = 0;
        monoReader.endAtLine = monoReader.textLines.Length;

        //set mono reader text speeds 
        monoReader.timeBetweenLetters = allMyMonologues[currentMonologue].timeBetweenLetters;
        monoReader.timeBetweenLines = allMyMonologues[currentMonologue].timeBetweenLines;
        monoReader.conversational = allMyMonologues[currentMonologue].conversational;
        monoReader.waitTimes = allMyMonologues[currentMonologue].waitTimes;
    }

    //has a wait for built in
    public void EnableMonologue()
    {
        //disable until its time to start 
        if (allMyMonologues[currentMonologue].waitToStart)
        {
            if (monoReader.usesTMP)
                monoReader.the_Text.enabled = false;
            else
                monoReader.theText.enabled = false;

            StartCoroutine(WaitToStart());
        }
        //starts now
        else
        {
            StartMonologue();
        }
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(allMyMonologues[currentMonologue].timeUntilStart);

        StartMonologue();
    }

    //actually starts
    void StartMonologue()
    {
        //enable text comps 
        if (monoReader.usesTMP)
            monoReader.the_Text.enabled = true;
        else
            monoReader.theText.enabled = true;

        //textback
        if (textBack)
        {
            textBack.FadeIn();
            if (animateTextback)
                animateTextback.active = true;
        }

        //enable speaker cam, disable cam controller
        if (speakerCam)
        {
            camManager.Set(speakerCam);
            if (camController)
                camController.enabled = false;
        }
        //use player camera, just move it around 
        else
        {
            //turn off cam movement
            camController.canMoveCam = false;
            //move cam to the right 
            camController.transform.Translate(new Vector3(cameraXPos, 0, 0), Space.Self);
            //look at specific obj
            if (lookAtObj != null)
                camController.transform.LookAt(lookAtObj.transform.position + new Vector3(0, cameraYLook, 0), player.gravityBody.GetUp());
            //look at speaker
            else
                camController.transform.LookAt(transform.position + new Vector3(0, cameraYLook, 0), player.gravityBody.GetUp());
        }

        //set player pos
        if (playerSpot)
        {
            player.transform.position = playerSpot.position;
        }

        //lock player movement
        if (allMyMonologues[currentMonologue].lockPlayer)
        {
            //no move
            player.canMove = false;
            player.canJump = false;
            //zero player vel
            player.playerRigidbody.velocity = Vector3.zero;
            player.playerRigidbody.angularVelocity = Vector3.zero;
        }

        //is this a looperAI?
        if (looperAI)
        {
            //set talking behavior
        }
        
        //begin mono 
        inMonologue = true;

        //start the typing!
        monoReader.SetTypingLine();
    }
    
    public void DisableMonologue()
    {
        StopAllCoroutines();

        //disable text components 
        if (monoReader.usesTMP)
            monoReader.the_Text.enabled = false;
        else
            monoReader.theText.enabled = false;

        //textback
        if (textBack)
        {
            textBack.FadeOut();
            if(animateTextback)
                animateTextback.active = false;
        }

        //is this an npc?
        if (looperAI)
        {
            //set speaker to idle
        }
        
        StartCoroutine(WaitForCameraTransition());
    }

    IEnumerator WaitForCameraTransition()
    {
        yield return new WaitForSeconds(1f);

        Monologue mono = allMyMonologues[currentMonologue];

        //is this an npc?
        if (looperAI)
        {
            //stop that waiting!
        }

        //unlock player
        if (mono.lockPlayer)
        {
            player.canMove = true;
            player.canJump = true;
        }

        //reenable player cam
        if (speakerCam)
        {
            camManager.Disable(speakerCam);
            camController.enabled = true;
        }
        camController.canMoveCam = true;

        //check for cinematic to enable 
        if (mono.playsCinematic)
        {
            cineManager.allCinematics[mono.cinematic.cIndex].cPlaybackManager.StartTimeline();
        }
        //cinematic triggers to enable
        if (mono.enablesCinematicTriggers)
        {
            for (int i = 0; i < mono.cTriggers.Length; i++)
            {
                cineManager.allCinematics[mono.cTriggers[i].cIndex].cTrigger.gameObject.SetActive(true);
            }
        }
        
        //if this monologue repeats at finish
        if (mono.repeatsAtFinish)
        {
            //reset the monologue trigger after 3 sec 
            mTrigger.WaitToReset(5f);
        }
        //disable the monologue trigger, it's done 
        else
        {
            mTrigger.gameObject.SetActive(false);
        }

        //if this monologue has a new monologue to activate
        if (mono.triggersMonologues)
        {
            //enable the monologues but wait to make them usable to player 
            for(int i = 0; i< mono.monologueIndeces.Length; i++)
            {
                MonologueTrigger mTrigger = wmManager.allMonologues[mono.monologueIndeces[i]].mTrigger;
                mTrigger.gameObject.SetActive(true);
                mTrigger.hasActivated = true;
                mTrigger.WaitToReset(mono.monologueWaits[i]);
            }
        }

        //loads next scene!
        if (mono.loadsScene)
        {
            sceneLoader.Transition(3f);
        }

        inMonologue = false;
    }
}

