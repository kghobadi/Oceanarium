using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerAnimations : AnimationHandler
{
    public int talkingAnimations;
    //select random talking anim to play
    public void RandomTalkingAnim()
    {
        int randomTalk = Random.Range(0, talkingAnimations);
        Animator.SetTrigger("talking" + randomTalk.ToString());
    }
}
