using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script allows you to offset the starting frame of an animation
public class OffsetAnimator : MonoBehaviour {

    Animator myAnimator;
    [Tooltip("Check this to randomize wait at start")]
    public bool randomOffset;
    public Vector2 waitRange = new Vector2(0.1f, 0.5f);
    float randomWait;

    void Awake () {
        myAnimator = GetComponent<Animator>();
	}

    void Start()
    {
        myAnimator.enabled = false;
    }

    private void OnEnable()
    {
        if (randomOffset)
            RandomOffset();
    }

    //offset by a random amount, with range designated in inspector or elsewhere 
    public void RandomOffset()
    {
        randomWait = Random.Range(waitRange.x, waitRange.y);
        StartCoroutine(WaitToStartAnimator(randomWait));
    }

    //offset by a specific amount 
    public void OffsetAnim(float offsetTime)
    {
        StartCoroutine(WaitToStartAnimator(offsetTime));
    }
    
    //actual wait to start coroutine
    IEnumerator WaitToStartAnimator(float time)
    {
        yield return new WaitForSeconds(time);
        myAnimator.enabled = true;
    }
}
