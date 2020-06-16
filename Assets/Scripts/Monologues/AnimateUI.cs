using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//animates a UI image compotent with timers 
public class AnimateUI : MonoBehaviour {
    Image thisImage;

    public Sprite[] animationSprites;
    int currentSprite;

    public bool active;
    public float frameTimer, frameTimerTotal;

    void Start()
    {
        thisImage = GetComponent<Image>();
        frameTimer = frameTimerTotal;
        currentSprite = 0;
    }

    void Update () {
        if (active && thisImage.enabled)
        {
            AnimateUi();
        }
        else
        {
            currentSprite = 0;
        }
    }

    void AnimateUi()
    {
        frameTimer -= Time.deltaTime;
        if (frameTimer < 0)
        {
            if (currentSprite < (animationSprites.Length - 1))
            {
                currentSprite++;
            }
            else
            {
                currentSprite = 0;
            }
            frameTimer = frameTimerTotal;
        }
        thisImage.sprite = animationSprites[currentSprite];
    }
}
