using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFader : MonoBehaviour {
    AudioSource musicSource;
    public bool fadingVolOut, fadingVolIn;
    public AudioClip musicTrack;
    
    void Awake () {
        musicSource = GetComponent<AudioSource>();
    }
	
	void Update () {
        if (fadingVolOut)
        {
            musicSource.volume -= Time.deltaTime;

            if (musicSource.volume <= 0)
            {
                musicSource.Stop();
                musicSource.clip = musicTrack;
                musicSource.Play();

                fadingVolIn = true;
                fadingVolOut = false;
            }
        }

        if (fadingVolIn)
        {
            musicSource.volume += Time.deltaTime;

            if (musicSource.volume >= 1)
            {
                fadingVolIn = false;
            }
        }
    }

    public void FadeTo(AudioClip nextTrack)
    {
        musicTrack = nextTrack;
        fadingVolOut = true;
    }
}
