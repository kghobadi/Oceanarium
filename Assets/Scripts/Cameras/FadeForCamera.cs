using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fades a sprite if its in front of the camera
[RequireComponent(typeof(FadeSprite))]
public class FadeForCamera : MonoBehaviour {
    
    FadeSprite fader;
    SpriteRenderer spriteRender;
    BoxCollider col;
    SphereCollider sCol;
    readonly int plantLayer = 22;
    bool worldManages;

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider>();
        sCol = GetComponent<SphereCollider>();
        if(col == null && sCol == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        fader = GetComponent<FadeSprite>();
        fader.keepActive = true;
        fader.fadeInSpeed = 0.25f;
        fader.fadeOutSpeed = 2f;
        
        //set this obj layer to plant
        gameObject.layer = plantLayer;
    }

    void Start()
    {
        worldManages = fader.worldManage;
    }

    //called by camera on Objects which are stagnant in env and might block view of player
    public void Fade(float amount)
    {
        //set fade out 
        fader.StopAllCoroutines();
        fader.fadeOutAmount = amount;
        fader.FadeOut();
        if (fader.worldManage)
        {
            fader.worldManage = false;
        }
       

        //set fade in
        fader.fadeInWait = 1f;
        fader.fadeInAmount = 1f;
        fader.StartCoroutine(fader.WaitToFadeIn());

        //set col
        StopAllCoroutines();
        if(col)
            col.isTrigger = true;
        if(sCol)
            sCol.isTrigger = true;
        StartCoroutine(ReturnCollider(1f));
    }

    //resets collider to active 
    IEnumerator ReturnCollider(float wait)
    {
        yield return new WaitForSeconds(wait);

        if (col)
            col.isTrigger = false;
        if (sCol)
            sCol.isTrigger = false;

        //reset world manager
        if(worldManages)
            fader.worldManage = true;
    }
}
