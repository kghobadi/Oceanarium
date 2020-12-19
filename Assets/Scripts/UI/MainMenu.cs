using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	SaveSystem saveSystem;
	LoadSceneAsync loadScene;
    MenuSelections menuSelections;

    public SpaceToStart spaceToStart;
	public GameObject stsObj;
   
	void Awake () 
	{
		//get refs
		saveSystem = FindObjectOfType<SaveSystem>();
		loadScene = FindObjectOfType<LoadSceneAsync>();
        menuSelections = GetComponent<MenuSelections>();
		//add callbacks
		saveSystem.startNewGame.AddListener(NewGame);
		saveSystem.returningGame.AddListener(PlayerReturning);
	}

    //called at start if first ever session 
    void NewGame()
    {
		//enable space to start ui 
		stsObj.SetActive(true);
        //disable menu selections
        menuSelections.DeactivateSelections();
		menuSelections.DeactivateMenu();
	}

	//called at start when player returns to game for another session
	void PlayerReturning()
    {
		//disable space to start
		stsObj.SetActive(false);
        //enable menu selections
        menuSelections.ActivateMenu();
		menuSelections.ActivateSelections();
		//make sure enable cursor
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	//new game button calls this
	public void StartNewGame()
    {
		//delete all prefs 
		PlayerPrefs.DeleteAll();
		//and begin fades
		spaceToStart.StartFades();
        //disable selections
        menuSelections.DeactivateSelections();
		menuSelections.DeactivateMenu();
	}

	//using values from save system, load player
	public void ContinueGame()
	{   
		//load proper scene 
		int scene = PlayerPrefs.GetInt("Galaxy");
		SceneManager.LoadScene(scene);
	}
}
