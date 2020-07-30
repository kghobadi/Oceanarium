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
            // need to check current selector based on l analog axis movement 
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

    //called to open esc menu 
    public void ActivateQuitMenu()
    {
        escMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        currentSelector = 0;
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
