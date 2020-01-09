using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruins : AudioHandler {

    [Header("Ruins Settings")]
    public GameObject[] pillars;
    public int activatedPillars = 0;
    public Guardian guardianScript;
    public Transform guardianPos;
    public ParticleSystem portalParticles;

    public Material silentMat, activeMat;
    public TimelinePlaybackManager cinematicManager;

    public AudioClip activationSound;

	public override void Awake () {
        base.Awake();

		for(int i = 0; i < pillars.Length; i++)
        {
            pillars[i].GetComponent<MeshRenderer>().material = silentMat;
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
        guardianScript.transform.position = guardianPos.position;
        guardianScript.MoveToLocationAndWait(guardianPos);
        portalParticles.Play();
        cinematicManager.PlayTimeline();
        PlaySound(activationSound, 1f);
    }
    
}
