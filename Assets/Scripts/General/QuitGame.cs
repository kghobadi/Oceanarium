using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //activate quit group   
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitGroup.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //activate restart group
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            restartGroup.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //on the 'no' under q prompts
    public void DeactivateObj(GameObject obj)
    {
        obj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
