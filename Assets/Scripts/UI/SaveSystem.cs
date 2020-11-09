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

	void Awake()
	{
		pc = FindObjectOfType<PlayerController>();
	}

    private void OnEnable()
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
			//get session
			sessions = PlayerPrefs.GetInt("Sessions");
			//get last galaxy scene
			currentGalaxy = PlayerPrefs.GetInt("Galaxy");
			//planets
			LoadPlanets();

			Debug.Log("loading game");
		}
		//
        else
        {
			//first session
			sessions = 0;
			PlayerPrefs.SetInt("Sessions", sessions);
			//invoke start game -- planet which is starting will listen
			startNewGame.Invoke();

			Debug.Log("started new game");
		}
    }

	public void LoadPlanets()
    {
		//get saved planet name 
		string savedPlanet = PlayerPrefs.GetString("ActivePlanet");

		//loop thru planet names
		for (int i = 0; i < planetNames.Length; i++)
        {
			//check for match
			if(savedPlanet == planetNames[i])
            {
				//set last planet manager
				lastPlanet = allPlanets[i];

				//start player there
				lastPlanet.StartPlayerAtPlanet();
			}
		}
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
	}

    //just for testing...
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
			ResetPlayerPrefs();
        }
    }

    //deletes all player prefs
    public void ResetPlayerPrefs()
    {
		PlayerPrefs.DeleteAll();
    }

    private void OnDisable()
    {
		if(saveEnabled)
			SavePrefs();
    }
}
