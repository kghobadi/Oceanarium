using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour {
    //general
    public bool hasActivated;

    //objects
    [Header("Object Trigger")]
    public bool hasObjects;
    public GameObject[] objects;
    public GameObject[] objectsToTurnOff;

    //animation
    [Header("Animation Trigger")]
    public bool hasAnimation;
    public Animator[] myAnimators;
    public string stateName;

    //dialogue
    [Header("Dialogue Trigger")]
    public bool hasDialogue;
    public DialogueText[] myDialogues;

    //gateway
    [HideInInspector]
    public bool isEssenzGateway;
    [HideInInspector]
    public EssenzGateway myGateway;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!hasActivated)
            {
                //Object Trigger
                if (hasObjects)
                {
                    //turn on stuff
                    for (int i = 0; i < objects.Length; i++)
                    {
                        objects[i].SetActive(true);
                    }

                    //turn off stuff
                    for (int i = 0; i < objectsToTurnOff.Length; i++)
                    {
                        objectsToTurnOff[i].SetActive(false);
                    }
                }

                //dialogue trigger
                if (hasDialogue)
                {
                    Debug.Log("dialogue time!");
                    //if greater than 1
                    if (myDialogues.Length > 1)
                    {
                        for (int i = 0; i < myDialogues.Length; i++)
                        {
                            myDialogues[i].EnableDialogue();
                        }
                    }
                    else
                    {
                        myDialogues[0].EnableDialogue();
                    }
                }

                //animation trigger
                if (hasAnimation)
                {
                    //if greater than 1
                    if (myAnimators.Length > 1)
                    {
                        for (int i = 0; i < myAnimators.Length; i++)
                        {
                            myAnimators[i].SetTrigger(stateName);
                        }
                    }
                    else
                    {
                        myAnimators[0].SetTrigger(stateName);
                    }
                }
                
                hasActivated = true;
            }
        }
    }
}
