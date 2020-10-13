using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// sets all world space canvas event cams to this cam
/// </summary>
public class SetEventCam : MonoBehaviour {
    Camera thisCam;
    Canvas[] canvases;

	void Awake ()
    {
        thisCam = GetComponent<Camera>();

        canvases = FindObjectsOfType<Canvas>();

        for(int i = 0; i < canvases.Length; i++)
        {
            canvases[i].worldCamera = thisCam;
        }
	}
}
