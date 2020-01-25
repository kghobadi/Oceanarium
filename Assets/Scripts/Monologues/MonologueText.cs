using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Cameras;
using Cinemachine;

public class MonologueText : MonoBehaviour
{
    //player refs
    GameObject player;
    PlayerController pc;
    CameraController playerCam;
    CameraManager camManager;
    [Tooltip("Character or creature who speaks this monologue")]
    public GameObject hostObj;
    [Tooltip("Camera to use for Monologue")]
    public GameCamera speakerCam;
    [Tooltip("Check this to start at start")]
    public bool enableAtStart;
    [Tooltip("Check this to lock your player's movement")]
    public bool lockPlayer;
    [Tooltip("Automatically sets player to this spot")]
    public Transform playerSpot;
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
        //speaker is typing out message
        if (isTyping)
        {
            //check for player
            if (player != null)
            {
                //reeset if too far from Monologue
                if (Vector3.Distance(transform.position, player.transform.position) > resetDistance)
                {
                    ResetMonologue();
                }
            }

            //player skips to the end of the line
            if (Input.GetKeyDown(KeyCode.Space) && canSkip)
            {
                if (currentTypingLine != null)
                {
                    StopCoroutine(currentTypingLine);
                }

                //set to full line
                CompleteTextLine(textLines[currentLine]);

                StartCoroutine(WaitToProgressLine());
            }

        }
        //player is waiting for next message
        if (waiting)
        {
            //player skips to next line
            if (Input.GetKeyDown(KeyCode.Space) && canSkip)
            {
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
        else
        {
            //this debug helps find the wait times for the current line of Monologue
            //Debug.Log(hostObj.name + " is on line " + currentLine + " which reads: " + textLines[currentLine] + " -- " + hostObj.name + " will wait " + waitTimes[currentLine].ToString() + "sec before speaking again!");
            if(currentTypingLine != null)
            {
                StopCoroutine(currentTypingLine);
            }
            currentTypingLine = TextScroll(textLines[currentLine]);

            StartCoroutine(currentTypingLine);

        }
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

        waiting = true;

        //if conversational, use the array of wait Timers set publicly
        if (conversational)
        {
            yield return new WaitForSeconds(waitTimes[currentLine]);
        }
        else
        {
            yield return new WaitForSeconds(timeBetweenLines);
        }
        

        ProgressLine();

    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine()
    {
        yield return new WaitForEndOfFrame();

        waiting = true;

        //if conversational, use the array of wait Timers set publicly
        if (conversational)
        {
            yield return new WaitForSeconds(waitTimes[currentLine]);
        }
        else
        {
            yield return new WaitForSeconds(timeBetweenLines);
        }
        
        ProgressLine();
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
        if (usesTMP)
            the_Text.enabled = true;
        else
            theText.enabled = true;

        //enable speaker cam, disable cam controller
        camManager.Set(speakerCam);
        if(playerCam)
            playerCam.enabled = false;
        //lock player movement
        if (lockPlayer)
        {
            pc.canMove = false;
            //NO VELOCIT
            pc.playerRigidbody.velocity = Vector3.zero;
        }

        //set player pos
        if (playerSpot)
        {
            pc.transform.position = playerSpot.position;
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
            mTrigger.WaitToReset(3f);
            currentLine = 0;
        }
    }

    public void DisableMonologue()
    {
        StopAllCoroutines();

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
        if (playerCam)
        {
            camManager.Disable(speakerCam);
            playerCam.enabled = true;
        }
           
        //unlock player
        if (lockPlayer)
        {
            pc.canMove = true;
        }
    }
}

