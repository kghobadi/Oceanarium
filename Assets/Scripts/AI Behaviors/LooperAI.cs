using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a basic AI that moves back and forth between a few points
//Moves to a point, then Idles in that location
public class LooperAI : AudioHandler {
    //player vars 
    GameObject player;
    PlayerController pc;
    //ai components
    MoveTowards movement;
    CreatureAnimation cAnimation;

    [Header("AI Settings")]
    public AIStates aiState;
    public enum AIStates
    {
        IDLE, MOVING, TALKING,
    }

    [Tooltip("Points for AI to travel between")]
    public Transform[] waypoints;
    public int currentPoint = -1;

    public float idleTimer, idleTime;

    [Header("Sounds")]
    public AudioClip[] swimmingSounds;
    public AudioClip[] idleSounds;

    public float nextSoundIn;
    public float idleSoundFreq, movingSoundsFreq;
    
    MonologueManager monoManager;

    public override void Awake()
    {
        base.Awake();
        //player refs
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();

        //ai component refs 
        cAnimation = GetComponent<CreatureAnimation>();
        movement = GetComponent<MoveTowards>();

        //monologue integration
        monoManager = GetComponent<MonologueManager>();
    }

    void Start ()
    {
        SetMove();
	}
	
	void Update () {
        //IDLE
        if(aiState == AIStates.IDLE)
        {
            SoundCountdown(idleSounds, idleSoundFreq);

            idleTimer += Time.deltaTime;
            //idle over 
            if(idleTimer > idleTime)
            {
                SetMove();
            }
        }

        //MOVING
        if (aiState == AIStates.MOVING)
        {
            //swim away 
            if (!cAnimation.Animator.GetBool("swim"))
                cAnimation.SetAnimator("swim");

            // play swimming sounds
            SoundCountdown(swimmingSounds, movingSoundsFreq);

            //movement running
            if (movement.moving == false)
            {
                SetIdle();
            }
        }

        //TALKING 
        if(aiState == AIStates.TALKING)
        {
            //once monologue is over return to idle
            if(monoManager.inMonologue == false)
            {
                SetIdle();
            }
        }
    }

    //set ai to idle 
    void SetIdle()
    {
        aiState = AIStates.IDLE;
        cAnimation.SetAnimator("idle");
        idleTimer = 0;
    }

    //sets move to next point in guardian locations 
    void SetMove()
    {
        //inc point 
        if (currentPoint < waypoints.Length - 1)
        {
            currentPoint++;
        }
        else
        {
            currentPoint = 0;
        }

        movement.MoveTo(waypoints[currentPoint].position, movement.moveSpeed + currentPoint);
        aiState = AIStates.MOVING;
    }

    //countsdown for next sound to play, uses sound array to play it 
    void SoundCountdown(AudioClip[] sounds, float soundFreq)
    {
        nextSoundIn -= Time.deltaTime;

        if (nextSoundIn < 0)
        {
            if(!myAudioSource.isPlaying)
                PlayRandomSoundRandomPitch(sounds, 1f);

            nextSoundIn = soundFreq;
        }
    }

    //called when player initiates Monologue 
    public void SetTalking()
    {
        //stop moving!
        if (movement.moving)
            movement.moving = false;

        aiState = AIStates.TALKING;
        cAnimation.SetAnimator("idle");
    }
}
