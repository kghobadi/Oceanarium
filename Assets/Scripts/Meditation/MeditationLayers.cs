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
    int planarCullingMask;

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
        //set planar culling mask
        planarCullingMask = mainCam.cullingMask;
    }

    void Update()
    {
        if (ascending)
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
            Ascend();
        }
    }

    //called by meditationMovement
    void Ascend()
    {
        //beginning ascension?
        if (!ascending)
        {
            meditationStart = Time.time;
            ascending = true;
        }

        //enter next layer
        meditationLayer++;
        //set camera layers
        SetCameraMask(meditationLayer);
        //event call
        ascended.Invoke();

        Debug.Log("ascended to new reality layer: " + meditationLayer.ToString());
    }

    //when played ends meditation
    void Descend()
    {
        //return to planar realm
        meditationLayer = MeditationLayer.PLANAR;
        meditationTimer = 0;

        //reset camera layers
        SetCameraMask(meditationLayer);

        ascending = false;
    }

    void SetCameraMask(MeditationLayer layer)
    {
        //current mask
        var currentMask = mainCam.cullingMask;

        switch (layer)
        {
            //disable all meditation layers
            case MeditationLayer.PLANAR:
                //everything except UI, SANCTUM, GALACTIC, ABYSSAL
                mainCam.cullingMask = planarCullingMask;
                break;
            case MeditationLayer.SANCTUM:
                //everything except UI, GALACTIC, ABYSSAL
                mainCam.cullingMask = planarCullingMask | (1 << LayerMask.NameToLayer("SANCTUM"));
                break;
            case MeditationLayer.GALACTIC:
                //everything except UI, ABYSSAL
                mainCam.cullingMask = planarCullingMask | (1 << LayerMask.NameToLayer("SANCTUM"))
                    | (1 << LayerMask.NameToLayer("GALACTIC"));
                break;
            case MeditationLayer.ABYSSAL:
                //everything except UI
                mainCam.cullingMask = planarCullingMask | (1 << LayerMask.NameToLayer("SANCTUM"))
                 | (1 << LayerMask.NameToLayer("GALACTIC")) | (1 << LayerMask.NameToLayer("ABYSSAL"));
                break;
        }

        //example code
        //int layer1 = LayerMask.NameToLayer("MyLayer1");
        //int layer2 = LayerMask.NameToLayer("MyLayer2");
        //this means you can see bother layers 
        //mainCam.cullingMask = (1 << layer1) | (1 << layer2);
    }
}
