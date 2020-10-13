using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "GuardianBehavior", menuName = "ScriptableObjects/GuardianScriptable", order = 1)]
public class GuardianBehaviorScriptable : ScriptableObject
{
    [Tooltip("Point for the guardian to travel to")]
    public Transform guardianLocation;
    [Tooltip("Check if guardian will deliver monologue at above point")]
    public bool hasMonologue;
    [Tooltip("Index of monologue within Guardian Mono Manager")]
    public int monologueIndex;
}
