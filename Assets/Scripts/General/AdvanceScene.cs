using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class AdvanceScene : MonoBehaviour
{
    public float timeToRestart = 5f;
    public float restartTimer;

    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //next scene 
        //if (Input.GetKeyDown(KeyCode.Return) || inputDevice.RightBumper.WasPressed)
        //{
        //    LoadNextScene();
        //}

        ////previous scene 
        //if (Input.GetKeyDown(KeyCode.CapsLock) || inputDevice.LeftBumper.WasPressed)
        //{
        //    LoadPreviousScene();
        //}

        //restart game
        if (Input.GetKey(KeyCode.Delete) || (inputDevice.Command))
        {
            restartTimer += Time.deltaTime;

            if (restartTimer > timeToRestart)
            {
                Restart();
            }
        }
        else
        {
            restartTimer = 0;
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPreviousScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        if(index > 0)
            SceneManager.LoadScene(index - 1);
    }

    public void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(index + 1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
