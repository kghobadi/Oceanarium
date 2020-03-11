using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class MonologueTrigger : MonoBehaviour
{
    //player
    GameObject player;
    PlayerController pc;

    //general
    public bool hasActivated;
    public bool playerInZone;
    public bool displayUI;

    public GameObject interactDisplay;
    //monologues
    public MonologueText[] myMonologues;
    [Tooltip("Time after monologue is finished until the trigger resets")]
    public float resetTime = 5f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerEnteredZone();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
            if((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) && !hasActivated)
            {
                WaitToStart(0f);
            }
        }
    }

    //activates monologues
    void ActivateMonologue()
    {
        if (!hasActivated)
        {
            for (int i = 0; i < myMonologues.Length; i++)
            {
                myMonologues[i].EnableMonologue();
            }

            hasActivated = true;
            ToggleInteractUI(false);
        }
    }

    public void PlayerEnteredZone()
    {
        playerInZone = true;
        pc.canJump = false;
        ToggleInteractUI(playerInZone);

        StopAllCoroutines();
    }

    public void PlayerExitedZone()
    {
        playerInZone = false;
        pc.canJump = true;
        ToggleInteractUI(playerInZone);

        hasActivated = false;
    }


    void ToggleInteractUI(bool newState)
    {
        if (displayUI)
        {
            interactDisplay.SetActive(newState);
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

    //called when monologue is started 
    public void WaitToStart(float time)
    {
        StartCoroutine(WaitToActivate(time));
    }

    IEnumerator WaitToActivate(float timer)
    {
        yield return new WaitForEndOfFrame();

        ActivateMonologue();
    }
}
