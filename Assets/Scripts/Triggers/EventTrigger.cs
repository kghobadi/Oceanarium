using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

    public bool canTrigger = true;
    public bool hasTriggered;
    public bool playerOnly;
    public bool npcOnly;
    public bool pearlOnly;
    
    public UnityEvent[] events;
    public GameObject specificObj;

    [Header("Wait times")]
    public bool waits;
    public float waitTime = 5f;

    [Header("If trigger can activate repeatedly")]
    public bool repeats;
    public float resetTime = 5f;
    
    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && canTrigger)
        {
            if (playerOnly)
            {
                if (other.gameObject.tag == "Player")
                {
                    SetTrigger();
                }
            }
            else if (pearlOnly)
            {
                if (other.gameObject.tag == "Pearl")
                {
                    SetTrigger();
                }
            }
            else if (npcOnly)
            {
                if (other.gameObject.tag == "NPC")
                {
                    SetTrigger();
                }
            }
            else if (specificObj != null)
            {
                if(other.gameObject == specificObj)
                {
                    SetTrigger();
                }
            }
            //catch all
            else if(!playerOnly && !npcOnly && !pearlOnly && specificObj == null)
            {
                if (other.gameObject.tag == "NPC" || other.gameObject.tag == "Player" || other.gameObject.tag == "Pearl")
                {
                    SetTrigger();
                }
            }
           
        }
    }

    public void SetCanTrigger(bool state)
    {
        canTrigger = true;
    }

    public void SetTrigger()
    {
        if (waits)
        {
            StartCoroutine(WaitToTrigger());
        }
        else
        {
            Activate();
        }

        hasTriggered = true;
    }

    IEnumerator WaitToTrigger()
    {
        yield return new WaitForSeconds(waitTime);

        Activate();
    }

    void Activate()
    {
        //invoke the events
        for (int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }

        //call repeat if necessary 
        if (repeats)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetTime);

        hasTriggered = false;
    }
}
