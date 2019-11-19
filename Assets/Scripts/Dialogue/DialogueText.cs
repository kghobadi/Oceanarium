using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DialogueText : MonoBehaviour
{
    //player refs
    GameObject player;
    ThirdPersonController tpc;

    //text component and string array of its lines
    Text theText;
    [Header("Main Bools & Text Lines")]
    //check this to start at start
    public bool enableAtStart;
    public bool lockPlayer;
    public string[] textLines;
    //current and last lines
    public int currentLine;
    public int endAtLine;

    //typing vars
    private bool isTyping = false;
    private bool cancelTyping = false;
    [Header("Typing Speeds, Wait Times")]
    public float typeSpeed;
    public float resetDistance = 25f;
    //check this to have wait time from trigger to enable dialogue
    public bool waitToStart;
    public float timeUntilStart;
    //wait between lines
    public float waitTime;
    //check this to allow an array of wait Times (one for every line of dialogue on this text obj)
    public bool conversational;
    public float[] waitTimes;
    
    //audio stuff
    [Header("Voice Audio Settings")]
    public bool hasVoiceAudio;
    public bool usesAllLetters;
    public int speakFreq = 4;
    public AudioSource[] myVoices;
    public int currentVoice;
    public AudioClip[] myGibberishSounds;

    //for spoken alphabet
    [Header("Letter lists for Voice Audio")]
    public List<char> letters = new List<char>();
    public List<char> capitalLetters = new List<char>();
    public List<AudioClip> spokenSounds = new List<AudioClip>();

    void Start()
    {
        //grab refs
        player = GameObject.FindGameObjectWithTag("Player");
        tpc = player.GetComponent<ThirdPersonController>();
        theText = GetComponent<Text>();

        ResetStringText();
       

        if (!enableAtStart)
        {
            theText.enabled = false;
        }
        else
        {
            EnableDialogue();
        }
    }

    //reset trigger if you swim away during dialogue
    void Update()
    {
        if (!lockPlayer)
        {
            if (Vector3.Distance(transform.position, player.transform.position) > resetDistance)
            {
                StopAllCoroutines();
                DisableDialogue();
                if(GetComponent<Trigger>())
                    GetComponent<Trigger>().hasActivated = false;
                currentLine = 0;
            }
        }
    }

    void ProgressLine()
    {
        currentLine += 1;

        if (currentLine >= endAtLine)
        {
            DisableDialogue();
        }

        else
        {
            StartCoroutine(TextScroll(textLines[currentLine]));
        }
    }

    //Coroutine that types out each letter individually
    private IEnumerator TextScroll(string lineOfText) 
    {
        int letter = 0;
        theText.text = "";
        isTyping = true;
        cancelTyping = false;
        while (isTyping && !cancelTyping && (letter < lineOfText.Length - 1))
        {
            theText.text += lineOfText[letter];
            if (hasVoiceAudio)
            {
                if(letter % speakFreq == 0)
                    Speak(lineOfText[letter]);
            }
            
            letter += 1;
            yield return new WaitForSeconds(typeSpeed);
        }
        theText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;

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

    public void EnableDialogue()
    {
        if (waitToStart)
        {
            StartCoroutine(WaitToStart());
        }

        else
        {
            theText.enabled = true;

            StartCoroutine(TextScroll(textLines[currentLine]));

            if (lockPlayer)
            {
                tpc.canMove = false;
            }
        }
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(timeUntilStart);

        theText.enabled = true;

        StartCoroutine(TextScroll(textLines[currentLine]));

        if (lockPlayer)
        {
            tpc.canMove = false;
        }
    }


    public void DisableDialogue()
    {
        theText.enabled = false;
        if (lockPlayer)
        {
            tpc.canMove = true;
        }

        //if this is gateway and we stopped dialogue, reset it
        if (GetComponent<Trigger>())
        {
            if (GetComponent<Trigger>().isEssenzGateway)
            {
                GetComponent<Trigger>().hasActivated = false;
                currentLine = 0;
            }
        }
    }

    //check through our alphabet of sounds and play corresponding character
    public void Speak(char letter)
    {
        //cycle through audioSources for voice
        if(currentVoice < myVoices.Length - 1)
        {
            currentVoice++;
        }
        else
        {
            currentVoice = 0;
        }
        if (usesAllLetters)
        {
            //check in letters
            if (letters.Contains(letter))
            {
                int index = letters.IndexOf(letter);
                myVoices[currentVoice].clip = spokenSounds[index];
                myVoices[currentVoice].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke");
            }
            //check in capital letters
            else if (capitalLetters.Contains(letter))
            {
                int index = capitalLetters.IndexOf(letter);
                myVoices[currentVoice].clip = spokenSounds[index];
                myVoices[currentVoice].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke capital");
            }
            //punctuation or other stuff?
            else
            {
                PlaySound();
                //Debug.Log("gibberish");
            }
        }
        //for characters who only use gibberish sounds
        else
        {
            PlaySound();
            //Debug.Log("gibberish");
        }

    }

    //to play a sound
    public void PlaySound()
    {
        int randomSound = UnityEngine.Random.Range(0, myGibberishSounds.Length);
        myVoices[currentVoice].clip = myGibberishSounds[randomSound];
        myVoices[currentVoice].PlayOneShot(myGibberishSounds[randomSound]);
    }
}

