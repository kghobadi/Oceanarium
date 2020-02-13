using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrownPlantAudio : AudioHandler {
    GameObject player;

    [Header("Sounds")]
    public AudioClip growthSound;
    [Tooltip("Add a trigger + Rigidbody to this plant/sprite and when the player touches me I will make this sound")]
    public AudioClip playerTouchedMe;

    
    [Tooltip("SET THIS TO SAME VALUE AS DISTANCE PARAM IN ANIMATOR: If player comes this close, i will make growth sound")]
    public float distToMakeSound = 5f;
    [Tooltip("Distance from player at which Growth sound will reset")]
    public float distToReset = 50f;
    public bool playedSound;

    public ParticleSystem growthParticles;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            //close enough to trigger growth 
            if (distance < distToMakeSound)
            {
                if (!playedSound && growthSound)
                {
                    PlaySoundRandomPitch(growthSound, myAudioSource.volume);
                    playedSound = true;
                    growthParticles.Play();
                }
            }
            //reset growth sound 
            else if (distance > distToReset)
            {
                playedSound = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("triggered touch");
            if(playerTouchedMe)
                PlaySoundRandomPitch(playerTouchedMe, myAudioSource.volume / 2);
        }
    }

}
