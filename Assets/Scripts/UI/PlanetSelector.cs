using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSelector : MonoBehaviour {

    Guardian guardian;
    PlayerController pc;
    CameraController cc;

    public PlanetManager planet;
    public FadeSprite blackground;
    
	void Awake ()
    {
        guardian = FindObjectOfType<Guardian>();
        pc = FindObjectOfType<PlayerController>();
        cc = FindObjectOfType<CameraController>();
	}
	
	public void TeleportTo()
    {
        if(pc.activePlanet != planet)
        {
            //deactivate current planet
            pc.activePlanet.DeactivatePlanet();

            //tp player
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
}
