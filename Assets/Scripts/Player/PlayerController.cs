using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Audio;
using InControl;

public class PlayerController : AudioHandler
{
    SaveSystem saveSystem;
    public FadeSprite[] controlsAtStart;

    //Current Planet
    [Header("Active Planet")]
    public PlanetManager activePlanet;
    public string activePlanetName;
    public CinemachineFreeLook currentCamera;

    //control bools
    [Header("Movement Bools")]
    public bool canMove = true;
    public bool canJump;

    [Header("Movement Speeds & Vars")]
    //general movement
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float forwardMovement;
    [HideInInspector] public float verticalMovement;
    Vector3 force;
    float camDist;
    [Tooltip("This just shows the current velocity of the player")]
    public float currentVelocity;
    [Tooltip("Speed player accelerates at in any WASD dir")]
    public float swimSpeed;
    [Tooltip("Max swim velocity player can reach")]
    public float maxSpeed = 10f;
    [Tooltip("Speed player travels while moving up & down")]
    public float elevateSpeed;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float repulsionForce = 200f, repulsionDistance = 5f;
    [Tooltip("Distance from current Planet at which player will be unable to elevate")]
    public float distMaxFromPlanet = 50f;
    [Tooltip("When this timer reaches Time Until Meditate while player is idle / inactive, will start meditating")]
    public float idleTimer = 0f, timeUntilMeditate = 10f;
    [Tooltip("Can't meditate until we reach quadsphere")]
    public bool canMeditate;
    public FadeSprite camFade;
    PostProcessingBehaviour camBehavior;
    public PostProcessingProfile normalPP, meditationPP;
    public GameObject meditationSoul;
    public UnityEvent startMeditation;
    public UnityEvent endMeditation;
    //player move states
    public MoveStates moveState;
    public enum MoveStates
    {
        SWIMMING, IDLE, MEDITATING, TALKING,
    }

    //for swim jumps 
    [Header("Swim Jump Variables")]
    public bool infiniteJump;
    public float jumpForce = 220;
    float jumpTimer;
    public float jumpMin, jumpMax, forceMultiplier;
    public float jumpGroundMinDistance = 20f;
    public LayerMask groundedMask, planetMask;
    public bool jumped;
    public float resetTimer = 0f, jumpResetTime = 2f;
    public int jumpFrameCounter = 0, totalJumpFrames = 12;
    public float jumpForcePerFrame, totalJumpForce;

    //all my body parts....
    InputDevice inputDevice;
    Transform cameraT;
    [HideInInspector]
    public GravityBody gravityBody;
    CapsuleCollider capCollider;
    GameObject playerSpriteObj;
    
    [HideInInspector]
    public CameraController camControls; //other things may need access to camera 
    [HideInInspector]
    public MeditationMovement meditationControls; // meditation camera controls 
    [HideInInspector]
    public Rigidbody playerRigidbody; // Public because of currents.
    [HideInInspector]
    public PlayerAnimations animator; // can trigger animations from elsewhere 
    //essence
    [HideInInspector]
    public EssenceInventory essenceInventory;

    [Header("Audio & Vis FX")]
    public AudioClip[] swimmingSounds;
    public AudioClip[] swimJumpSounds, outOfBreathSounds;
    public float swimStepTimer, swimStepFrequency = 1f;
    public GameObject bubbleParticles;
    //for meditating
    public AudioMixerSnapshot normal, meditating;
    QuitGame quitScript;
    public float restartTimer, restartTotal = 60f;

    #region Monobehaviour
    public override void Awake()
    {
        base.Awake();

        cameraT = Camera.main.transform;
        camControls = cameraT.GetComponent<CameraController>();
        camBehavior = cameraT.GetComponent<PostProcessingBehaviour>();
        meditationControls = cameraT.GetComponent<MeditationMovement>();  
        playerRigidbody = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        capCollider = GetComponent<CapsuleCollider>();
        playerSpriteObj = transform.GetChild(0).gameObject;
        animator = playerSpriteObj.GetComponent<PlayerAnimations>();
        quitScript = FindObjectOfType<QuitGame>();
        idleTimer = 0;

        //essence Inventory check
        if (essenceInventory == null)
            essenceInventory = gameObject.AddComponent<EssenceInventory>();

        //save stuff
        saveSystem = FindObjectOfType<SaveSystem>();
        saveSystem.returningGame.AddListener(LoadMeditation);

        //cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //called when player loads in rather than starting new game
    public void DeactivateControls()
    {
        for (int i = 0; i < controlsAtStart.Length; i++)
        {
            controlsAtStart[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        inputDevice = InputManager.ActiveDevice;

        if (canMove)
        {
            //check for sprinting input
            SwimInputs();

            //correlates jumping logic with animations
            JumpDetection();

            //repulses player from planet when they land too hard
            RepulsionLogic();

            if (canJump)
            {
                //called to handle jump inputs
                TakeJumpInput();
            }
        }

        //extra disables for meditation
        if(moveState == MoveStates.MEDITATING)
        {
            //all 3 controller buttons besides 'Talk' can disable meditation
            if(inputDevice.Action2.WasPressed ||inputDevice.Action4.WasPressed)
            {
                DisableMeditation();
            }
        }

        //reset jump 
        JumpReset();
    }

    void FixedUpdate()
    {
        //only apply swim force when not meditating 
        if(moveState != MoveStates.MEDITATING && moveState != MoveStates.TALKING)
        {
            ApplySwimForce();
        }
            
        // Limit velocity
        if (playerRigidbody.velocity.magnitude > maxSpeed)
        {
            playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxSpeed);
        }

        //jump forces 
        if (jumped)
        {
            //add jump force 
            if(jumpFrameCounter < totalJumpFrames)
            {
                playerRigidbody.AddForce(transform.forward * jumpForcePerFrame);
                jumpFrameCounter++;
            }
        }
    }
    #endregion

    #region Movement
    //controls swimming 
    void SwimInputs()
    {
        //create empty force vector for this frame 
        force = Vector3.zero;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            // 3 axes 
            //only swim when not meditating 
            if (moveState != MoveStates.MEDITATING)
            {
                horizontalMovement = inputDevice.LeftStickX;
                forwardMovement = inputDevice.LeftStickY;
            }

            //riight is up -- top priority
            if (inputDevice.RightTrigger.IsPressed)
                verticalMovement = inputDevice.RightTrigger;
            //left is down
            else if (inputDevice.LeftTrigger.IsPressed)
                verticalMovement = inputDevice.LeftTrigger * -1;
            //nothing -- zero
            else if(inputDevice.LeftTrigger.IsPressed == false 
                && inputDevice.RightTrigger.IsPressed == false)
                verticalMovement = 0;
        }
        //mouse & keyboard
        else
        {
            // 3 axes 
            //only swim when not meditating 
            if (moveState != MoveStates.MEDITATING)
            {
                horizontalMovement = Input.GetAxis("Horizontal");
                forwardMovement = Input.GetAxis("Vertical");
            }
            verticalMovement = Input.GetAxis("Elevation");
        }
            
        //dist from camera
        camDist = Vector3.Distance(playerSpriteObj.transform.position, cameraT.position);

        //FORWARD force checks
        if (forwardMovement > 0)
        {
            //add forward force 
            force += transform.forward * forwardMovement;
        }
        if (forwardMovement < 0)
        {
            //add backward force
            force += transform.forward * forwardMovement;
        }

        //HORIZONTAL force checks
        if (horizontalMovement > 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }
        if (horizontalMovement < 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }
        
        //Sound checks
        if (Mathf.Abs(horizontalMovement) > 0 || Mathf.Abs(forwardMovement) > 0 || Mathf.Abs(verticalMovement) > 0)
        {
            idleTimer = 0;
            swimStepTimer -= Time.deltaTime;
            if(swimStepTimer < 0)
            {
                PlayRandomSoundRandomPitch(swimmingSounds, 0.5f);
                swimStepTimer = swimStepFrequency;
            }

            //fade out title card when player moves
            if(controlsAtStart.Length > 0)
            {
                //fade out WASD
                controlsAtStart[0].FadeOut();
            }
        }
        else
        {
            swimStepTimer = 0;
        }


        //set animator floats for blend tree
        animator.characterAnimator.SetFloat("Move X", horizontalMovement);
        animator.characterAnimator.SetFloat("Move Z", forwardMovement);
        animator.characterAnimator.SetFloat("Move Y", verticalMovement);

        //Animator checks 
        //IDLE
        if (forwardMovement == 0 && horizontalMovement == 0 && verticalMovement == 0)
        {
            idleTimer += Time.deltaTime;
            //idle until reach meditation time
            if(idleTimer < timeUntilMeditate)
            {
                moveState = MoveStates.IDLE;
                //diver idle
                animator.SetAnimator("idle");
            }
            //starts meditating
            else
            {
                SetMeditation();
            }
        }
        //elevating
        else if (verticalMovement != 0)
        {
            DisableMeditation();

            moveState = MoveStates.SWIMMING;
            animator.SetAnimator("elevating");
        }
        //swimming 
        else if(verticalMovement == 0 && (forwardMovement != 0 || horizontalMovement != 0))
        {
            idleTimer = 0;
            moveState = MoveStates.SWIMMING;
            animator.SetAnimator("swimming");
        }
    }

    //apply force 
    void ApplySwimForce()
    {
        //add force only if you do not exceed max vel mag
        if (playerRigidbody.velocity.magnitude < maxSpeed)
        {
            //SWIM force
            playerRigidbody.AddForce(force * swimSpeed);

            //ELEVATION force 
            if (gravityBody.distanceFromPlanet < distMaxFromPlanet)
            {
                playerRigidbody.AddForce(transform.up * verticalMovement * elevateSpeed);
            }
            //set idle anim once too far from planet 
            else
            {
                animator.SetAnimator("idle");
            }
        }

        //set current vel
        currentVelocity = playerRigidbody.velocity.magnitude;
    }

    #endregion

    #region Meditation
    //called when you reach quadsphere
    public void EnableMeditationAbility()
    {
        canMeditate = true;
        PlayerPrefs.SetString("CanMeditate", "yes");
    }

    //called when the game loads 
    public void LoadMeditation()
    {
        if(PlayerPrefs.GetString("CanMeditate") == "yes")
        {
            EnableMeditationAbility();
        }
    }

    //begin meditating
    public void SetPearlMeditation()
    {
        //only if not already and controls from start are gone and not in pause menu
        if (quitScript.escMenu.activeSelf == false)
        {
            canMove = false;
            canJump = false;

            //diver meditates
            moveState = MoveStates.MEDITATING;
            animator.SetAnimator("meditating");
        }
    }

    //stop pearl meditating --unnecessary due to logic in MonologeManager
    public void DisablePearlMeditation()
    {
        //only if not already and controls from start are gone and not in pause menu
        if (moveState == MoveStates.MEDITATING)
        {
            canMove = true;
            canJump = true;

            //diver idle
            moveState = MoveStates.IDLE;
            animator.SetAnimator("idle");
        }
    }

    //begin meditating
    void SetMeditation()
    {
        //only if not already and controls from start are gone and not in pause menu
        if (moveState != MoveStates.MEDITATING 
            && quitScript.escMenu.activeSelf == false
            && canMeditate)
        {
            //lerp camera, enable rigidbody
            camControls.LerpFOV(camControls.meditationFOV, 2f);
            camControls.cRigidbody.isKinematic = true;
            canJump = false;

            //transition audio
            meditating.TransitionTo(2f);

            //set timer 
            if (idleTimer < timeUntilMeditate)
                idleTimer = timeUntilMeditate;

            //diver meditates
            moveState = MoveStates.MEDITATING;
            animator.SetAnimator("meditating");

            //enable peace sign above your body
            if (meditationSoul)
            {
                meditationSoul.SetActive(true);
            }

            //fade 
            if (camFade)
                camFade.FadeIn();

            //set pp 
            if(meditationPP)
                camBehavior.profile = meditationPP;

            //planet manager sets all pearl lures 
            activePlanet.SetMeditationVisuals();

            //fp
            camControls.canMoveCam = false;
            meditationControls.ActivateFPS();

            //call the event
            startMeditation.Invoke();
        }
    }

    //stop meditating 
    public void DisableMeditation()
    {
        //return from meditating FOV
        if (moveState == MoveStates.MEDITATING)
        {
            //lerp cam fov, disable rigidbody
            camControls.LerpFOV(camControls.originalFOV, 2f);
            camControls.cRigidbody.isKinematic = false;
            canJump = true;

            //transition audio
            normal.TransitionTo(2f);

            //planet manager resets all pearl lures 
            activePlanet.ResetVisuals();

            //fade 
            if(camFade)
                camFade.FadeOut();

            //disable peace sign
            if (meditationSoul)
            {
                meditationSoul.SetActive(false);
            }

            //set pp 
            if (normalPP)
                camBehavior.profile = normalPP;

            //fp
            camControls.canMoveCam = true;
            meditationControls.DeactivateFPS();

            //diver idle
            moveState = MoveStates.IDLE;
            animator.SetAnimator("idle");

            //call the event
            endMeditation.Invoke();
        }
    }

    #endregion

    #region Jumping/Diving
    void TakeJumpInput()
    {
        //get input device 
        inputDevice = InputManager.ActiveDevice;

        //start jumpTimer
        if ((Input.GetButton("Jump")|| inputDevice.Action1) && !jumped)
        {
            //set warm up animation for charged swim jump 
            if (jumpTimer > jumpMin)
            {
                //set charging anim
            }
            //as long as it is less than the max, it goes up
            if (jumpTimer < jumpMax)
            {
                jumpTimer += Time.deltaTime;
            }
            //hit jump max, so auto release
            else
            {
                Jump();
            }
        }

        //on button up
        if ((Input.GetButtonUp("Jump") || inputDevice.Action1.WasReleased) && !jumped)
        {
            Jump();
        }
    }

    //actual jump 
    void Jump()
    {
        //do nothing
        if (canJump == false)
            return;

        //just pressed, so normal jump
        if (jumpTimer <= jumpMin)
        {
            totalJumpForce = jumpForce;
        }

        //held, so we add to the force
        if (jumpTimer > jumpMin)
        {
            //multiply jump force * time 
            totalJumpForce = jumpForce + (jumpTimer * forceMultiplier);
        }

        //jump force per frame 
        jumpForcePerFrame = totalJumpForce / totalJumpFrames;

        //spawn bubble particles
        GameObject bubbles = Instantiate(bubbleParticles, transform.position, Quaternion.identity, transform);
        bubbles.transform.localEulerAngles = new Vector3(0, 180, 0);

        //animate and play sound, reset jump timer
        animator.characterAnimator.SetTrigger("jump");
        PlaySoundMultipleAudioSources(swimJumpSounds);

        //reset timers 
        jumpTimer = 0;
        idleTimer = 0;
        resetTimer = 0;
        jumpFrameCounter = 0;
        jumped = true;
    }

    //reset jumped timer 
    void JumpReset()
    {
        if (jumped)
        {
            resetTimer += Time.deltaTime;
            if(resetTimer > jumpResetTime)
            {
                jumped = false;
            }
        }
    }

    //checks if player is grounded to planet surface 
    void JumpDetection()
    {
        canJump = false;
        Ray ray = new Ray(transform.position, -gravityBody.GetUp());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, jumpGroundMinDistance, groundedMask))
        {
            canJump = true;
        }
    }

    void RepulsionLogic()
    {
        //shoot ray in direction of player's velocity
        Ray ray = new Ray(transform.position, -gravityBody.GetUp());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, repulsionDistance, groundedMask))
        {
            float proportionalHeightSquared = Mathf.Pow((repulsionDistance - hit.distance) / repulsionDistance, 2);
            playerRigidbody.AddForce(-playerRigidbody.velocity * proportionalHeightSquared * repulsionForce);
        }
    }

    #endregion 
}
