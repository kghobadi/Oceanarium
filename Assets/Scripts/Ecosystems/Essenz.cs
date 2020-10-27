using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic class for Essenz collectibles
/// This is an Energy Currency, used between species.
/// </summary>
public class Essenz : AudioHandler
{
    public AudioClip[] collectionSounds;
    public AudioClip[] donationSounds;
    PlayerController pc;
    Orbit orbitScript;
    FadeSprite fadeSprite;
    ParticleSystem particles;

    [Header("Essenz Settings")]
    public CollectibleStates cState;
    public enum CollectibleStates
    {
        AWAIT, VORTEX, ORBIT, FADE, DONATE,
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
        fadeSprite = GetComponent<FadeSprite>();
        if (fadeSprite == null)
            fadeSprite = GetComponentInChildren<FadeSprite>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void Start ()
    {
        //set speed 
        vortexSpeed = Random.Range(vortexMin, vortexMax);

        //set orbit
        if (orbitScript)
        {
            orbitScript.orbiting = false;
            orbitScript.orbitalSpeed = vortexSpeed;
            orbitScript.planetToOrbit = pc.transform;
        }

        //keep that active
        if (fadeSprite)
        {
            fadeSprite.keepActive = true;
        }

        //set particles
        if (particles)
        {
            particles.Play();
        }

        //await collection
        cState = CollectibleStates.AWAIT;
	}

    void Update()
    {
        //orbiting player
        if (cState == CollectibleStates.VORTEX)
        {
            Vector3 pos = pc.transform.position;

            transform.position = Vector3.MoveTowards(transform.position, pos, donationSpeed * Time.deltaTime);

            //when reaches mouth, send it into orbit
            if (Vector3.Distance(transform.position, pos) < minDist)
            {
                ConsumeCollectible();
            }
        }

        //donating to some source
        if (cState == CollectibleStates.DONATE)
        {
            transform.position = Vector3.MoveTowards(transform.position, donationFinal, donationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, donationFinal) < 1f)
            {
                ExpendCollectible();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" &&
            cState == CollectibleStates.AWAIT)
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
        Debug.Log("Vortexing to player");
    }

    /// <summary>
    /// Add to player inventory; set orbit, or fade;
    /// </summary>
    void ConsumeCollectible()
    {
        if (orbitScript)
            SetOrbit(pc.transform.position);
        if (fadeSprite)
        {
            //parent to player
            transform.SetParent(pc.transform);
            //fade
            SetFade(true);
        }
        if (particles)
            particles.Stop();

        //add to player list
        pc.essenceInventory.collectedEssenz.Add(this);

        //consume sound
        PlayRandomSoundRandomPitch(collectionSounds, 1f);
    }

    //set orbit state
    void SetOrbit(Vector3 point)
    {
        cState = CollectibleStates.ORBIT;
        orbitScript.orbiting = true;
    }

    //fade sprite
    void SetFade(bool fadeOut)
    {
        if (fadeOut)
            fadeSprite.FadeOut();
        else
        {
            fadeSprite.FadeIn();
        }

        cState = CollectibleStates.FADE;
    }

    /// <summary>
    /// Can be called by other objects or characters to absorb some of the Player's energy.
    /// </summary>
    /// <param name="donationDest"></param>
    public void DonateEssenz(Transform donationDest)
    {
        orbitScript.orbiting = false;

        donationFinal = donationDest.position;

        //fade in
        if (cState == CollectibleStates.FADE)
            SetFade(false);

        //play particles again
        if (particles)
            particles.Play();

        //set state
        cState = CollectibleStates.DONATE;

        //remove from player list
        pc.essenceInventory.collectedEssenz.Remove(this);

        //play a sound
        PlayRandomSoundRandomPitch(donationSounds, 1f);
    }

    /// <summary>
    /// the final use case of the collected
    /// </summary>
    void ExpendCollectible()
    {
        if (fadeSprite)
        {
            fadeSprite.keepActive = false;
            SetFade(true);
            transform.SetParent(null);
        }
        else
            Destroy(gameObject);
    }
}
