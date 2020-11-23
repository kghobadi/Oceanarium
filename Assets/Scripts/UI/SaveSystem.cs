using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour {

	PlayerController pc;

	public bool saveEnabled;
	[Tooltip("Current Galaxy (scene)")]
	public int currentGalaxy;
	[Tooltip("Current play through of game/app")]
	public int sessions;

	[Header("Planets")]
	public PlanetManager[] allPlanets;
	public string[] planetNames;
	public PlanetManager lastPlanet;
	[Tooltip("Debug option to unlock all planets")]
	public bool unlockAll;
	public UnityEvent startNewGame;
	public UnityEvent returningGame;

	void Awake()
	{
		pc = FindObjectOfType<PlayerController>();
	}

    private void Start()
    {
		GetPlanets();

		LoadPrefs();
	}

    //get planet info
    void GetPlanets()
    {
		//fetch all planets
		allPlanets = FindObjectsOfType<PlanetManager>();
		//generate string [] of planet names 
		planetNames = new string[allPlanets.Length];
		for(int i = 0; i < allPlanets.Length; i++)
        {
			planetNames[i] = allPlanets[i].planetName;

			//unlocks all planets temporarily
            if (unlockAll)
            {
				if(allPlanets[i].pSelector)
					allPlanets[i].pSelector.UnlockTemporary();
			}
        }
    }

	//load all available prefs
	public void LoadPrefs()
    {
		//check if player has played before
		if (PlayerPrefs.GetInt("Sessions") > 0)
		{
			LoadGame();
		}
		//no sessions, new game
		else
		{
			NewGame();
		}
	}

    void LoadGame()
    {
        //get session
        sessions = PlayerPrefs.GetInt("Sessions");
        //get last galaxy scene
        currentGalaxy = PlayerPrefs.GetInt("Galaxy");
		//for other scripts
		returningGame.Invoke();

		Debug.Log("loading game");
    }

    void NewGame()
    {
        //first session
        sessions = 0;
        PlayerPrefs.SetInt("Sessions", sessions);
        //invoke start game -- planet which is starting will listen
        startNewGame.Invoke();

        Debug.Log("started new game");
    }

	
	//save all player pref
	public void SavePrefs()
	{
		//save current scene
		PlayerPrefs.SetInt("Galaxy", SceneManager.GetActiveScene().buildIndex);
		//save current planet
		PlayerPrefs.SetString("ActivePlanet", pc.activePlanet.planetName);
		//save session 
		sessions++;
		PlayerPrefs.SetInt("Sessions", sessions);

		Debug.Log("Saving prefs");
	}

	//editor button or Tools > calls this
    //deletes all player prefs
    public void ResetPlayerPrefs()
    {
		PlayerPrefs.DeleteAll();
		Debug.Log("Deleted all player prefs");
    }

    private void OnDisable()
    {
		if(saveEnabled)
			SavePrefs();
    }
}
