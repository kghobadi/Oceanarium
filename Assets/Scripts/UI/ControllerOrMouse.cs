using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;
using TMPro;

public class ControllerOrMouse : MonoBehaviour {
    Image image;
    SpriteRenderer sRenderer;
    Text text;
    TMP_Text textTMP;

    [Tooltip("This is set automatically on Awake")]
    public InfoType infoType;
    public enum InfoType
    {
        IMAGE, SPRITE, TEXT, TMPTEXT,
    }

    [Header("Image items")]
    public Sprite controller;
    public Sprite mKeyboard;

    [Header("Text items")]
    public string controllerText;
    public string mKeyboardText;

    InputDeviceClass lastClass;
    InputDevice inputDevice;

    void Awake()
    {
        SetInfoType();
    }

    //checks for all our possible UI info types and sets enum
    void SetInfoType()
    {
        image = GetComponent<Image>();

        if (image == null)
        {
            sRenderer = GetComponent<SpriteRenderer>();

            if(sRenderer == null)
            {
                text = GetComponent<Text>();

                if(text == null)
                {
                    textTMP = GetComponent<TMP_Text>();

                    infoType = InfoType.TMPTEXT;
                }
                else
                {
                    infoType = InfoType.TEXT;
                }
            }
            else
            {
                infoType = InfoType.SPRITE;
            }
        }
        else
        {
            infoType = InfoType.IMAGE;
        }

        //set last class to unknown at awake 
        lastClass = InputDeviceClass.Unknown;
    }

    void Update ()
    {
        CheckInputType();
    }

    //auto switches between controller & keyboard input datas
    public void CheckInputType()
    {
        //get input device 
        inputDevice = InputManager.ActiveDevice;

        //only run this code if last class is still unknown (from awake) 
        //or if current input device is dif from last class
        if(lastClass == InputDeviceClass.Unknown || inputDevice.DeviceClass != lastClass)
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                switch (infoType)
                {
                    case InfoType.IMAGE:
                        image.sprite = controller;
                        break;
                    case InfoType.SPRITE:
                        sRenderer.sprite = controller;
                        break;
                    case InfoType.TEXT:
                        text.text = controllerText;
                        break;
                    case InfoType.TMPTEXT:
                        textTMP.text = controllerText;
                        break;
                }
            }
            //mouse keyboard
            else
            {
                switch (infoType)
                {
                    case InfoType.IMAGE:
                        image.sprite = mKeyboard;
                        break;
                    case InfoType.SPRITE:
                        sRenderer.sprite = mKeyboard;
                        break;
                    case InfoType.TEXT:
                        text.text = mKeyboardText;
                        break;
                    case InfoType.TMPTEXT:
                        textTMP.text = mKeyboardText;
                        break;
                }
            }

            //save class to check against
            lastClass = inputDevice.DeviceClass;
        }
    }
}
