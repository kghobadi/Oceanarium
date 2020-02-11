using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrownPlantAudio : AudioHandler {
    [Header("Sounds")]
    public AudioClip growthSound;
    public AudioClip playerTouchedMe;

    void Start()
    {
        if(growthSound)
            PlaySoundRandomPitch(growthSound, myAudioSource.volume);
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
