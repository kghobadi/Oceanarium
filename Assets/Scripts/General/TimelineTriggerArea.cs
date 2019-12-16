using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineTriggerArea : MonoBehaviour {

	[Header("Component References")]
	public TimelinePlaybackManager timelinePlaybackManager;

	[Header("Settings")]
	public string playerString = "Player";

    public bool hasActivated;


	void OnTriggerEnter(Collider theCollision){
        if(theCollision.gameObject.tag == playerString && !hasActivated)
        {
            timelinePlaybackManager.PlayerEnteredZone();
            hasActivated = true;
        }
		
	}

	void OnTriggerExit(Collider theCollision){
        if (theCollision.gameObject.tag == playerString)
        {
            timelinePlaybackManager.PlayerExitedZone();
        }
       
	}
}
