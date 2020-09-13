using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

//this script is for the first button press in the opening scene 
public class SpaceToStart : MonoBehaviour {

    LoadSceneAsync sceneLoader;

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
                if (inputDevice.Action1.WasPressed)
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

            if (Input.GetKeyUp(KeyCode.Space))
            {
                holdSpaceTimer = 0;
            }
        }
	}

    void StartFades()
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

        swirls.Stop();
        swirls.Clear();
        explosion.Play();

        guardianMonologue.EnableMonologue();
        
        hasStarted = true;
    }
}
