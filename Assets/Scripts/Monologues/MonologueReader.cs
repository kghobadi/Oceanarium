using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InControl;
using UnityEngine.Events;

//this script is responsible for the active reading out of a monologue 
public class MonologueReader : MonoBehaviour {
    [HideInInspector]
    public GameObject hostObj;  // parent NPC obj set by MonologueManager
    [HideInInspector]
    public MonologueManager monoManager; //my mono manager
    [HideInInspector]
    public SpeakerSound speakerAudio; // speaker sound 

    [HideInInspector] public Text theText;
    [HideInInspector] public TMP_Text the_Text;
    [HideInInspector] public bool usesTMP;
    [Tooltip("No need to fill this in, that will happen automatically")]
    public string[] textLines;
    //current and last lines
    public int currentLine;
    public int endAtLine;
    public bool canSkip = true;
    //typing vars
    private bool isTyping = false;
    IEnumerator currentTypingLine;
    IEnumerator waitForNextLine;

    [Header("Text Timing")]
    public float timeBetweenLetters;
    //wait between lines
    public float timeBetweenLines;
    [Tooltip("Check this and fill in array below so that each line of text can be assigned a different wait")]
    public bool conversational;
    public bool autoProgress;
    public float[] waitTimes;
    bool waiting;
    public UnityEvent waitingForPlayerInput;
    public FadeUI pressToSkip;
    
    [Tooltip("For 'dialogue' options at the end of Monologue")]
    public bool displayChoices;

    void Awake()
    {
        theText = GetComponent<Text>();

        //press to skip ref
        //if (pressToSkip == null && monoManager.worldSpaceCanvas == false)
        //{
        //    pressToSkip = GameObject.FindGameObjectWithTag("Skip").GetComponent<FadeUI>();
        //}

        if (theText == null)
        {
            usesTMP = true;
            the_Text = GetComponent<TMP_Text>();
        }
    }

    void Start()
    {
        if(hostObj)
            speakerAudio = hostObj.GetComponent<SpeakerSound>();

        if (usesTMP)
            the_Text.enabled = false;
        else
            theText.enabled = false;
    }

    void Update ()
    {
        if(canSkip)
            LineSkipping();
    }

    void LineSkipping()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //speaker is typing out message
        if (isTyping)
        {
            //player skips to the end of the line
            if ((Input.GetKeyDown(KeyCode.E) || inputDevice.Action3.WasPressed) 
                && canSkip)
            {
                if (currentTypingLine != null)
                {
                    StopCoroutine(currentTypingLine);
                }

                //set to full line
                if (isTyping)
                    CompleteTextLine(textLines[currentLine]);

                SetWaitForNextLine();
            }
        }

        //player is waiting for next message
        if (waiting)
        {
            //player skips to next line
            if ((Input.GetKeyDown(KeyCode.E) || inputDevice.Action3.WasPressed) 
                && canSkip)
            {
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

        if(pressToSkip)
        {
            pressToSkip.FadeOut();
        }

        //reached the  end, reset
        if (currentLine >= endAtLine)
        {
            if (displayChoices)
            {
                monoManager.dialogueChoices.SetActive(true);

                //enable cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                monoManager.DisableMonologue();
            }
        }
        //set next typing line 
        else
        {
            SetTypingLine();
        }
    }

    //calls text scroll coroutine 
    public void SetTypingLine()
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
        if (monoManager.looperAI)
        {
            //set ai talking anim
            //if (monoManager.npcController.Animation)
            //{
            //    //set talking anim if not already talking 
            //    if (monoManager.npcController.Animation.characterAnimator.GetBool("talking") == false)
            //        monoManager.npcController.Animation.SetAnimator("talking");
            //}
        }

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //add this letter to our text
            if (usesTMP)
                the_Text.text += lineOfText[letter];
            else
                theText.text += lineOfText[letter];

            //check what audio to play 
            if(speakerAudio)
                speakerAudio.AudioCheck(lineOfText, letter);
            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if (isTyping)
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
        else if(autoProgress)
        {
            waitForNextLine = WaitToProgressLine(timeBetweenLines);
        }
        //almost all dialogues wait for user input 
        else if (!autoProgress)
        {
            waitForNextLine = WaitToProgressLine();
        }

        StartCoroutine(waitForNextLine);
    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine()
    {
        yield return new WaitForEndOfFrame();

        waiting = true;
        waitingForPlayerInput.Invoke();

        if (pressToSkip)
        {
            pressToSkip.FadeIn();
        }
    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine(float time)
    {
        yield return new WaitForEndOfFrame();

        waiting = true;

        yield return new WaitForSeconds(time);

        ProgressLine();
    }
}
