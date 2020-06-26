using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Monologues
{
    public Monologue monologue;
    public MonologueTrigger mTrigger; 
}

public class WorldMonologueManager : MonoBehaviour
{
    public Monologues[] allMonologues;
}
