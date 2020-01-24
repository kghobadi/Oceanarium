using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is for the first button press in the opening scene 
public class SpaceToStart : MonoBehaviour {

    public FadeSprite [] spritesToFadeIn;
    public FadeSprite [] spritesToFadeOut;
    public FadeUItmp[] textToFadeOut;

    public MonologueText guardianMonologue;

    public ParticleSystem swirls;
    public ParticleSystem explosion;

    public bool hasStarted;
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted)
        {
            StartFades();
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
