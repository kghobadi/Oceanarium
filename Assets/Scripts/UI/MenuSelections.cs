using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

//this script allows you to move around menus with a controller
public class MenuSelections : MonoBehaviour
{
    public bool menuActive;
    public StarSelectors[] menuSelections;
    public GameObject[] subMenus;
    public int currentSelector = 0;

    //for controller selections 
    bool canChange;
    float changeTimer, changeReset = 0.25f;
    InputDevice inputDevice;

    void Update ()
    {
        if (menuActive)
        {
            //get input device 
            inputDevice = InputManager.ActiveDevice;

            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                //handles controller inputs for the menu
                ControllerSelection();
                //resets when you change selectors
                ChangeReset();
            }
        }
    }

    //activates menu and resets selection
    public void ActivateMenu()
    {
        menuActive = true;

        currentSelector = 0;
        if (menuSelections.Length > 0)
            menuSelections[currentSelector].ActivateStars();
    }
    
    public void DeactivateMenu()
    {
        menuActive = false;
    }

    //function controls selection with controller
    void ControllerSelection()
    {
        //get inputY
        float inputValY = inputDevice.LeftStickY;

        //detection of changing 
        if (canChange)
        {
            //pos val, selection moves up
            if (inputValY < 0)
            {
                ChangeMenuSelector(true);
            }
            //neg val, selection moves down
            else if (inputValY > 0)
            {
                ChangeMenuSelector(false);
            }
        }

        //detection of selection
        if (inputDevice.Action1.WasPressed)
        {
            if (menuSelections[currentSelector])
                menuSelections[currentSelector].SelectMe();
        }

        //detection of closing submenus
        if (inputDevice.Action2.WasPressed)
        {
            DeactivateAllSubMenus();
        }
    }

    //allows you to increment menu selector & change stars
    public void ChangeMenuSelector(bool upOrDown)
    {
        if (menuSelections.Length > 1)
        {
            //deactivate current stars
            menuSelections[currentSelector].DeactivateStars();

            //up
            if (upOrDown)
            {
                //increment up
                if (currentSelector < menuSelections.Length - 1)
                {
                    currentSelector++;
                }
                else
                {
                    currentSelector = 0;
                }
            }
            //down
            else
            {
                //increment down
                if (currentSelector > 0)
                {
                    currentSelector--;
                }
                else
                {
                    currentSelector = menuSelections.Length - 1;
                }
            }

            //activate next stars
            menuSelections[currentSelector].ActivateStars();

            //change reset called 
            canChange = false;
            changeTimer = 0;
        }
    }

    //handles reset timer for controller selection
    void ChangeReset()
    {
        if (canChange == false)
        {
            changeTimer += Time.deltaTime;

            if (changeTimer > changeReset)
            {
                canChange = true;
            }
        }
    }

    //turns on all menu selections
    public void ActivateSelections()
    {
        for (int i = 0; i< menuSelections.Length; i++)
        {
            menuSelections[i].gameObject.SetActive(true);
        }
    }

    //turns off all menu selections
    public void DeactivateSelections()
    {
        for (int i = 0; i < menuSelections.Length; i++)
        {
            menuSelections[i].gameObject.SetActive(false);
        }
    }

    //turns off all subMenus
    public void DeactivateAllSubMenus()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            subMenus[i].SetActive(false);
        }
    }

    //checks if there is a sub menu open
    public bool CheckSubMenusActive()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            if (subMenus[i].activeSelf)
                return true;
        }

        return false;
    }
}
