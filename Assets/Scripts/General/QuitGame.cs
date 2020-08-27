using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class QuitGame : MonoBehaviour {
    
    MenuSelections mainMenuSelections;

    public GameObject escMenu;
    [Header("Controls UI")]
    public GameObject controlsUI;
    public GameObject controlsKeyboard;
    public GameObject controllerInputs;

    void Awake()
    {
        mainMenuSelections = GetComponent<MenuSelections>();
    }

    void Start()
    {
        escMenu.SetActive(false);
    }

    void Update ()
    {
        bool pressed = false;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        
        //activate quit group   
        if ((Input.GetKeyDown(KeyCode.Escape) ||  inputDevice.Command.WasPressed) && escMenu.activeSelf == false && !pressed)
        {
            ActivateQuitMenu();

            pressed = true;
        }

        //either turns off all sub menus, or leaves esc menu 
        if ((Input.GetKeyDown(KeyCode.Escape) || inputDevice.Command.WasPressed) && escMenu.activeSelf == true && !pressed)
        {
            bool subMenus = mainMenuSelections.CheckSubMenusActive();

            if (subMenus)
                mainMenuSelections.DeactivateAllSubMenus();
            else
            {
                mainMenuSelections.DeactivateMenu();
                DeactivateObj(escMenu);
            }
                

            pressed = true;
        }
    }
    
    //called to open esc menu 
    public void ActivateQuitMenu()
    {
        escMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //activate menu & set Selectors
        mainMenuSelections.ActivateMenu();
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
