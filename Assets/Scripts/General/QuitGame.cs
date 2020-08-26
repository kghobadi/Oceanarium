using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class QuitGame : MonoBehaviour {
    public GameObject escMenu;

    [Header("Esc Menu Selections & Sub Menus")]
    public StarSelectors[] menuSelections;
    public GameObject[] subMenus;
    public int currentSelector = 0;

    [Header("Controls UI")]
    public GameObject controlsUI;
    public GameObject controlsKeyboard;
    public GameObject controllerInputs;

    //for controller selections 
    bool canChange;
    float changeTimer, changeReset = 0.25f;
    
    void Start()
    {
        escMenu.SetActive(false);
    }

    void Update ()
    {
        bool pressed = false;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            //handles controller inputs for the menu
            ControllerSelection();
            //resets when you change selectors
            ChangeReset();
        }
        //mouse & keyboard
        else
        {

        }

        //activate quit group   
        if ((Input.GetKeyDown(KeyCode.Escape) ||  inputDevice.Command.WasPressed) && escMenu.activeSelf == false && !pressed)
        {
            ActivateQuitMenu();

            pressed = true;
        }

        //either turns off all sub menus, or leaves esc menu 
        if ((Input.GetKeyDown(KeyCode.Escape) || inputDevice.Command.WasPressed) && escMenu.activeSelf == true && !pressed)
        {
            bool subMenus = CheckSubMenusActive();

            if (subMenus)
                DeactivateAllSubMenus();
            else
                DeactivateObj(escMenu);

            pressed = true;
        }
    }

    //function controls selection with controller
    void ControllerSelection()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        float inputValY = inputDevice.LeftStickY;

        //detection of changing 
        if (canChange)
        {
            //pos val, selection moves up
            if (inputValY > 0)
            {
                ChangeMenuSelector(true);
            }
            //neg val, selection moves down
            else if (inputValY < 0)
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
        if(canChange == false)
        {
            changeTimer += Time.deltaTime;

            if(changeTimer > changeReset)
            {
                canChange = true;
            }
        }
    }

    //called to open esc menu 
    public void ActivateQuitMenu()
    {
        escMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //set Selector
        currentSelector = 0;
        if(menuSelections.Length > 0)
            menuSelections[currentSelector].ActivateStars();
    }

    //checks if there is a sub menu open
    bool CheckSubMenusActive()
    {
        for(int i = 0; i < subMenus.Length; i++)
        {
            if (subMenus[i].activeSelf)
                return true;
        }

        return false;
    }

    //turns off all subMenus
    public void DeactivateAllSubMenus()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            subMenus[i].SetActive(false);
        }
    }

    public void ShowControls()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //enable controls header obj
        controlsUI.SetActive(true);

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            controllerInputs.SetActive(true);
            controlsKeyboard.SetActive(false);
        }
        //mouse & keyboard
        else
        {
            controlsKeyboard.SetActive(true);
            controllerInputs.SetActive(false);
        }
    }
    
    //on the 'no' under q prompts
    public void DeactivateObj(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
