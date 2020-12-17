using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : AudioHandler {
    SaveSystem saveSystem;
    PlayerController pc;
    Guardian guardianScript;
    PlanetManager planetMan;
    bool loaded;

    [Header("Ruins Settings")]
    public GameObject[] pillars;
    MeshRenderer[] pMeshes;
    public int activatedPillars = 0;
   
    [Tooltip("True if this Ruin activates a current inside, false if it is used for Guardian to take you to next Galaxy")]
    public bool activatesCurrent = true;
    public Currents currentToActivate;
    public Material silentMat, activeMat;
    public TimelinePlaybackManager cinematicManager;
    public AudioClip activationSound;

    [Header("Guardian Trip")]
    public Transform guardianPos;
    public ParticleSystem portalParticles;
    
	public override void Awake () 
    {
        base.Awake();

        saveSystem = FindObjectOfType<SaveSystem>();
        saveSystem.returningGame.AddListener(LoadGame);

        pc = FindObjectOfType<PlayerController>();
        guardianScript = FindObjectOfType<Guardian>();
        planetMan = GetComponentInParent<PlanetManager>();

        pMeshes = new MeshRenderer[pillars.Length];
		for(int i = 0; i < pillars.Length; i++)
        {
            pMeshes[i] = pillars[i].GetComponent<MeshRenderer>();
            pMeshes[i].material = silentMat;
        }
	}

    void LoadGame()
    {
        loaded = true;
    }
    
    public void ActivatePillar(GameObject pillar)
    {
        pillar.GetComponent<MeshRenderer>().material = activeMat;
        activatedPillars++;

        //check if all are activated
        if(activatedPillars >= pillars.Length)
        {
            AllActivated(true);

            //new game -- normal
            //if(!loaded)
            //    AllActivated(true);
            ////loaded game -- abnormal
            //else
            //{
            //    StartCoroutine(WaitToActivate(0.1f));
            //}
        }
    }

    //move guardian & play cinematic 
    public void AllActivated(bool fx)
    {
        //activates current
        if (activatesCurrent)
        {
            currentToActivate.ActivateCurrent();
        }
        //brings guardian here 
        else
        {
            guardianScript.transform.position = guardianPos.position;
            guardianScript.MoveToLocationAndWaitForTrip(guardianPos);
            if(portalParticles)
                portalParticles.Play();
        }

        if (fx)
        {
            //only if the player is on this planet...
            if (pc.activePlanet == planetMan)
            {
                cinematicManager.StartTimeline();
                PlaySound(activationSound, 1f);
            }
        }
        
    }

    IEnumerator WaitToActivate(float wait)
    {
        yield return new WaitForSeconds(wait);

        AllActivated(false);
    }
    
}
