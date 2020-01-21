using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for game cameras

namespace Cameras
{
    public class GameCamera : MonoBehaviour
    {
        CameraManager manager;

        private void Awake()
        {
            manager = FindObjectOfType<CameraManager>();
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}

