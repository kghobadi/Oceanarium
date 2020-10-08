using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fades a sprite if its in front of the camera
[RequireComponent(typeof(FadeSprite))]
public class FadeForCamera : MonoBehaviour {
    
    FadeSprite fader;
    SpriteRenderer spriteRender;
    readonly int plantLayer = 22;

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        fader = GetComponent<FadeSprite>();
        fader.keepActive = true;
        fader.fadeInSpeed = 0.25f;
        fader.fadeOutSpeed = 2f;
        //set this obj layer to plant
        gameObject.layer = plantLayer;
    }

    //called by camera on Objects which are stagnant in env and might block view of player
    public void Fade(float amount)
    {
        //set fade out 
        fader.StopAllCoroutines();
        fader.fadeOutAmount = amount;
        fader.FadeOut();

        //set fade in
        fader.fadeInWait = 1f;
        fader.fadeInAmount = 1f;
        fader.StartCoroutine(fader.WaitToFadeIn());
    }
}
