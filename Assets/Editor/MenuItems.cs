using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    private static void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Deleted all player prefs");
    }

    //see more here: https://learn.unity.com/tutorial/editor-scripting#5c7f8528edbc2a002053b5f9
}