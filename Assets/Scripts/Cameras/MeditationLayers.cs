using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is for any sprite object that exists on a separate plane of reality.
/// We express this thru having a layer that becomes visible once the player meditates long enough.
/// Can put this on objects with Sprite Renderers, FadeSprites, and as Parent of 3D objs.
/// </summary>
public class MeditationLayers : MonoBehaviour
{
    MeditationMovement meditation;
    public MeditationMovement.MeditationLayers meditationLayer;
    public bool isActive;
    SpriteRenderer sR;
    FadeSprite fader;

    private void Awake()
    {
        //comp refs
        meditation = FindObjectOfType<MeditationMovement>();
        sR = GetComponent<SpriteRenderer>();
        fader = GetComponent<FadeSprite>();
        if(fader)
            fader.keepActive = true;
        //add ascend as listener
        meditation.enteredNewLayer.AddListener(Ascend);
        //add descend as listener
        meditation.endedMeditation.AddListener(Descend);
    }

    //called by meditationMovement
    void Ascend()
    {
        //meets my layer :) && inactive
        if(meditation.meditationLayer >= meditationLayer && isActive == false)
        {
            //fades
            if (fader)
                fader.FadeIn();
            else
            {
                //sprite
                if(sR)
                    sR.enabled = true;
                //3d object or sprite is in children
                else
                {
                    for(int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            isActive = true;
        }
    }

    //when played ends meditation
    void Descend()
    {
        if (isActive)
        { 
            //fades
            if (fader)
                fader.FadeOut();
            else
            {
                //sprite
                if (sR)
                    sR.enabled = false;
                //3d object or sprite is in children
                else
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            isActive = false;
        }
    }
       

}
