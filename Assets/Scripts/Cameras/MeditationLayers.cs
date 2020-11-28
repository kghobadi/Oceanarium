using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is for controlling the plane of reality.
/// We express this thru having a layer that becomes visible to the camera once the player meditates long enough.
/// </summary>
public class MeditationLayers : MonoBehaviour
{
    PlayerController meditater;
    Camera mainCam;

    public MeditationLayer meditationLayer;
    public enum MeditationLayer
    {
        PLANAR = 0,
        SANCTUM = 1,
        GALACTIC = 2,
        ABYSSAL = 3,
    }

    public bool ascending;
    public float meditationTimer = 0f;
    float meditationStart;
    public float meditationIncrements = 30f;
    public UnityEvent ascended;

    private void Awake()
    {
        //comp refs
        meditater = FindObjectOfType<PlayerController>();
        mainCam = Camera.main;
        //add ascend as listener
        meditater.startMeditation.AddListener(Ascend);
        //add descend as listener
        meditater.endMeditation.AddListener(Descend);
    }

    private void Start()
    {
        //start layer at 0
        meditationLayer = MeditationLayer.PLANAR;
    }

    void Update()
    {
        if(ascending)
            Ascendancy();
    }

    void Ascendancy()
    {
        //time meditation
        meditationTimer = Time.time - meditationStart;

        //if timer is greater than next increment and we have not reached final layer
        if (meditationTimer > meditationIncrements * (int)meditationLayer
            && meditationLayer < MeditationLayer.ABYSSAL)
        {
            NextLayer();
        }
    }

    void NextLayer()
    {
        //ascend
        meditationLayer++;
        ascended.Invoke();

        Debug.Log("new reality layer: " + meditationLayer.ToString());
    }

    //called by meditationMovement
    void Ascend()
    {
        //enter sanctum
        meditationLayer = MeditationLayer.SANCTUM;
        meditationStart = Time.time;

        ascending = true;
    }

    //when played ends meditation
    void Descend()
    {
        //return to planar realm
        meditationLayer = MeditationLayer.PLANAR;
        meditationTimer = 0;

        //reset camera layers

        ascending = false;
    }
       

}
