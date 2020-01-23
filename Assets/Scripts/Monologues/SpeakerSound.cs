using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerSound : AudioHandler
{
    //audio stuff
    [Header("Speaker Bools")]
    [Tooltip("enables voice audio")]
    public bool hasVoiceAudio;
    public bool usesAllLetters, countsUp;
    public int speakFreq = 4;
    
    //for random sound reading
    [Header("Speaker Sounds")]
    public AudioClip[] spokenSounds;
    //for spoken alphabet
    public List<char> letters = new List<char>();
    public List<char> capitalLetters = new List<char>();
    //for matching up letters with sounds 
    public List<AudioClip> spokenLetters = new List<AudioClip>();

    //checks what kind of audio to play for the speaker 
    public void AudioCheck(string lineOfText, int letter)
    {
        if (hasVoiceAudio)
        {
            if (!countsUp)
            {
                if (letter % speakFreq == 0)
                    Speak(lineOfText[letter]);
            }
            else
            {
                if (!voices[voiceCounter].isPlaying)
                {
                    PlaySoundUp(spokenSounds);
                }
            }
        }
    }

    //check through our alphabet of sounds and play corresponding character
    public void Speak(char letter)
    {
        //cycle through audioSources for voice
        voiceCounter = CountUpArray(voiceCounter, voices.Length - 1);

        if (usesAllLetters)
        {
            //check in letters
            if (letters.Contains(letter))
            {
                int index = letters.IndexOf(letter);
                voices[voiceCounter].clip = spokenSounds[index];
                voices[voiceCounter].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke");
            }
            //check in capital letters
            else if (capitalLetters.Contains(letter))
            {
                int index = capitalLetters.IndexOf(letter);
                voices[voiceCounter].clip = spokenSounds[index];
                voices[voiceCounter].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke capital");
            }
            //punctuation or other stuff?
            else
            {
                PlayRandomSound(spokenSounds, 1f);
                //Debug.Log("gibberish");
            }
        }
        //for characters who only use gibberish sounds
        else
        {
            PlayRandomSound(spokenSounds, 1f);
            Debug.Log("gibberish");
        }

    }

}
