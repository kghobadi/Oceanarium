using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TimelinePlaybackManager : MonoBehaviour {


	[Header("Timeline References")]
	public PlayableDirector playableDirector;

	[Header("Timeline Settings")]
	public bool playTimelineOnlyOnce;

	[Header("Player Input Settings")]
	public KeyCode interactKey;
	public bool disablePlayerInput = false;
	private PlayerController inputController;

	[Header("Player Timeline Position")]
	public bool setPlayerTimelinePosition = false;
	public Transform playerTimelinePosition;

	[Header("Trigger Zone Settings")]
	public GameObject triggerZoneObject;

	[Header("UI Interact Settings")]
	public bool displayUI;
	public GameObject interactDisplay;

    [Header("Guardian Settings")]
    public bool resetGuardianBehavior;
    [Tooltip("Planet with next Guardian points")]
    public PlanetManager planetPoints;
    [Tooltip("First place guardian will travel to")]
    public Transform nextSpot;
    GameObject guardian;
    Guardian gBehavior;

    [Header("Current Settings")]
    public Currents currentToActivate;
    public bool pullsPlayerIntoCurrent;

    [Header("Player Settings")]
	public string playerTag = "Player";
	private GameObject playerObject;
	private PlayerCutsceneSpeedController playerCutsceneSpeedController;

	private bool playerInZone = false;
	private bool timelinePlaying = false;
	private float timelineDuration;

	void Awake(){
		playerObject = GameObject.FindWithTag (playerTag);
		inputController = playerObject.GetComponent<PlayerController> ();
		playerCutsceneSpeedController = playerObject.GetComponent<PlayerCutsceneSpeedController> ();
        guardian = GameObject.FindGameObjectWithTag("Guardian");
        gBehavior = guardian.GetComponent<Guardian>();
	}

    void Start()
    {
        ToggleInteractUI(false);
    }

    public void PlayerEnteredZone(){
		playerInZone = true;
		ToggleInteractUI (playerInZone);
	}

	public void PlayerExitedZone(){
		
		playerInZone = false;

		ToggleInteractUI (playerInZone);

	}
		
	void Update(){

		if (playerInZone && !timelinePlaying) {

			var activateTimelineInput = Input.GetKey (interactKey);

			if (interactKey == KeyCode.None) {
				PlayTimeline ();
			} else {
				if (activateTimelineInput) {
					PlayTimeline ();
					ToggleInteractUI (false);
				}
			}
				
		}

	}

	public void PlayTimeline(){

		if (setPlayerTimelinePosition) {
			SetPlayerToTimelinePosition ();
		}

		if (playableDirector) {
			
			playableDirector.Play ();

		}

        //reset guardian AI for this cinematic
        if (resetGuardianBehavior)
        {
            gBehavior.ResetGuardianLocation(nextSpot.position, planetPoints.guardianLocations, planetPoints.planetColliders);
        }
			
        //has current to activate
        if(currentToActivate != null)
        {
            if (!currentToActivate.currentActivated)
                currentToActivate.ActivateCurrent();

            //player enters current 
            if (pullsPlayerIntoCurrent)
            {
                currentToActivate.EnterCurrent();
            }
        }

		timelinePlaying = true;
			
		StartCoroutine (WaitForTimelineToFinish());
			
	}

	IEnumerator WaitForTimelineToFinish(){

		timelineDuration = (float)playableDirector.duration;
		
		ToggleInput (false);
		yield return new WaitForSeconds(timelineDuration);
		ToggleInput (true);

			
		if (!playTimelineOnlyOnce) {
			triggerZoneObject.SetActive (true);
		} else if (playTimelineOnlyOnce) {
			playerInZone = false;
		}

		timelinePlaying = false;

	}
		
	void ToggleInput(bool newState){
		if (disablePlayerInput){
			playerCutsceneSpeedController.SetPlayerSpeed ();
			inputController.canMove = newState;
		}
	}


	void ToggleInteractUI(bool newState){
		if (displayUI) {
			interactDisplay.SetActive (newState);
		}
	}

	void SetPlayerToTimelinePosition(){
		playerObject.transform.position = playerTimelinePosition.position;
		playerObject.transform.localRotation = playerTimelinePosition.rotation;
	}

}