using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;

//script allows you to press Space to trigger events 
//it also auto configures for controller input (action 1)
public class SpaceTo : MonoBehaviour {
    public UnityEvent[] events;
    public bool activated;
    
	void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;
        
        if (!activated)
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                if (inputDevice.Action1.WasPressed)
                {
                    Activate();
                }
            }
            //mouse keyboard
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Activate();
                }
            }
        }
	}

    public void Activate()
    {
        for (int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }
    }
}
