using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlanetManager))]
public class PlanetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get planet man script
        PlanetManager planetScript = (PlanetManager)target;

        //button for setting starting planet
        if (GUILayout.Button("Set Starting Planet"))
        {
            planetScript.SetAsStart();
        }

        base.OnInspectorGUI();
    }
}
