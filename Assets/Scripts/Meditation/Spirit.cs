using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spirit : MonoBehaviour 
{
	PlayerController pc;
	MeditationLayers meditationLayers;
    MonologueManager monoManager;
    SpeakerAnimations animations;

    [Tooltip("The layer where this spirit/activity becomes visible")]
	public MeditationLayers.MeditationLayer visibleLayer;
    public bool s_Enabled;

    [Header("Events")]
    public UnityEvent enableSpirit;
    public UnityEvent disableSpirit;

    [Header("Monologue?")]
    public bool hasMono;
    public int spiritMonologueIndex;
    int physicalMono;

    [Header("Animation")]
    public string spiritState;
    string physicalState;

    private void Awake()
    {
        //comp refs
        pc = FindObjectOfType<PlayerController>();
        meditationLayers = FindObjectOfType<MeditationLayers>();
        monoManager = GetComponent<MonologueManager>();
        animations = GetComponent<SpeakerAnimations>();
        if(animations == null)
            animations = GetComponentInChildren<SpeakerAnimations>();

        //event binding
        meditationLayers.ascended.AddListener(EnableSpirit);
        pc.endMeditation.AddListener(DisableSpirit);
    }

    private void OnBecameVisible()
    {
       
    }

    void EnableSpirit()
    {
        if (s_Enabled)
            return;

        //check that 
        if(meditationLayers.meditationLayer >= visibleLayer)
        {
            //set monologue manager & trigger
            if (hasMono)
            {
                //get index of physical mono
                physicalMono = monoManager.currentMonologue;
                //set monologue system to spirit monologue
                monoManager.SetMonologueSystem(spiritMonologueIndex);
                //set spirit enabled
                monoManager.spiritEnabled = true;
                //set monologue trigger
                monoManager.mTrigger.spiritEnabled = true;
            }

            //is there a spirit state && animation script?
            if(spiritState.Length > 2 && animations)
            {
                //set current physical bool
                physicalState = animations.currentAnimatorBool;
                //set animator
                animations.SetAnimator(spiritState);
            }

            enableSpirit.Invoke();
            s_Enabled = true;
        }
    }

    private void OnBecameInvisible()
    {
       
    }

    void DisableSpirit()
    {
        //set monologue manager & trigger
        if (hasMono)
        {
            //reset monologue system to physical monologue
            monoManager.SetMonologueSystem(physicalMono);
            //reset spirit 
            monoManager.spiritEnabled = false;
            //reset monologue trigger
            monoManager.mTrigger.spiritEnabled = false;
        }

        //is there a spirit state && animation script?
        if (physicalState != "" && animations)
        {
            //reset animator to physical state
            animations.SetAnimator(physicalState);
        }

        disableSpirit.Invoke();

        s_Enabled = false;
    }
}
