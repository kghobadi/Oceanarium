using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class MonologueText : MonoBehaviour
{
    //player refs
    GameObject player;
    [Tooltip("Character or creature who speaks this monologue")]
    public GameObject hostObj;
    [Tooltip("UI panel which holds this monologue")]
    public FadeUI textPanel; // I often parent all of a character's monologues to a single Panel 
    [Tooltip("Check this to start at start")]
    public bool enableAtStart;
    [Tooltip("Check this to lock your player's movement")]
    public bool lockPlayer;
    //Audio
    SpeakerSound speakerAudio;
    //animator
    SpeakerAnimations speakerAnimator;

    //text component and string array of its lines
    TMP_Text theText;
    [Header("Text lines")]
    [Tooltip("No need to fill this in, that will happen automatically")]
    public string[] textLines;

    //current and last lines
    public int currentLine;
    public int endAtLine;
    public bool hasFinished;

    //typing vars
    private bool isTyping = false;

    [Header("Text Timing")]
    public float timeBetweenLetters;
    public float resetDistance = 25f;
    //check this to have wait time from trigger to enable Monologue
    public bool waitToStart;
    public float timeUntilStart;
    //wait between lines
    public float waitTime;
    [Tooltip("Check this and fill in array below so that each line of text can be assigned a different wait")]
    public bool conversational;
    public float[] waitTimes;

    [Header("Monologues To Enable")]
    public MonologueText[] monologuesToEnable;

    void Awake()
    {
        //grab refs
        player = GameObject.FindGameObjectWithTag("Player");
        theText = GetComponent<TMP_Text>();
        speakerAnimator = hostObj.GetComponentInChildren<SpeakerAnimations>();
        speakerAudio = hostObj.GetComponent<SpeakerSound>();
    }

    void Start()
    {
        ResetStringText();
       
        if (!enableAtStart)
        {
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
        if (isTyping)
        {
            if(player != null)
            {
                //reeset if too far from Monologue
                if (Vector3.Distance(transform.position, player.transform.position) > resetDistance)
                {
                    ResetMonologue();
                }
            }
           
        }
        else
        {

        }
    }

    void ProgressLine()
    {
        currentLine += 1;

        if (currentLine >= endAtLine)
        {
            hasFinished = true;
            DisableMonologue();
        }
        else
        {
            //this debug helps find the wait times for the current line of Monologue
            //Debug.Log(hostObj.name + " is on line " + currentLine + " which reads: " + textLines[currentLine] + " -- " + hostObj.name + " will wait " + waitTimes[currentLine].ToString() + "sec before speaking again!");
            StartCoroutine(TextScroll(textLines[currentLine]));
        }
    }

    //Coroutine that types out each letter individually
    private IEnumerator TextScroll(string lineOfText) 
    {
        int letter = 0;
        theText.text = "";
        isTyping = true;
        speakerAnimator.RandomTalkingAnim();
        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //add this letter to our text
            theText.text += lineOfText[letter];
            //check what audio to play 
            speakerAudio.AudioCheck(lineOfText, letter);
            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }
        theText.text = lineOfText;
        isTyping = false;

        //if conversational, use the array of wait Timers set publicly
        if (conversational)
        {
            yield return new WaitForSeconds(waitTimes[currentLine]);
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        ProgressLine();

    }

    public void ResetStringText()
    {
        textLines = (theText.text.Split('\n'));

        endAtLine = textLines.Length;
    }

    public void EnableMonologue()
    {
        if (waitToStart)
        {
            theText.enabled = false;
            StartCoroutine(WaitToStart());
        }

        else
        {
            theText.enabled = true;
            textPanel.FadeIn();
            StartCoroutine(TextScroll(textLines[currentLine]));
        }
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(timeUntilStart);

        theText.enabled = true;

        StartCoroutine(TextScroll(textLines[currentLine]));
        
    }

    public void ResetMonologue()
    {
        StopAllCoroutines();
        DisableMonologue();
        GetComponent<MonologueTrigger>().hasActivated = false;
        currentLine = 0;
    }

    public void DisableMonologue()
    {
        theText.enabled = false;
        textPanel.FadeOut();
        speakerAnimator.SetAnimator("idle");
        //start new monologues 
        for(int i = 0; i < monologuesToEnable.Length; i++)
        {
            monologuesToEnable[i].EnableMonologue();
        }
    }
}

