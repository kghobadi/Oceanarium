using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrownPlantAudio : AudioHandler {
    [Header("Sounds")]
    public AudioClip growthSound;
    public AudioClip playerTouchedMe;

    DistanceAnimatorParameter distanceAnim;
    public float distToMakeSound = 5f;

    void Start()
    {
        distanceAnim = GetComponent<DistanceAnimatorParameter>();

        if(growthSound)
            PlaySoundRandomPitch(growthSound, myAudioSource.volume);
    }

    void Update()
    {
        if(distanceAnim.distance < distToMakeSound)
            PlaySoundRandomPitch(playerTouchedMe, myAudioSource.volume);
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
