using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;

public class DialogueChoice : MonoBehaviour {
    Transform choiceHeader;

    [HideInInspector]
    public MonologueManager monoManager;

    public UnityEvent[] events;

    public bool hasActivated;
    public DialogueChoices dialogueChoice;
    public enum DialogueChoices
    {
        CHOICE1, CHOICE2, CHOICE3, CHOICE4,
    }

    private void Awake()
    {
        choiceHeader = transform.parent;
    }

    //on pc called by button class?
    public void ActivateChoice()
    {
        for(int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }

        hasActivated = true;
        choiceHeader.gameObject.SetActive(false);

        //disable cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {
        ControllerInputs();
    }

    //for controller support 
    void ControllerInputs()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //check its controller 
        if(inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            switch (dialogueChoice)
            {
                case DialogueChoices.CHOICE1:
                    if (inputDevice.Action1.WasPressed)
                    {
                        ActivateChoice();
                    }
                    break;
                case DialogueChoices.CHOICE2:
                    if (inputDevice.Action2.WasPressed)
                    {
                        ActivateChoice();
                    }
                    break;
                case DialogueChoices.CHOICE3:
                    if (inputDevice.Action3.WasPressed)
                    {
                        ActivateChoice();
                    }
                    break;
                case DialogueChoices.CHOICE4:
                    if (inputDevice.Action4.WasPressed)
                    {
                        ActivateChoice();
                    }
                    break;
            }
        }
        
    }
}
