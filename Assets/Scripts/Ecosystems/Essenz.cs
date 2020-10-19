using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essenz : AudioHandler
{
    public AudioClip[] donationSounds;
    PlayerController pc;
    Orbit orbitScript;

    [Header("Essenz Settings")]
    public CollectibleStates cState;
    public enum CollectibleStates
    {
        AWAIT, VORTEX, ORBIT, DONATE,
    }
    Vector3 donationFinal;
    public float vortexSpeed = 100f;
    public float vortexMin = 75f, vortexMax = 100f;
    public float donationSpeed = 15f;
    public float minDist = 0.25f;
    
    public override void Awake()
    {
        base.Awake();

        pc = FindObjectOfType<PlayerController>();
        orbitScript = GetComponent<Orbit>();
    }

    void Start ()
    {
        //set speed 
        vortexSpeed = Random.Range(vortexMin, vortexMax);

        //set orbit
        orbitScript.orbiting = false;
        orbitScript.orbitalSpeed = vortexSpeed;
        orbitScript.planetToOrbit = pc.transform;

        //await collection
        cState = CollectibleStates.AWAIT;
	}

    void Update()
    {
        //orbiting player
        if (cState == CollectibleStates.VORTEX)
        {
            Vector3 pos = pc.transform.position;

            transform.position = Vector3.Lerp(transform.position, pos, donationSpeed * Time.deltaTime);

            //when reaches mouth, send it into orbit
            if (Vector3.Distance(transform.position, pos) < minDist)
            {
                SetOrbit(pos);
            }
        }

        //donating to some source
        if (cState == CollectibleStates.DONATE)
        {
            transform.position = Vector3.Lerp(transform.position, donationFinal, donationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, donationFinal) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" &&
            cState != CollectibleStates.VORTEX)
        {
            VortexToPlayer();
        }
    }

    //when player triggers collectible
    void VortexToPlayer()
    {
        //vortex towards player
        cState = CollectibleStates.VORTEX;
        //add to player list 
        pc.essenceInventory.collectedEssenz.Add(this);
    }

    //set orbit state
    void SetOrbit(Vector3 point)
    {
        cState = CollectibleStates.ORBIT;
        orbitScript.orbiting = true;
    }

    //called on donation decider
    public void DonateEssenz(Transform donationDest)
    {
        orbitScript.orbiting = false;

        donationFinal = donationDest.position;

        cState = CollectibleStates.DONATE;

        //remove from player list
        pc.essenceInventory.collectedEssenz.Remove(this);

        //play a sound
        PlayRandomSoundRandomPitch(donationSounds, 1f);
    }
}
