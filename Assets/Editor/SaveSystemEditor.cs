using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveSystem))]
public class SaveSystemEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //Get save script
        SaveSystem saveScript = (SaveSystem)target;

        //button for resetting player prefs
        if(GUILayout.Button("Delete Prefs"))
        {
            saveScript.ResetPlayerPrefs();
        }
    }

    //this function is the same as calling  base.OnInspectorGUI();
    //DrawDefaultInspector();

    //Int example
    //saveScript.sessions = EditorGUILayout.IntField("Session" , saveScript.sessions);
    //String example
    //EditorGUILayout.LabelField("Active Planet", saveScript.lastPlanet.planetName);

    //see more here: https://learn.unity.com/tutorial/editor-scripting#5c7f8528edbc2a002053b5f9
}
