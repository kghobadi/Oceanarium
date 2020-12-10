using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class QuitGame : MonoBehaviour {
    PlayerController pc;
    CameraController camControl;
    MenuSelections mainMenuSelections;
    SaveSystem saveSystem;

    public GameObject escMenu;
    [Header("Controls UI")]
    public GameObject controlsUI;

    void Awake()
    {
        mainMenuSelections = GetComponent<MenuSelections>();
        pc = FindObjectOfType<PlayerController>();
        camControl = FindObjectOfType<CameraController>();
        saveSystem = FindObjectOfType<SaveSystem>();
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
                DeactiveMenu();
            }
                
            pressed = true;
        }
    }
    
    //called to open esc menu 
    public void ActivateQuitMenu()
    {
        //disable movement
        if (pc)
        {
            //meditation check
            if (pc.moveState == PlayerController.MoveStates.MEDITATING)
            {
                //turn it off
                pc.DisableMeditation();

                //return camera to player
                camControl.SetCamPos(camControl.heightAvg);
            }

            //dont open menu if player is in convo
            else if (pc.moveState == PlayerController.MoveStates.PEARLMED || pc.moveState == PlayerController.MoveStates.TALKING)
                return;

            pc.DisableMovement(true);
        }
        if(camControl)
            camControl.canMoveCam = false;
      
        //enable cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //activate menu & set Selectors
        escMenu.SetActive(true);
        mainMenuSelections.ActivateMenu();
    }
    
    public void DeactiveMenu()
    {
        //leave menu
        mainMenuSelections.DeactivateMenu();
        DeactivateObj(escMenu);

        //enable movement 
        if (pc)
        {
            if(pc.moveState != PlayerController.MoveStates.TALKING && pc.moveState != PlayerController.MoveStates.PEARLMED)
                pc.EnableMovement(true);
        }
        if(camControl)
            camControl.canMoveCam = true;   

        //relock cursor 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowControls()
    {
        //enable controls header obj
        controlsUI.SetActive(true);
    }
    
    //on the 'no' under q prompts
    public void DeactivateObj(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void Quit()
    {
        //save prefs
        if (saveSystem.saveEnabled)
            saveSystem.SavePrefs();

        Application.Quit();
    }

    public void RestartGame()
    {
        //save prefs
        if(saveSystem.saveEnabled)
            saveSystem.SavePrefs();

        SceneManager.LoadScene(0);
    }
}
