using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.UI;

//this script is for the first button press in the opening scene 
public class SpaceToStart : MonoBehaviour {

    LoadSceneAsync sceneLoader;

    public Image img;
    public FadeSprite [] spritesToFadeIn;
    public FadeSprite [] spritesToFadeOut;
    public FadeUItmp[] textToFadeOut;

    public MonologueText guardianMonologue;

    public ParticleSystem swirls;
    public ParticleSystem explosion;

    public bool hasStarted;
    float holdSpaceTimer, timeNec = 1f;

    private void Awake()
    {
        img = GetComponent<Image>();
        sceneLoader = FindObjectOfType<LoadSceneAsync>();
    }

    void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (!hasStarted)
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                //press X/[] to start
                if (inputDevice.Action3.WasPressed)
                {
                    StartFades();
                }
            }
            //mouse keyboard
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartFades();
                }
            }
        }
        else
        {
            //allows you to skip intro by holding space 
            if (Input.GetKey(KeyCode.Space))
            {
                holdSpaceTimer += Time.deltaTime;

                if (holdSpaceTimer > timeNec && sceneLoader.transition == false)
                {
                    sceneLoader.Transition(1f);
                }
            }

            //reset timer 
            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdSpaceTimer = 0;
            }
        }
	}

    public void StartFades()
    {
        //fade in 
        for(int i = 0; i < spritesToFadeIn.Length; i++)
        {
            spritesToFadeIn[i].FadeIn();
        }
        //fade out
        for (int i = 0; i < spritesToFadeOut.Length; i++)
        {
            spritesToFadeOut[i].FadeOut();
        }

        //fade out
        for (int i = 0; i < textToFadeOut.Length; i++)
        {
            textToFadeOut[i].FadeOut();
        }

        //set particles
        swirls.Stop();
        swirls.Clear();
        explosion.Play();

        guardianMonologue.EnableMonologue();
        
        hasStarted = true;
    }
}
