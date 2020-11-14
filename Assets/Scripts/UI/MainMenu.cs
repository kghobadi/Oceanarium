using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	SaveSystem saveSystem;
	LoadSceneAsync loadScene;

	public SpaceToStart spaceToStart;
	public GameObject stsObj;
	public GameObject newGameButton;
	public GameObject continueGameButton;

	void Awake () 
	{
		//get refs
		saveSystem = FindObjectOfType<SaveSystem>();
		loadScene = FindObjectOfType<LoadSceneAsync>();
		//add callbacks
		saveSystem.startNewGame.AddListener(NewGame);
		saveSystem.returningGame.AddListener(PlayerReturning);
	}

	void NewGame()
    {
		//disable new game & continue buttons, 
		stsObj.SetActive(true);
		newGameButton.SetActive(false);
		continueGameButton.SetActive(false);
    }

	void PlayerReturning()
    {
		//disable space to start, enalbe new game & continue buttons
		stsObj.SetActive(false);
		newGameButton.SetActive(true);
		continueGameButton.SetActive(true);
	}

	//new game button calls this
	public void StartNewGame()
    {
		//delete all prefs 
		PlayerPrefs.DeleteAll();
		//and begin
		spaceToStart.StartFades();
		//fades
		newGameButton.SetActive(false);
		continueGameButton.SetActive(false);
	}

	//using values from save system, load player
	public void ContinueGame()
	{   
		//load proper scene 
		int scene = PlayerPrefs.GetInt("Galaxy");
		SceneManager.LoadScene(scene);
	}
}
