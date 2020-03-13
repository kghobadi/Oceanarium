using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ControllerOrMouse : MonoBehaviour {
    Image image;
    SpriteRenderer sRenderer;
    public Sprite controller;
    public Sprite mKeyboard;

    void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
            sRenderer = GetComponent<SpriteRenderer>();
    }

    void Update ()
    {
        CheckInputType();
    }

    public void CheckInputType()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            if(image)
                image.sprite = controller;
            if (sRenderer)
                sRenderer.sprite = controller;
        }
        //mouse keyboard
        else
        {
            if (image)
                image.sprite = mKeyboard;
            if (sRenderer)
                sRenderer.sprite = mKeyboard;
        }
    }
}
