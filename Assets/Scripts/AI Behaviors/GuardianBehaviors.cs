using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct GuardianBehaviorSets
{
    [Tooltip("Point for the guardian to travel to")]
    public Transform guardianLocation;
    [Tooltip("Check if guardian will deliver monologue at above point")]
    public bool hasMonologue;
    [Tooltip("Index of monologue within Guardian Mono Manager")]
    public int monologueIndex;
}

[Serializable]
public struct GuardianBehaviorGroup
{
    [Tooltip("An array of Guardian Behavior Sets")]
    public GuardianBehaviorSets[] gBehaviorGroup;
}

public class GuardianBehaviors : MonoBehaviour {

    public GuardianBehaviorGroup[] gBehaviors;
}
