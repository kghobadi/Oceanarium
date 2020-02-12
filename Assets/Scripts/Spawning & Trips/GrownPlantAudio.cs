using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrownPlantAudio : AudioHandler {
    [Header("Sounds")]
    public AudioClip growthSound;
    [Tooltip("Add a trigger + Rigidbody to this plant/sprite and when the player touches me I will make this sound")]
    public AudioClip playerTouchedMe;

    DistanceAnimatorParameter distanceAnim;
    [Tooltip("SET THIS TO SAME VALUE AS DISTANCE PARAM IN ANIMATOR: If player comes this close, i will make growth sound")]
    public float distToMakeSound = 5f;
    public bool playedSound;

    void Start()
    {
        distanceAnim = GetComponent<DistanceAnimatorParameter>();

        if(growthSound)
            PlaySoundRandomPitch(growthSound, myAudioSource.volume);
    }

    void Update()
    {
        if(distanceAnim.distance < distToMakeSound)
        {
            if(!playedSound && growthSound)
            {
                PlaySoundRandomPitch(growthSound, myAudioSource.volume);
                playedSound = true;
            }
        }
        else
        {
            playedSound = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(playerTouchedMe)
                PlaySoundRandomPitch(playerTouchedMe, myAudioSource.volume);
        }
    }

}
