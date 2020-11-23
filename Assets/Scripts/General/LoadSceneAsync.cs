//This script lets you load a Scene asynchronously.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InControl;

public class LoadSceneAsync : MonoBehaviour
{
    [Tooltip("The Trip Activation script of the Guardian")]
    public TripActivation tripToTransition;
    [Tooltip("Check this if you are certain the scene to be loaded is next in the build order")]
    public bool loadsNextScene;
    [Tooltip("Build index of the scene")]
    public int sceneToLoad;
    [Tooltip("Check this to start async load at start of the scene")]
    public bool loadPreparesOnStart;
    public bool preparing = false;
    public bool transition;
    public FadeUI fadeToBlack;

    MusicFader mFader;
    AsyncOperation asyncOperation = null;
    InputDevice inputDevice;

    void Start()
    {
        mFader = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicFader>();
        if (loadPreparesOnStart)
        {
            //Start loading the Scene asynchronously and output the progress bar
            StartCoroutine(LoadScene());
        }
      
    }

    public void Load()
    {
        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadScene());
    }
    

    IEnumerator LoadScene()
    {
        yield return null;
        asyncOperation = null;

        //load next scene in build order  
        if (loadsNextScene)
        {
            //Begin to load the next Scene
            asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        //load specified scene
        else
        {
            //Begin to load the Scene you specify
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        }

        preparing = true;
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                Debug.Log("ready to load " + sceneToLoad + "!");
                //Wait to you press the space key to activate the Scene && we are in the trip
                if (tripToTransition != null)
                {
                    inputDevice = InputManager.ActiveDevice;
                    //player pressed space during trip 
                    if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action3.WasPressed)
                        && tripToTransition.tripping)
                    {
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                    }
                    //trip timer reached trip length...
                    if(tripToTransition.tripping && transition)
                    {
                        //Activate the Scene
                        Transition(2f);
                    }
                }
                //no trip, just called from other script 
                else
                {
                    if (transition)
                    {
                        //Activate the Scene
                        asyncOperation.allowSceneActivation = true;
                    }
                     
                }
            }

            yield return null;
        }
    }

    public void Transition(float wait)
    {
        fadeToBlack.FadeIn();
        mFader.FadeOut(0, mFader.fadeSpeed);

        StartCoroutine(WaitToTransition(wait));
    }

    IEnumerator WaitToTransition(float time)
    {
        yield return new WaitForSeconds(time);
        
        if(!transition)
            transition = true;
        //Activate the Scene
        else
            asyncOperation.allowSceneActivation = true;
    }
}