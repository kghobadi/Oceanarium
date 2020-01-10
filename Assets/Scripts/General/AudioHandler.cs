using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public abstract class AudioHandler : MonoBehaviour
{
    [HideInInspector]
    public AudioSource myAudioSource;
    // pitch range
    [Header("Audio Handling")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    public virtual void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();

        if (myAudioSource == null)
        {
            myAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    //plays a sound with vol
    public virtual void PlaySound(AudioClip sound, float vol)
    {
        myAudioSource.PlayOneShot(sound, vol);
    }

    //plays a sound with random pitch and vol
    public virtual void PlaySoundRandomPitch(AudioClip sound, float vol)
    {
        RandomizePitch(pitchRange.x, pitchRange.y);
        myAudioSource.PlayOneShot(sound, vol);
    }

    //plays a random sound from an array with vol
    public virtual void PlayRandomSound(AudioClip[] sounds, float vol)
    {
        AudioClip sound = sounds[Random.Range(0, sounds.Length)];
        myAudioSource.PlayOneShot(sound, vol);
    }

    //plays a random sound from an array with vol and random pitch 
    public virtual void PlayRandomSoundRandomPitch(AudioClip[] sounds, float vol)
    {
        RandomizePitch(pitchRange.x, pitchRange.y);
        AudioClip sound = sounds[Random.Range(0, sounds.Length)];
        myAudioSource.PlayOneShot(sound, vol);
    }

    //randomizes npc audio source pitch based on range provided 
    public virtual void RandomizePitch(float min, float max)
    {
        float randomPitch = Random.Range(min, max);
        myAudioSource.pitch = randomPitch;
    }

    public int voiceCounter;
    public AudioSource[] voices;
    //called to play sounds 
    public void PlaySoundMultipleAudioSources(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);

        voiceCounter = CountUpArray(voiceCounter, voices.Length - 1);

        voices[voiceCounter].PlayOneShot(soundArray[randomSound]);
    }

    public int currentSound = 0;
    //counts up through sound array, in order 
    public void PlaySoundUp(AudioClip[] soundArray)
    {
        currentSound = CountUpArray(currentSound, soundArray.Length - 1);

        voices[voiceCounter].PlayOneShot(soundArray[currentSound]);
    }

    //for counting up arrays
    public int CountUpArray(int counter, int total)
    {
        if (counter < total)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }

        return counter;
    }
}

