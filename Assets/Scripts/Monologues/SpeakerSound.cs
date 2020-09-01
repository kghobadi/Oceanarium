using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerSound : AudioHandler
{
    //audio stuff
    [Header("Speaker Bools")]
    [Tooltip("enables voice audio")]
    public bool hasVoiceAudio;
    public int speakFreq = 4;
    public bool randomizePitch;
    
    //for random sound reading
    [Header("Speaker Sounds")]
    public AudioClip[] spokenSounds;

    //checks what kind of audio to play for the speaker 
    public void AudioCheck(string lineOfText, int letter)
    {
        if (hasVoiceAudio)
        {
            if (randomizePitch)
            {
                //increment voices 
                voiceCounter = CountUpArray(voiceCounter, voices.Length - 1);
                //play r sound r pitch from that source
                PlayRandomSoundRandomPitchSource(spokenSounds, 1f, voices[voiceCounter]);
            }
            else
            {
                PlaySoundUp(spokenSounds);
            }
        }
    }

  
}
