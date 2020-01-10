using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonologueTrigger : MonoBehaviour
{
    //general
    public bool hasActivated;

    //monologues
    public MonologueText[] myMonologues;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!hasActivated)
            {
                for (int i = 0; i < myMonologues.Length; i++)
                {
                    myMonologues[i].EnableMonologue();
                }

                hasActivated = true;
            }
        }
    }

    void OnEnable()
    {
        hasActivated = false;
    }

    void OnDisable()
    {
        if (hasActivated)
        {
            for (int i = 0; i < myMonologues.Length; i++)
            {
                myMonologues[i].DisableMonologue();
            }
        }
    }

}
