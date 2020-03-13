using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class QuitGame : MonoBehaviour {
    public GameObject quitGroup;
    public GameObject restartGroup;

    void Start()
    {
        quitGroup.SetActive(false);
        restartGroup.SetActive(false);
    }

    void Update ()
    {
        bool pressed = false;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //activate quit group   
        if ((Input.GetKeyDown(KeyCode.Escape) ||  inputDevice.Command.WasPressed) && quitGroup.activeSelf == false && !pressed)
        {
            ActivateQuitMenu();

            pressed = true;
        }

        //quit
        if(inputDevice.Action1.WasPressed && quitGroup.activeSelf == true)
        {
            Quit();
        }

        //deactivate quit menu
        if ((Input.GetKeyDown(KeyCode.Escape) || inputDevice.Command.WasPressed) && quitGroup.activeSelf == true && !pressed)
        {
            DeactivateObj(quitGroup);

            pressed = true;
        }

        //activate restart group
        if (Input.GetKeyDown(KeyCode.Delete) && restartGroup.activeSelf == false && !pressed)
        {
            ActivateRestartMenu();

            pressed = true;
        }
        //deactivate restart menu
        if (Input.GetKeyDown(KeyCode.Delete) && restartGroup.activeSelf == true && !pressed)
        {
            DeactivateObj(restartGroup);

            pressed = true;
        }
    }

    public void ActivateQuitMenu()
    {
        quitGroup.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //turn off restart if open
        if (restartGroup.activeSelf)
            restartGroup.SetActive(false);
    }

    public void ActivateRestartMenu()
    {
        restartGroup.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //turn off quit if open
        if (quitGroup.activeSelf)
            quitGroup.SetActive(false);
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
