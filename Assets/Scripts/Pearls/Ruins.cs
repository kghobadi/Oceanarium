using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : AudioHandler {

    PlayerController pc;
    Guardian guardianScript;

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
    
	public override void Awake () {
        base.Awake();

        pc = FindObjectOfType<PlayerController>();
        guardianScript = FindObjectOfType<Guardian>();

        pMeshes = new MeshRenderer[pillars.Length];
		for(int i = 0; i < pillars.Length; i++)
        {
            pMeshes[i] = pillars[i].GetComponent<MeshRenderer>();
            pMeshes[i].material = silentMat;
        }
	}
    
    public void ActivatePillar(GameObject pillar)
    {
        pillar.GetComponent<MeshRenderer>().material = activeMat;
        activatedPillars++;

        //check if all are activated
        if(activatedPillars >= pillars.Length)
        {
            AllActivated();
        }
    }

    //move guardian & play cinematic 
    public void AllActivated()
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
            portalParticles.Play();
        }

        //play sound + cinematic only if player is not talking or meditating (camera issues)
        if(pc.moveState != PlayerController.MoveStates.TALKING && pc.moveState != PlayerController.MoveStates.MEDITATING)
        {
            cinematicManager.PlayTimeline();
        }
      
        PlaySound(activationSound, 1f);
    }
    
}
