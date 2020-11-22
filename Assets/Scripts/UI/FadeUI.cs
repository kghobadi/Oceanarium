using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//This script handles fades 
//can fadeIn at start or fadeOut when leaving

public class FadeUI : MonoBehaviour
{
    //set this in editor to decide which component to grab
    public enum UIType
    {
        IMAGE, RAWIMAGE, TEXT, TMPTEXT,
    }
    public UIType uiType;

    //store image/text + color
    Image thisImage;
    RawImage rawImage;
    Text thisText;
    TMP_Text tmpText;
    Color alphaValue;

    //these will be on during the fades
    public bool fadingIn, fadingOut, keepActive = true, fadeOutImmediately;
    public float fadeInAmount = 1f;
    public float fadeOutAmount = 0f;
    //controls the speed of the fade
    public float fadeInWait, fadeOutWait, fadeInSpeed = 0.75f, fadeOutSpeed = 1f;

    public bool shownAtStart;

    void GetUIType()
    {
        //checks privately whether this object has image or text component
        thisImage = GetComponent<Image>();
        if (thisImage == null)
        {
            rawImage = GetComponent<RawImage>();

            if (rawImage == null)
            {
                thisText = GetComponent<Text>();

                //it's a TMP TEXT
                if (thisText == null)
                {
                    tmpText = GetComponent<TMP_Text>();
                    uiType = UIType.TMPTEXT;
                }
                //its a Text
                else
                {
                    uiType = UIType.TEXT;
                }
            }
            else
            {
                uiType = UIType.RAWIMAGE;
            }
        }
        //its an image
        else
        {
            uiType = UIType.IMAGE;
        }
    }

    void Start()
    {
        GetUIType();

        SetAlpha();
        alphaValue.a = 0;
        UpdateAlpha();

        //automatically fadeIn at start if object has this script
        if (shownAtStart)
            StartCoroutine(WaitToFadeIn());
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        fadingIn = true;
        fadingOut = false;
    }
    public void FadeOut()
    {
        fadingIn = false;
        fadingOut = true;
    }

    void Update()
    {
        //when fadingIn, this is called every frame
        if (fadingIn)
        {
            if (alphaValue.a < fadeInAmount)
            {
                alphaValue.a += fadeInSpeed * Time.deltaTime;
                UpdateAlpha();
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
                UpdateAlpha();
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

    //switch statement sets alpha depending on component type 
    void SetAlpha()
    {
        switch (uiType)
        {
            case UIType.IMAGE:
                alphaValue = thisImage.color;
                break;
            case UIType.RAWIMAGE:
                alphaValue = rawImage.color;
                break;
            case UIType.TEXT:
                alphaValue = thisText.color;
                break;
            case UIType.TMPTEXT:
                alphaValue = tmpText.color;
                break;
        }
    }

    //switch statement updates alpha depending on component type 
    void UpdateAlpha()
    {
        switch (uiType)
        {
            case UIType.IMAGE:
                thisImage.color = alphaValue;
                break;
            case UIType.RAWIMAGE:
                rawImage.color = alphaValue;
                break;
            case UIType.TEXT:
                thisText.color = alphaValue;
                break;
            case UIType.TMPTEXT:
                tmpText.color = alphaValue;
                break;
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
