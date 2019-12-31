using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fades a sprite if its in front of the camera
public class FadeForCamera : MonoBehaviour {

    public bool fadingBack;
    
	void Update () {
        //lerps alpha back after Fade for obstruction
        if (fadingBack)
        {
            Color alphaVal = GetComponent<SpriteRenderer>().color;
            alphaVal.a = Mathf.Lerp(alphaVal.a, 1, Time.deltaTime);
            GetComponent<SpriteRenderer>().color = alphaVal;

            if (alphaVal.a > 0.99f)
            {
                fadingBack = false;
            }
        }
    }

    //called by camera on Objects which are stagnant in env and might block view of player
    public void Fade()
    {
        StopAllCoroutines();
        fadingBack = false;

        Color alphaVal = GetComponent<SpriteRenderer>().color;
        alphaVal.a = 0.2f;
        GetComponent<SpriteRenderer>().color = alphaVal;
        GetComponent<BoxCollider>().isTrigger = true;

        StartCoroutine(FadeOut());
    }

    //fades object back to normal alpha when no longer obstructing view of player
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);

        fadingBack = true;

        GetComponent<BoxCollider>().isTrigger = false;
    }
}
