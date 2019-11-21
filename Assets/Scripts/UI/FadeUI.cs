using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script handles fades 
//can fadeIn at start or fadeOut when leaving

public class FadeUI : MonoBehaviour {
    //set this in editor to decide which component to grab
    public bool isImage;

    //store image/text + color
    Image thisImage;
    Text thisText;
    Color alphaValue;

    //these will be on during the fades
    public bool fadingIn, fadingOut, keepActive, fadeOutImmediately;

    //controls the speed of the fade
    public float fadeInWait, fadeOutWait, fadeInSpeed = 0.75f, fadeOutSpeed = 1f;

    public bool shownAtStart;

	void Start () {
        //checks privately whether this object has image or text component
        thisImage = GetComponent<Image>();
        thisText = GetComponent<Text>();

        if (thisImage != null)
            isImage = true;

        //differet syntax for image and text
        if (isImage)
        {
            alphaValue = thisImage.color;
            alphaValue.a = 0;
            thisImage.color = alphaValue;
        }
        else
        {
            alphaValue = thisText.color;
            alphaValue.a = 0;
            thisText.color = alphaValue;
        }

        //automatically fadeIn at start if object has this script
        if(shownAtStart)
            StartCoroutine(WaitToFadeIn());
	}

    public void FadeIn()
    {
        fadingIn = true;
        fadingOut = false;
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
            if(alphaValue.a < 1)
            {

                alphaValue.a += fadeInSpeed * Time.deltaTime;
                if(isImage)
                    thisImage.color = alphaValue;
                else
                    thisText.color = alphaValue;
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
            if (alphaValue.a > 0)
            {

                alphaValue.a -= fadeOutSpeed * Time.deltaTime;
                if (isImage)
                    thisImage.color = alphaValue;
                else
                    thisText.color = alphaValue;
            }
            else
            {
                fadingOut = false;
                if (!keepActive)
                {
                    gameObject.SetActive(false);
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
