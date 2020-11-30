using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MonologueTrigger : MonoBehaviour
{
    //player
    GameObject player;
    Camera cameraMan;
    PlayerController pc;
    CameraController cc;

    //general
    [Tooltip("Indicates whether the Monologue has been activated by Player or not")]
    public bool hasActivated;
    [Tooltip("Player will immediately activate Monologue when they enter the Trigger")]
    public bool autoActivates;
    [Tooltip("Player will set this true when entering trigger, false when exiting")]
    public bool playerInZone;
    [Tooltip("Check this as true to show interactDisplay UI")]
    public bool displayUI;
    [Tooltip("Check if this is the guardian's trigger")]
    public bool guardian;

    [Tooltip("This UI object will generally be located under the Monologue Canvas")]
    public GameObject interactDisplay;
    //monologues
    [Tooltip("Drag and Drop any Monologue Managers that should be activated by this Trigger")]
    public MonologueManager[] myMonologues;
    [Tooltip("Indeces corresponding to monologues inside each manager listed")]
    public int[] monologueIndeces;
    [Tooltip("Time after monologue is finished until the trigger resets")]
    public float resetTime = 5f;

    [Header("Spirit")]
    [Tooltip("Called by spirit script")]
    public bool spiritEnabled;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        cameraMan = Camera.main;
        cc = cameraMan.GetComponent<CameraController>();

        //when player meditates, auto leave physical trigger 
        pc.startMeditation.AddListener(PlayerExitedZone);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !spiritEnabled)
        {
            PlayerEnteredZone();
        }

        if (other.gameObject.tag == "MainCamera" && spiritEnabled)
        {
            PlayerEnteredZone();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && spiritEnabled == false)
        {
            if (playerInZone)
            {
                if (displayUI)
                {
                    if (interactDisplay.activeSelf == false && hasActivated == false)
                    {
                        ToggleInteractUI(true);
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && spiritEnabled == false)
        {
            PlayerExitedZone();
        }
        if (other.gameObject.tag == "MainCamera" && spiritEnabled)
        {
            PlayerExitedZone();
        }
    }

    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (playerInZone)
        {
            if((Input.GetKeyDown(KeyCode.E) || inputDevice.Action3.WasPressed || autoActivates) && !hasActivated)
            {
                WaitToStart(0f);
            }
        }
    }

    //activates monologues
    void ActivateMonologue()
    {
        if (!hasActivated && pc.canMove)
        {
            for (int i = 0; i < myMonologues.Length; i++)
            {
                myMonologues[i].mTrigger = this;

                //set monologue system 
                //if(myMonologues[i].worldSpaceCanvas == false)
                //{
                //    myMonologues[i].SetMonologueSystem(monologueIndeces[i]);
                //}

                myMonologues[i].EnableMonologue();
            }

            hasActivated = true;
            ToggleInteractUI(false);
        }
    }

    //player inside trigger
    public void PlayerEnteredZone()
    {
        playerInZone = true;

        if(!hasActivated)
            ToggleInteractUI(playerInZone);

        StopAllCoroutines();
    }

    //player left trigger
    public void PlayerExitedZone()
    {
        playerInZone = false;

        ToggleInteractUI(playerInZone);
    }

   

    //turns on and off interact UI
    void ToggleInteractUI(bool newState)
    {
        if (displayUI)
        {
            interactDisplay.SetActive(newState);
        }
        else
        {
            interactDisplay.SetActive(false);
        }
    }

    //called when monologue text script is reset
    public void WaitToReset(float time)
    {
        StartCoroutine(WaitToReactivate(time));
    }

    IEnumerator WaitToReactivate(float timer)
    {
        yield return new WaitForSeconds(timer);

        hasActivated = false;
    }

    //immediate reset 
    public void Reset()
    {
        hasActivated = false;
    }

    //called when monologue is started 
    public void WaitToStart(float time)
    {
        StartCoroutine(WaitToActivate(time));
    }

    IEnumerator WaitToActivate(float timer)
    {
        yield return new WaitForSeconds(timer);

        yield return new WaitForEndOfFrame();

        ActivateMonologue();
    }
}
