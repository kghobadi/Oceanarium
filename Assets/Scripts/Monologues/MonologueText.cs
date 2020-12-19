using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Cameras;
using Cinemachine;
using InControl;
using UnityEngine.Events;
public class MonologueText : MonoBehaviour
{
    //player refs
    GameObject player;
    PlayerController pc;
    CameraController playerCam;
    CameraManager camManager;
    [Tooltip("Character or creature who speaks this monologue")]
    public GameObject hostObj;
    [Tooltip("Check this to start at start")]
    public bool enableAtStart;
    [Tooltip("Check this to lock your player's movement")]
    public bool lockPlayer;
    [Tooltip("Automatically sets player to this spot if you set one")]
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
  
    //Audio
    SpeakerSound speakerAudio;
    //animator
    SpeakerAnimations speakerAnimator;

    //text component and string array of its lines
    Text theText;
    TMP_Text the_Text;
    bool usesTMP;
    [Header("Text lines")]
    [Tooltip("No need to fill this in, that will happen automatically")]
    public string[] textLines;

    //current and last lines
    public int currentLine;
    public int endAtLine;
    public bool hasFinished;
    [Tooltip("Check this if you want the Monologue to disappear after the first time its seen")]
    public bool disablesAtFinish;
    public bool loadsScene;
    public bool canSkip = true;
    public bool enablesCinematic;
    public TimelinePlaybackManager cinematic;
    LoadSceneAsync sceneLoader;

    //typing vars
    public bool inMonologue;
    private bool isTyping = false;
    IEnumerator currentTypingLine;
    IEnumerator waitForNextLine;

    [Header("Text Timing")]
    public float timeBetweenLetters;
    public float resetDistance = 25f;
    //check this to have wait time from trigger to enable Monologue
    public bool waitToStart;
    public float timeUntilStart;
    //wait between lines
    public float timeBetweenLines;
    [Tooltip("Check this and fill in array below so that each line of text can be assigned a different wait")]
    public bool conversational;
    public float[] waitTimes;
    bool waiting;

    [Header("Monologues To Enable")]
    public MonologueText[] monologuesToEnable;
    MonologueTrigger mTrigger;

    [Header("Events")]
    public UnityEvent started;
    public UnityEvent ended;

    void Awake()
    {
        //grab refs
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            pc = player.GetComponent<PlayerController>();
            playerCam = Camera.main.GetComponent<CameraController>();
        }
       
        camManager = FindObjectOfType<CameraManager>();
        theText = GetComponent<Text>();

        if(theText == null)
        {
            usesTMP = true;
            the_Text = GetComponent<TMP_Text>();
        }
        speakerAnimator = hostObj.GetComponentInChildren<SpeakerAnimations>();
        speakerAudio = hostObj.GetComponent<SpeakerSound>();
        mTrigger = GetComponent<MonologueTrigger>();
        sceneLoader = FindObjectOfType<LoadSceneAsync>();
    }

    void Start()
    {
        ResetStringText();
       
        if (!enableAtStart)
        {
            if (usesTMP)
                the_Text.enabled = false;
            else
                theText.enabled = false;
        }
        else
        {
            EnableMonologue();
        }
    }

    //reset trigger if you swim away during Monologue
    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //speaker is typing out message
        if (isTyping)
        {
            //player skips to the end of the line
            if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed ) && canSkip)
            {
                if (currentTypingLine != null)
                {
                    StopCoroutine(currentTypingLine);
                }

                //set to full line
                if(isTyping)
                    CompleteTextLine(textLines[currentLine]);

                SetWaitForNextLine();
            }

        }
        //player is waiting for next message
        if (waiting)
        {
            //player skips to next line
            if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) && canSkip)
            {
                //stop wait coroutine 
                if (waitForNextLine != null)
                {
                    StopCoroutine(waitForNextLine);
                }

                ProgressLine();
            }
        }
        
    }

    void ProgressLine()
    {
        currentLine += 1;
        waiting = false;

        //reached the  end, reset
        if (currentLine >= endAtLine)
        {
            hasFinished = true;
            ResetMonologue();
        }
        //set next typing line 
        else
        {
            SetTypingLine();
        }
    }

    //calls text scroll coroutine 
    void SetTypingLine()
    {
        if (currentTypingLine != null)
        {
            StopCoroutine(currentTypingLine);
        }
        currentTypingLine = TextScroll(textLines[currentLine]);

        StartCoroutine(currentTypingLine);
    }

    //Coroutine that types out each letter individually
    private IEnumerator TextScroll(string lineOfText) 
    {
        // set first letter
        int letter = 0;
        if (usesTMP)
            the_Text.text = "";
        else
            theText.text = "";

        isTyping = true;
        //set talking anim
        if(speakerAnimator.talkingAnimations > 0)
            speakerAnimator.RandomTalkingAnim();

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //add this letter to our text
            if (usesTMP)
                the_Text.text += lineOfText[letter];
            else
                theText.text += lineOfText[letter];
            
            //check what audio to play 
            speakerAudio.AudioCheck(lineOfText, letter);
            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if(isTyping)
            CompleteTextLine(lineOfText);

        SetWaitForNextLine();
    }

    //completes current line of text
    void CompleteTextLine(string lineOfText)
    {
        if (usesTMP)
            the_Text.text = lineOfText;
        else
            theText.text = lineOfText;
        isTyping = false;
    }

    //calls wait for next line coroutine 
    void SetWaitForNextLine()
    {
        //start waiting coroutine 
        if (waitForNextLine != null)
        {
            StopCoroutine(waitForNextLine);
        }

        //check what the wait time for this line should be 
        if (conversational)
        {
            waitForNextLine = WaitToProgressLine(waitTimes[currentLine]);
        }
        else
        {
            waitForNextLine = WaitToProgressLine(timeBetweenLines);
        }

        StartCoroutine(waitForNextLine);
    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine(float time)
    {
        yield return new WaitForEndOfFrame();

        waiting = true;

        yield return new WaitForSeconds(time);

        ProgressLine();
    }

    public void ResetStringText()
    {
        if (usesTMP)
            textLines = (the_Text.text.Split('\n'));
        else
            textLines = (theText.text.Split('\n'));
        
        endAtLine = textLines.Length;
    }

    public void EnableMonologue()
    {
        if (waitToStart)
        {
            if (usesTMP)
                the_Text.enabled = false;
            else
                theText.enabled = false;

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
        yield return new WaitForSeconds(timeUntilStart);

        StartMonologue();
    }

    //actually starts
    void StartMonologue()
    {
        started.Invoke();

        if (usesTMP)
            the_Text.enabled = true;
        else
            theText.enabled = true;

        //enable speaker cam, disable cam controller
        if (speakerCam)
        {
            camManager.Set(speakerCam);
            if (playerCam)
                playerCam.enabled = false;
        }
        //use player camera, just move it around 
        else
        {
            //turn off cam movement
            playerCam.canMoveCam = false;
            //move cam to the right 
            playerCam.transform.Translate(new Vector3(cameraXPos, 0, 0), Space.Self);
            //look at specific obj
            if (lookAtObj != null)
                playerCam.transform.LookAt(lookAtObj.transform.position + new Vector3(0, cameraYLook, 0), pc.gravityBody.GetUp());
            //look at speaker
            else
                playerCam.transform.LookAt(hostObj.transform.position + new Vector3(0, cameraYLook, 0), pc.gravityBody.GetUp());
        }

        //set player pos
        if (playerSpot)
        {
            pc.transform.position = playerSpot.position;
        }

        //lock player movement
        if (lockPlayer)
        {
            pc.canMove = false;
            //NO VELOCIT
            pc.playerRigidbody.velocity = Vector3.zero;
        }

        //set player to idle anim
        if(pc)
            pc.animator.SetAnimator("idle");
        inMonologue = true;
        StartCoroutine(TextScroll(textLines[currentLine]));
    }
    
    public void ResetMonologue()
    {
        DisableMonologue();

        if (loadsScene)
        {
            sceneLoader.Transition(3f);
        }

        if (!disablesAtFinish)
        {
            currentLine = 0;
            hasFinished = false;
        }
    }

    public void DisableMonologue()
    {
        StopAllCoroutines();

        ended.Invoke();

        if (usesTMP)
            the_Text.enabled = false;
        else
            theText.enabled = false;
        
        speakerAnimator.SetAnimator("idle");
        inMonologue = false;
        //disable speaker cam, enable cam controller
        if (enablesCinematic)
        {
            cinematic.PlayTimeline();
        }

        //reenable player cam
        if (speakerCam && playerCam)
        {
            camManager.Disable(speakerCam);
            playerCam.enabled = true;
        }
        
        //unlock player
        if (lockPlayer)
        {
            pc.canMove = true;
            //enable cam 
            playerCam.canMoveCam = true;
        }
    }
}

