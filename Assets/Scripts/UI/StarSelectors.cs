using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class StarSelectors : MonoBehaviour {
    PlanetSelector pSelector;
    [HideInInspector]
    public MenuSelections menuSelections;
    public bool deactivateMainMenu;

    public GameObject[] starsActive;
    public UnityEvent[] selectionEvents;

    void Awake()
    {
        pSelector = GetComponent<PlanetSelector>();
        if (menuSelections == null)
            menuSelections = GetComponentInParent<MenuSelections>();
    }

    private void OnEnable()
    {
        DeactivateStars();
    }

    public void ActivateStars()
    {
        //play sound from menu
        if (menuSelections)
        {
            if (menuSelections.selects.Length > 0)
            {
                menuSelections.PlayRandomSound(menuSelections.changeSelections, 1f);
            }
        }

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

        DeactivateOtherStars();
    }

    void DeactivateOtherStars()
    {
        for (int i = 0; i < menuSelections.menuSelections.Length; i++)
        {
            if (menuSelections.menuSelections[i] != this)
            {
                menuSelections.menuSelections[i].DeactivateStars();
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

   
    public void SelectMe()
    {
        //play sound from menu
        if (menuSelections)
        {
            if (menuSelections.selects.Length > 0)
            {
                menuSelections.PlayRandomSound(menuSelections.selects, 1f);
            }
        }

        //call the selection events 
        for (int i = 0; i < selectionEvents.Length; i++)
        {
            selectionEvents[i].Invoke();
        }

        if (deactivateMainMenu)
        {
            //check if submenus are active
            if (menuSelections.CheckSubMenusActive())
            {
                //disable menu selections while submenu is active
                menuSelections.DeactivateMenu();
            }
        }
    }
}
