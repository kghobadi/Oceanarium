using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is used to trigger different fade behaviors 
public class AudioTrigger : MonoBehaviour {
    public MusicFader fadeOut;
    public MusicFader fadeIn;
    public float fadeOutSpeed, fadeInSpeed;
    public bool hasActivated;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
        {
            if (!hasActivated)
            {
                fadeOut.FadeOut(fadeOut.fadeOutAmount, fadeOutSpeed);
                fadeIn.SetSound(fadeIn.musicTrack);
                fadeIn.FadeIn(fadeIn.fadeInAmount, fadeInSpeed);
            }
        }
    }

}
