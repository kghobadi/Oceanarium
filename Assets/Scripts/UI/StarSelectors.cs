using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StarSelectors : MonoBehaviour {
    PlanetSelector pSelector;

    public GameObject[] starsActive;
    public UnityEvent[] selectionEvents;

    private void Awake()
    {
        pSelector = GetComponent<PlanetSelector>();
    }

    private void OnEnable()
    {
        DeactivateStars();
    }

    public void ActivateStars()
    {
        if (pSelector)
        {
            if(pSelector.unlocked)
                for (int i = 0; i < starsActive.Length; i++)
                {
                    starsActive[i].SetActive(true);
                }
        }
        else
        {
            for (int i = 0; i < starsActive.Length; i++)
            {
                starsActive[i].SetActive(true);
            }
        }
    }

    public void DeactivateStars()
    {
        for (int i = 0; i < starsActive.Length; i++)
        {
            starsActive[i].SetActive(false);
        }
    }

    //call the selection events 
    public void SelectMe()
    {
        for(int i = 0; i < selectionEvents.Length; i++)
        {
            selectionEvents[i].Invoke();
        }
    }
}
