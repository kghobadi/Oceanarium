using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour {

    public bool hasTriggered;
    public bool playerOnly;
    public bool waitToTrigger;
    public float waitTime = 5f;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;


    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (playerOnly)
            {
                if (other.gameObject.tag == "Player")
                {
                    if (waitToTrigger)
                        StartCoroutine(WaitToTrigger());
                    else
                        SetTrigger();

                    Debug.Log(other.gameObject.name + " triggered " + gameObject.name);
                }
            }
            else
            {
                if (other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
                {
                    if (waitToTrigger)
                        StartCoroutine(WaitToTrigger());
                    else 
                        SetTrigger();

                    Debug.Log(other.gameObject.name + " triggered " + gameObject.name);
                }
            }
           
        }
        
    }

    IEnumerator WaitToTrigger()
    {
        yield return new WaitForSeconds(waitTime);

        SetTrigger();
    }

    void SetTrigger()
    {
        //activate stuff
        for(int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].SetActive(true);
        }

        //deactivate stuff
        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            objectsToDeactivate[i].SetActive(false);
        }

        hasTriggered = true;
    }
}
