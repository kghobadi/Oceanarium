using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script handles fades 
//can fadeIn at start or fadeOut when leaving

public class FadeSprite : MonoBehaviour {

    //store image/text + color
    SpriteRenderer thisSR;
    Color alphaValue;
    DeactivateObject wm;

    //these will be on during the fades
    public bool fadingIn, fadingOut, keepActive, fadeOutImmediately;

    //controls the speed of the fade
    public float fadeInWait, fadeOutWait, fadeInSpeed = 0.75f, fadeOutSpeed = 1f;

    public float fadeInAmount = 1f, fadeOutAmount = 0f;

    public bool fadeInAtStart;
    public bool returnsToPool;
    public bool worldManage;

	void Start () {
        //checks privately whether this object has image or text component
        thisSR = GetComponent<SpriteRenderer>();
        wm = GetComponent<DeactivateObject>();

        //differet syntax for image and text
        alphaValue = thisSR.color;
        
        //automatically fadeIn at start if object has this script
        if (fadeInAtStart)
        {
            alphaValue.a = 0;
            thisSR.color = alphaValue;
            StartCoroutine(WaitToFadeIn());
        }
	}

    public void FadeIn()
    {
        if (fadeInWait > 0)
        {
            StartCoroutine(WaitToFadeIn());
        }
        else
        {
            fadingIn = true;
            fadingOut = false;
        }
    }

    public void FadeOut()
    {
        fadingIn = false;
        fadingOut = true;
    }
	
	void Update () {
        //when fadingIn, this is called every frame
        if (fadingIn)
        {
            if(alphaValue.a < fadeInAmount)
            {
                alphaValue.a += fadeInSpeed * Time.deltaTime;
                thisSR.color = alphaValue;
            }
            else
            {
                fadingIn = false;
                if (fadeOutImmediately)
                {
                    StartCoroutine(WaitToFadeOut());
                }
            }
        }

        //when fading out, this is called every frame and eventually turns off object
        if (fadingOut)
        {
            if (alphaValue.a > fadeOutAmount)
            {
                alphaValue.a -= fadeOutSpeed * Time.deltaTime;
                thisSR.color = alphaValue;
            }
            else
            {
                fadingOut = false;
                if (!keepActive)
                {
                    gameObject.SetActive(false);
                }
                if (worldManage)
                {
                    wm.Deactivate();
                }
                if (returnsToPool)
                {
                    GetComponent<PooledObject>().ReturnToPool();
                }
            }
        }
    }

    public IEnumerator WaitToFadeIn()
    {
        yield return new WaitForSeconds(fadeInWait);
        
        fadingIn = true;
    }

    public IEnumerator WaitToFadeOut()
    {
        yield return new WaitForSeconds(fadeOutWait);
        
        fadingOut = true;
    }
}
