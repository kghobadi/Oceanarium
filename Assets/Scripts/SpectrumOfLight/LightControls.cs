using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControls : MonoBehaviour {

    GameObject planet;
    public float lightSpeed;

    public GameObject[] photons;

    public AudioSource[] photonAudios;
    public AudioClip[] photonSounds;

    //fov stuff
    public Camera myCam;
    public bool lerpingFOV, increasingFOV;
    float originalFOV, desiredFOV, lerpSpeed;
    public float fovPerPhoton = 5;
    public int dancerCount;

    void Start()
    {
        myCam = Camera.main;
        originalFOV = myCam.fieldOfView;
        planet = GameObject.FindGameObjectWithTag("Planet");
        //photons = GameObject.FindGameObjectsWithTag("Photon");
    }

    void Update()
    {
        transform.LookAt(planet.transform.position);

        transform.position = Vector3.MoveTowards(transform.position, planet.transform.position, lightSpeed * Time.deltaTime);

        //is the player pressing stuff?
        InputCalls();

        //are we lerping??
        CheckLerping();
    }

    void CheckLerping()
    {
        //lerps fov 
        if (lerpingFOV)
        {
            myCam.fieldOfView = Mathf.Lerp(myCam.fieldOfView, desiredFOV, Time.deltaTime * lerpSpeed);

            //when to stop depends on if increasing or decreasing
            if (increasingFOV)
            {
                if (myCam.fieldOfView > desiredFOV - 0.1f)
                {
                    lerpingFOV = false;
                }
            }
            else
            {
                if (myCam.fieldOfView < desiredFOV + 0.1f)
                {
                    lerpingFOV = false;
                }
            }

        }
    }

    //called to set cam to specific new fov
    public void LerpFOV(float fov, float speed)
    {
        desiredFOV = fov;
        if (desiredFOV > myCam.fieldOfView)
        {
            increasingFOV = true;
        }
        else
        {
            increasingFOV = false;
        }
        lerpSpeed = speed;
        lerpingFOV = true;
    }

    void InputCalls()
    {
        //all photons Dance
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < photons.Length; i++)
            {
                Dance(i);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            for (int i = 0; i < photons.Length; i++)
            {
                StopDancing(i);
            }
        }

        //red
        if (Input.GetKey(KeyCode.S))
        {
            Dance(0);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StopDancing(0);
        }

        //orange
        if (Input.GetKey(KeyCode.D))
        {
            Dance(1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StopDancing(1);
        }

        //yellow
        if (Input.GetKey(KeyCode.F))
        {
            Dance(2);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            StopDancing(2);
        }

        //green
        if (Input.GetKey(KeyCode.J))
        {
            Dance(3);
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            StopDancing(3);
        }

        //blue
        if (Input.GetKey(KeyCode.K))
        {
            Dance(4);
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            StopDancing(4);
        }

        //purple
        if (Input.GetKey(KeyCode.L))
        {
            Dance(5);
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            StopDancing(5);
        }

        //if we have more or less dancers than before, calc new fov
        if(CheckDancers() != dancerCount)
        {
            dancerCount = CheckDancers();

            float calcFOV = originalFOV + (dancerCount * fovPerPhoton);

            LerpFOV(calcFOV, 15);
        }
    }
    
    //photon animates in a cyclical looping pattern and emits sounds and particles 
    //we also want it to move a bit ahead of the others, as if faster
    //lerp FOV by interval
    //for each key held down we want to lerp FOV a bit more, move a bit faster
    void Dance(int photon)
    {
        photons[photon].GetComponent<Photon>().dancing = true;
        if(!photonAudios[photon].isPlaying)
            photonAudios[photon].Play();
    }

    void StopDancing(int photon)
    {
        photons[photon].GetComponent<Photon>().dancing = false;
        if(photonAudios[photon].isPlaying)
            photonAudios[photon].Stop();
    }

    //how many photons are currently Dancing??
    int CheckDancers()
    {
        int tempCount = 0;
        for (int i = 0; i < photons.Length; i++)
        {
            if (photons[i].GetComponent<Photon>().dancing)
            {
                tempCount++;
            }
        }

        return tempCount;
    }
}
