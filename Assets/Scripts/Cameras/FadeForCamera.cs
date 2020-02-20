using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Fades a sprite if its in front of the camera
public class FadeForCamera : MonoBehaviour {
    
    public bool fadingBack;

    SpriteRenderer spriteRender;
    BoxCollider bCollider;
    SphereCollider sphereCol;
    public bool boxOrSphere;

    void Awake()
    {
        GetRefs();
    }

    void GetRefs()
    {
        spriteRender = GetComponent<SpriteRenderer>();

        //get collider on this obj
        bCollider = GetComponent<BoxCollider>();
        if (bCollider != null)
            boxOrSphere = true;
        else
        {
            sphereCol = GetComponent<SphereCollider>();
            boxOrSphere = false;
        }
    }

    void Update () {
        //lerps alpha back after Fade for obstruction
        if (fadingBack)
        {
            Color alphaVal = spriteRender.color;
            alphaVal.a = Mathf.Lerp(alphaVal.a, 1, Time.deltaTime);
            spriteRender.color = alphaVal;

            if (alphaVal.a > 0.99f)
            {
                fadingBack = false;

                alphaVal = spriteRender.color;
                alphaVal.a = 1f;
                spriteRender.color = alphaVal;

                SetTrigger(false);
            }
        }
    }

    //called by camera on Objects which are stagnant in env and might block view of player
    public void Fade(float amount)
    {
        StopAllCoroutines();
        fadingBack = false;

        //null check 
        if (spriteRender == null)
            GetRefs();
        
        Color alphaVal = spriteRender.color;
        alphaVal.a = amount;
        spriteRender.color = alphaVal;

        SetTrigger(true);

        StartCoroutine(FadeOut());
    }

    //sets collider to trigger or not
    void SetTrigger(bool trueOrFalse)
    {
        if (boxOrSphere)
            bCollider.isTrigger = trueOrFalse;
        else
            sphereCol.isTrigger = trueOrFalse;
    }

    //fades object back to normal alpha when no longer obstructing view of player
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);

        fadingBack = true;
        
    }

    //fades object back to normal alpha when no longer obstructing view of player
    IEnumerator ResetTrigger(bool trueOrFalse)
    {
        yield return new WaitForSeconds(1);

        SetTrigger(trueOrFalse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Fade(0.95f);
        }
    }
}
