using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour {

    GameObject player;
    PlayerController pc;
    GameObject guardian;
    Guardian gBehavior;
    SaveSystem saveSystem;
    [HideInInspector] public MusicFader mFader;
    [HideInInspector] public Planter[] planterPearls;
    [HideInInspector] public GrowthPearl[] growthPearls;

    public string planetName;
    public bool playerHere, startingPlanet;
    [Tooltip("If starting planet checked, player will start at this Transform")]
    public Transform playerStartingPoint;
    [Tooltip("Location player will be teleported when using Galaxy map")]
    public Transform teleportationPoint;
    [Tooltip("This planet's colliders")]
    public Collider[] planetColliders;
    [HideInInspector] public CreatureSpawner creatureSpawner;
    [Tooltip("Any sort of creature that moves with code")]
    public List<GameObject> spriteCreatures = new List<GameObject>();
    [Tooltip("Any stagnant, animated object on the planet")]
    public List<GameObject> props = new List<GameObject>();
    public AudioClip musicTrack;
    [Tooltip("My selector on Galaxy Map")]
    public PlanetSelector pSelector;

    [Header("Guardian Behaviors")]
    public GuardianBehaviorSets[] guardianBehaviors;
    GuardianBehaviors gBehaviorsGroups; 

    [Header("Player Movement Settings")]
    public float newSwimSpeed = 25f;
    public float newMaxSpeed = 15f;
    [Tooltip("Use this if the player needs a new elevation speed (for changing height)")]
    public float newElevationSpeed = 50f;
    public float newMaxDistFromPlanet = 50f;
    public float newJumpForce = 1000f;

    void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        creatureSpawner = GetComponent<CreatureSpawner>();
        saveSystem = FindObjectOfType<SaveSystem>();
        saveSystem.startNewGame.AddListener(NewGame);
        guardian = GameObject.FindGameObjectWithTag("Guardian");
        if(guardian)
            gBehavior = guardian.GetComponent<Guardian>();
        mFader = FindObjectOfType<MusicFader>();
        planterPearls = GetComponentsInChildren<Planter>();
        growthPearls = GetComponentsInChildren<GrowthPearl>();
        gBehaviorsGroups = GetComponent<GuardianBehaviors>();
    }

    //called to change guardian behavior group on this planet
    public void ReassignGuardianBehaviors(int index)
    {
        if (gBehaviorsGroups)
        {
            //sets behavior to proper group
            guardianBehaviors = gBehaviorsGroups.gBehaviors[index].gBehaviorGroup;

            //reset guardian AI for this planet 
            Collider[] planet = GetComponents<Collider>();

            //only set guardian if not first planet
            gBehavior.ResetGuardianLocation(guardianBehaviors[0].guardianLocation.position, guardianBehaviors, planet);
            
        }
    }
    
    void Start()
    {
        DeactivatePlanet();
    }

    //invoked by Save system
    void NewGame()
    {
        if (startingPlanet)
        {
            StartPlayerAtPlanet();
        }
    }

    public void StartPlayerAtPlanet()
    {
        //null check
        if(pc == null)
            Awake();
        //teleport player 
        pc.transform.position = playerStartingPoint.position;
        //teleport guardian 
        if (guardian)
        {
            if (Vector3.Distance(pc.transform.position, gBehavior.transform.position) > 50f)
                gBehavior.TeleportGuardian(playerStartingPoint.position);
        }
        //activate planet 
        ActivatePlanet(guardianBehaviors[0].guardianLocation.position);
        //fade to music
        mFader.FadeTo(musicTrack);

        //we KNOW the player loaded rather than new game
        if (!startingPlanet)
        {
            pc.DeactivateControls();
        }
    }

    public void ActivatePlanet(Vector3 guardianPos)
    {
        //set player current planet 
        playerHere = true;
        pc.activePlanet = this;
        pc.activePlanetName = planetName;

        //reset guardian AI for this planet 
        Collider[] planet = GetComponents<Collider>();
        //only set guardian if not first planet
        if(gBehavior)
            gBehavior.ResetGuardianLocation(guardianPos, guardianBehaviors, planet);

        //update player's movement settings 
        pc.elevateSpeed = newElevationSpeed;
        pc.swimSpeed = newSwimSpeed;
        pc.maxSpeed = newMaxSpeed;
        pc.distMaxFromPlanet = newMaxDistFromPlanet;
        pc.jumpForce = newJumpForce;

        for (int i = 0; i < spriteCreatures.Count; i++)
        {
            spriteCreatures[i].SetActive(true);
        }
        for (int i = 0; i < props.Count; i++)
        {
            props[i].SetActive(true);
        }
    }

    public void DeactivatePlanet()
    {
        playerHere = false;
        for (int i = 0; i < spriteCreatures.Count; i++)
        {
            spriteCreatures[i].SetActive(false);
        }
        for (int i = 0; i < props.Count; i++)
        {
            props[i].SetActive(false);
        }
    }

    public void SetMeditationVisuals()
    {
        for(int i = 0; i < planterPearls.Length; i++)
        {
            planterPearls[i].SetMeditationLure();
        }
        for (int i = 0; i < growthPearls.Length; i++)
        {
            growthPearls[i].SetMeditationLure();
        }
    }

    public void ResetVisuals()
    {
        for (int i = 0; i < planterPearls.Length; i++)
        {
            planterPearls[i].ResetLure();
        }
        for (int i = 0; i < growthPearls.Length; i++)
        {
            growthPearls[i].ResetLure();
        }
    }
}
