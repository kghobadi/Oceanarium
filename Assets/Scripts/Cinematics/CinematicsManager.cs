using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameras;

[System.Serializable]
public struct Cinematics
{
    public string cinematicName;
    public Cinematic cinematic;
    public TimelinePlaybackManager cPlaybackManager;
    public TimelineTriggerArea cTrigger;
}

public class CinematicsManager : MonoBehaviour
{
    public Cinematics[] allCinematics;
}
