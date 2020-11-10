using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSelector : MonoBehaviour {

    Guardian guardian;
    PlayerController pc;
    CameraController cc;
    SaveSystem saveSystem;

    public PlanetManager planet;
    public FadeSprite blackground;

    public bool unlocked;

	void Awake ()
    {
        guardian = FindObjectOfType<Guardian>();
        pc = FindObjectOfType<PlayerController>();
        cc = FindObjectOfType<CameraController>();
        saveSystem = FindObjectOfType<SaveSystem>();

        CheckLockState();
	}

    void CheckLockState()
    {
        if (PlayerPrefs.GetString(planet.planetName) == "unlocked")
            unlocked = true;
    }
	
    //teleports player to planet
	public void TeleportTo()
    {
        if (unlocked)
        {
            if (pc.activePlanet != planet)
            {
                //deactivate current planet
                pc.activePlanet.DeactivatePlanet();

                //actual TP
                if(planet.teleportationPoint) //tp player to tp point
                    pc.transform.position = planet.teleportationPoint.position;
                else //tp player to planet start point
                    pc.transform.position = planet.playerStartingPoint.position;

                //tp camera? 
                cc.SetCamPos();
                //teleport guardian 
                if (Vector3.Distance(pc.transform.position, guardian.transform.position) > 50f)
                    guardian.TeleportGuardian(planet.playerStartingPoint.position);
                //activate planet 
                planet.ActivatePlanet(planet.guardianBehaviors[0].guardianLocation.position);
                //fade to music
                planet.mFader.FadeTo(planet.musicTrack);

                //fade out starting blackground
                if (blackground)
                    blackground.FadeOut();
            }
        }
        //locked
        else
        {
            //maybe play a sound that indicates this is not available?
            Debug.Log(planet.planetName + " is locked");
        }
    }

    //NEED TO CALL THIS WHEN PLAYER FINISHES PLANET -- i.e. activates current/RUINS
    //called when player first reaches planet 
    public void UnlockPlanet()
    {
        unlocked = true;

        PlayerPrefs.SetString(planet.planetName, "unlocked");
    }

    //debug unlock -- does not save
    public void UnlockTemporary()
    {
        unlocked = true;
    }
}
