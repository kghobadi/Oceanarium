using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

    Class for managing all in-game cameras
        Ensure one camera used at a time
        Switch between camera views

 */
 namespace Cameras
{
    public class CameraManager : MonoBehaviour
    {
        GameCamera[] cameras;

        public GameCamera defaultCamera;
        public GameCamera previousCamera, currentCamera;

        private void Awake()
        {
            cameras = FindObjectsOfType<GameCamera>();
        }

        // Start is called before the first frame update
        void Start()
        {
            // Ensure default camera is active and all others are inactive on start
            foreach (GameCamera cam in cameras)
            {
                if (cam == defaultCamera) Enable(cam);
                else cam.gameObject.SetActive(false);
            }
        }

        public void Enable(GameCamera camera)
        {
            camera.gameObject.SetActive(true);
            currentCamera = camera;
        }

        public void Disable(GameCamera camera)
        {
            camera.gameObject.SetActive(false);
            currentCamera = null;
        }

        public void Set(GameCamera camera)
        {
            if (camera == null)
                return;

            if (currentCamera != null)
                Disable(currentCamera);

            Enable(camera);
        }

        public void Reset()
        {
            if (currentCamera != null && currentCamera != defaultCamera)
                Disable(currentCamera);

            Enable(defaultCamera);
        }
    }

}
