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
    [HideInInspector]
    public DeactivateObject wm;

    public FadeStates fadeState;
    public enum FadeStates
    {
        FADINGIN, FADINGOUT, OPAQUE, TRANSPARENT,
    }

    [Tooltip("Keep gameObject active when fades out")]
    public bool keepActive;
    [Tooltip("Fade out as soon as object has fully faded in")]
    public bool fadeOutImmediately;

    //controls the speed of the fade
    public float fadeInWait, fadeOutWait, fadeInSpeed = 0.75f, fadeOutSpeed = 1f;

    public float fadeInAmount = 1f, fadeOutAmount = 0f;

    public bool fadeInAtStart;
    public bool returnsToPool;
    public bool worldManage;
    
    private void Awake()
    {
        //checks privately whether this object has image or text component
        thisSR = GetComponent<SpriteRenderer>();

        wm = GetComponent<DeactivateObject>();
    }

    void Start () {
        
        //differet syntax for image and text
        alphaValue = thisSR.color;

        //default fade state to transparent?
        fadeState = FadeStates.OPAQUE;
        
        //automatically fadeIn at start if object has this script
        if (fadeInAtStart)
        {
            //set alpha to 0
            alphaValue.a = 0;
            thisSR.color = alphaValue;

            //fade in!
            FadeIn();
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
            fadeState = FadeStates.FADINGIN;
        }
    }

    public void FadeOut()
    {
        fadeState = FadeStates.FADINGOUT;
    }
	
	void Update () {
        //when fadingIn, this is called every frame
        if (fadeState == FadeStates.FADINGIN)
        {
            if(alphaValue.a < fadeInAmount)
            {
                alphaValue.a += fadeInSpeed * Time.deltaTime;
                thisSR.color = alphaValue;
            }
            else
            {
                //set alpha val
                alphaValue.a = fadeInAmount;
                thisSR.color = alphaValue;
                //set fade state -- faded in
                fadeState = FadeStates.OPAQUE;

                if (fadeOutImmediately)
                {
                    StartCoroutine(WaitToFadeOut());
                }
            }
        }

        //when fading out, this is called every frame and eventually turns off object
        if (fadeState == FadeStates.FADINGOUT)
        {
            if (alphaValue.a > fadeOutAmount)
            {
                alphaValue.a -= fadeOutSpeed * Time.deltaTime;
                thisSR.color = alphaValue;
            }
            else
            {
                //set to transparent state -- faded out
                fadeState = FadeStates.TRANSPARENT;
                //deactivate
                if (!keepActive)
                {
                    gameObject.SetActive(false);
                }
                //deactivate with WM
                if (worldManage)
                {
                    wm.Deactivate();
                }
                //pooled object -- return to pool
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

        fadeState = FadeStates.FADINGIN;
    }

    public IEnumerator WaitToFadeOut()
    {
        yield return new WaitForSeconds(fadeOutWait);

        fadeState = FadeStates.FADINGOUT;
    }
}
