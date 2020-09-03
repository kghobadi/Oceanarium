using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.AI;

public class TimelinePlaybackManager : MonoBehaviour
{
    [Header("Timeline References")]
    public PlayableDirector playableDirector;

    [Header("Timeline Settings")]
    public bool playTimelineOnlyOnce = true;

    [Header("Player Input Settings")]
    public KeyCode interactKey;
    public bool disablePlayerInput = false;

    [Header("Player Timeline Position")]
    public bool parentPlayerToPos = false;
    public Transform playerTimelinePosition;
    public Transform playerExitPosition;

    [Header("NPC Character Settings")]
    //into cinematic
    public Transform[] characterTransforms;
    public Transform[] newCharacterPositions;
    public bool[] parentCharacters;
    //leaving cinematic
    public Transform[] exitPositions;

    [Header("Trigger Zone Settings")]
    public GameObject triggerZoneObject;

    [Header("UI Interact Settings")]
    public bool displayUI;
    public GameObject interactDisplay;

    [Header("Player Settings")]
    public string playerTag = "Player";
    private GameObject playerObject;
    private PlayerCutsceneSpeedController playerCutsceneSpeedController;

    [Header("Fades")]
    public FadeUI cinematicFade;
    public bool fadesIn, fadesOut;

    private bool playerInZone = false;
    private bool timelinePlaying = false;
    private float timelineDuration;

    void Awake()
    {
        playerObject = GameObject.FindWithTag(playerTag);
        playerCutsceneSpeedController = playerObject.GetComponent<PlayerCutsceneSpeedController>();
    }

    void Start()
    {
        ToggleInteractUI(false);
    }

    public void PlayerEnteredZone()
    {
        playerInZone = true;
        ToggleInteractUI(playerInZone);
    }

    public void PlayerExitedZone()
    {
        playerInZone = false;
        ToggleInteractUI(playerInZone);
    }

    void Update()
    {
        if (playerInZone && !timelinePlaying)
        {
            var activateTimelineInput = Input.GetKey(interactKey);

            if (interactKey == KeyCode.None)
            {
                StartTimeline();
            }
            else
            {
                if (activateTimelineInput)
                {
                    StartTimeline();
                    ToggleInteractUI(false);
                }
            }
        }

        //cinematic is active!
        if (timelinePlaying)
        {
            //hard set rotations of parented characters 
            for (int i = 0; i < characterTransforms.Length; i++)
            {
                if (parentCharacters[i])
                {
                    characterTransforms[i].localEulerAngles = Vector3.zero;
                }
            }
        }
    }

    public void StartTimeline()
    {
        if (fadesIn)
        {
            StartCoroutine(WaitForFade());
        }
        else
            PlayTimeline();
    }

    IEnumerator WaitForFade()
    {
        if(cinematicFade)
            cinematicFade.FadeIn();

        yield return new WaitForSeconds(1f);

        if(cinematicFade)
            cinematicFade.FadeOut();

        PlayTimeline();
    }

    //ACTUALLY PLAYS TIMELINE 
    public void PlayTimeline()
    {
        if (playerTimelinePosition)
        {
            SetPlayerToTimelinePosition(playerTimelinePosition, parentPlayerToPos);
        }

        if (characterTransforms.Length > 0)
        {
            for (int i = 0; i < characterTransforms.Length; i++)
            {
                SetCharacterPosition(i);
            }
        }

        if (playableDirector)
        {
            playableDirector.Play();
        }

        timelinePlaying = true;

        StartCoroutine(WaitForTimelineToFinish());
    }

    //TIMELINE wait -- END
    IEnumerator WaitForTimelineToFinish()
    {

        timelineDuration = (float)playableDirector.duration;

        ToggleInput(false);

        //fade before end
        if (fadesOut)
        {
            yield return new WaitForSeconds(timelineDuration - 1f);
            cinematicFade.FadeIn();
            yield return new WaitForSeconds(1f);
            cinematicFade.FadeOut();
            EndTimeline();
        }
        //end immediately after wait 
        else
        {
            yield return new WaitForSeconds(timelineDuration);
            EndTimeline();
        }
    }

    //timeline ends 
    void EndTimeline()
    {
        ToggleInput(true);
        
        if(playerExitPosition)
            SetPlayerToTimelinePosition(playerExitPosition, false);

        if (characterTransforms.Length > 0)
        {
            for (int i = 0; i < characterTransforms.Length; i++)
            {
                ResetCharacters(i);
            }
        }

        timelinePlaying = false;

        //disable cinematic object
        if (playTimelineOnlyOnce)
        {
            playerInZone = false;
            gameObject.SetActive(false);
        }
        //reactivates trigger
        else
        {
            triggerZoneObject.SetActive(true);
        }
    }

    void ToggleInput(bool newState)
    {
        if (disablePlayerInput)
        {
            playerCutsceneSpeedController.SetPlayerSpeed();
        }
    }

    void ToggleInteractUI(bool newState)
    {
        if (displayUI)
        {
            interactDisplay.SetActive(newState);
        }
    }

    //called when cinematic begins 
    void SetPlayerToTimelinePosition(Transform pos, bool parent)
    {
        if (parent)
            playerObject.transform.SetParent(pos);
        else
            playerObject.transform.SetParent(null);
        playerObject.transform.position = pos.position;
        playerObject.transform.localRotation = pos.rotation;
    }

    //called when cinematic begins 
    void SetCharacterPosition(int index)
    {
        if (parentCharacters[index])
            characterTransforms[index].SetParent(newCharacterPositions[index]);
        characterTransforms[index].position = newCharacterPositions[index].position;
        characterTransforms[index].localRotation = newCharacterPositions[index].rotation;

    }

    //called when cinematic ends 
    void ResetCharacters(int index)
    {
        //unparent character and set position 
        if (parentCharacters[index])
            characterTransforms[index].SetParent(null);
        characterTransforms[index].position = exitPositions[index].position;
        characterTransforms[index].localRotation = exitPositions[index].rotation;
    }

}