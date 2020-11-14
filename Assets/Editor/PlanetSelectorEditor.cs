using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetSelector))]
public class PlanetSelectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Get planet man script
        PlanetSelector planetScript = (PlanetSelector)target;

        //button for unlocking planet
        if (GUILayout.Button("Unlock Planet"))
        {
            planetScript.UnlockPlanet();
        }
    }
}
