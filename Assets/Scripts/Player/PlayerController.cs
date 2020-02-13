using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Audio;

public class PlayerController : AudioHandler
{
    public FadeSprite blackBack;
    public FadeSprite wasdControls;

    //Current Planet
    [Header("Active Planet")]
    public PlanetManager activePlanet;
    public string activePlanetName;

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
    public float currentSpeed;
    public float driftSpeed;
    public float swimSpeed;
    public float elevateSpeed;
    public float maxSpeed = 10f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float repulsionForce = 200f, repulsionDistance = 5f;
    public float distMaxFromPlanet = 50f;
    public float idleTimer = 0f, timeUntilMeditate = 10f;
    //player move states
    public MoveStates moveState;
    public enum MoveStates
    {
        SWIMMING, IDLE, MEDITATING,
    }

    //for swim jumps 
    [Header("Swim Jump Variables")]
    public bool infiniteJump;
    public float jumpForce = 220;
    float jumpTimer;
    public float jumpMin, jumpMax, forceMultiplier;
    public float jumpGroundMinDistance = 20f;
    public LayerMask groundedMask, planetMask;

    //all my body parts....
    Transform cameraT;
    GravityBody gravityBody;
    CapsuleCollider capCollider;
    GameObject playerSpriteObj;
    
    public CinemachineFreeLook diverFreeLook;
    [HideInInspector]
    public CameraController camControls; //other things may need access to camera 
    [HideInInspector]
    public Rigidbody playerRigidbody; // Public because of currents.
    [HideInInspector]
    public PlayerAnimations animator; // can trigger animations from elsewhere 

    [Header("Audio & Vis FX")]
    public AudioClip[] swimmingSounds;
    public AudioClip[] swimJumpSounds, outOfBreathSounds;
    public float swimStepTimer, swimStepFrequency = 1f;
    public GameObject bubbleParticles;
    //for meditating
    public AudioMixerSnapshot normal, meditating;
    QuitGame quitScript;
    public float restartTimer, restartTotal = 60f;
    
    public override void Awake()
    {
        base.Awake();

        cameraT = Camera.main.transform;
        camControls = cameraT.GetComponent<CameraController>();
        playerRigidbody = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        capCollider = GetComponent<CapsuleCollider>();
        playerSpriteObj = transform.GetChild(0).gameObject;
        animator = playerSpriteObj.GetComponent<PlayerAnimations>();
        quitScript = FindObjectOfType<QuitGame>();
        idleTimer = 0;
       
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove)
        {
            //check for sprinting input
            SwimInputs();

            //called while spacebar is pressed
            TakeJumpInput();

            //called when spacebar is released
            OnJumpRelease();

            //correlates jumping logic with animations
            JumpDetection();

            //repulses player from planet when they land too hard
            RepulsionLogic();

            //game will restart if meditate for a minute
            MeditativeRestart();
        }
        
        //esc to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        // Limit velocity
        if (playerRigidbody.velocity.magnitude > maxSpeed)
        {
            playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxSpeed);
        }
    }

    //controls swimming 
    void SwimInputs()
    {
        //create empty force vector for this frame 
        force = Vector3.zero;

        // 3 axes 
        horizontalMovement = Input.GetAxis("Horizontal");
        forwardMovement = Input.GetAxis("Vertical");
        verticalMovement = Input.GetAxis("Elevation");
        //dist from camera
        camDist = Vector3.Distance(playerSpriteObj.transform.position, cameraT.position);

        //VERTICAL force checks
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
            if(blackBack!= null)
            {
                if (blackBack.gameObject.activeSelf)
                    blackBack.FadeOut();
                if (wasdControls.gameObject.activeSelf)
                    wasdControls.FadeOut();
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
            if(idleTimer < timeUntilMeditate)
            {
                moveState = MoveStates.IDLE;
                //diver idle
                animator.SetAnimator("idle");
            }
            //starts meditating
            else
            {
                if (moveState != MoveStates.MEDITATING)
                {
                    camControls.LerpFOV(camControls.meditationFOV, 2f);
                    meditating.TransitionTo(2f);
                    canJump = false;
                }

                moveState = MoveStates.MEDITATING;
                //diver meditates
                animator.SetAnimator("meditating");
            }
        }
        //elevating
        else if (verticalMovement != 0)
        {
            //return from meditating FOV
            if(moveState == MoveStates.MEDITATING)
            {
                camControls.LerpFOV(camControls.originalFOV, 2f);
                normal.TransitionTo(2f);
                canJump = true;
            }

            moveState = MoveStates.SWIMMING;
            animator.SetAnimator("elevating");

        }
        //swimming 
        else if(verticalMovement == 0 && (forwardMovement != 0 || horizontalMovement != 0))
        {
            //return from meditating FOV
            if (moveState == MoveStates.MEDITATING)
            {
                camControls.LerpFOV(camControls.originalFOV, 2f);
                normal.TransitionTo(2f);
                canJump = true;
            }

            idleTimer = 0;
            moveState = MoveStates.SWIMMING;
            animator.SetAnimator("swimming");
        }

        //apply force 
       
        {
            //add force only if you do not exceed max vel mag
            if (playerRigidbody.velocity.magnitude < maxSpeed)
            {
                playerRigidbody.AddForce(force * swimSpeed);
            }

            //ELEVATION force 
            if (gravityBody.distanceFromPlanet < distMaxFromPlanet)
            {
                playerRigidbody.AddForce(transform.up * verticalMovement * elevateSpeed);
            }
            else
            {
                animator.SetAnimator("idle");
            }
        }

    }

    //after meditating long enough, game will restart 
    void MeditativeRestart()
    {
        if(moveState == MoveStates.MEDITATING)
        {
            restartTimer += Time.deltaTime;

            if(restartTimer > restartTotal)
            {
                quitScript.RestartGame();
            }
        }
        else
        {
            restartTimer = 0;
        }
    }
    
    void TakeJumpInput()
    {
        //start jumpTimer
        if (Input.GetButton("Jump") && (infiniteJump || canJump))
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
    }

    //release jump key, set swim jump 
    void OnJumpRelease()
    {
        //on button up
        if (Input.GetButtonUp("Jump") && (infiniteJump || canJump))
        {
            Jump();
        }
    }

    //actual jump 
    void Jump()
    {
        //just pressed, so normal jump
        if (jumpTimer <= jumpMin)
        {
            playerRigidbody.AddForce(force * jumpForce);
        }

        //held, so we add to the force
        if (jumpTimer > jumpMin)
        {
            //multiply jump force * time 
            float powerJumpForce = jumpForce + (jumpTimer * forceMultiplier);
            playerRigidbody.AddForce(transform.forward * powerJumpForce);

            //spawn bubble particles
            GameObject bubbles = Instantiate(bubbleParticles, transform.position, Quaternion.identity, transform);
            bubbles.transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        //animate and play sound, reset jump timer
        animator.characterAnimator.SetTrigger("jump");
        PlaySoundMultipleAudioSources(swimJumpSounds);
        jumpTimer = 0;
    }

    //checks if player is grounded to planet surface 
    void JumpDetection()
    {
        canJump = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, jumpGroundMinDistance, groundedMask))
        {
            canJump = true;
            if (hit.distance < hoverHeight)
            {
                float proportionalHeightSquared = Mathf.Pow((hoverHeight - hit.distance) / hoverHeight, 2);
                // Add more force the lower we are.
                playerRigidbody.AddForce(transform.up * proportionalHeightSquared * hoverForce);
            }
        }
    }

    void RepulsionLogic()
    {
        //shoot ray in direction of player's velocity
        Ray ray = new Ray(transform.position, -gravityBody.GetFutureUp());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, repulsionDistance, groundedMask))
        {
            float proportionalHeightSquared = Mathf.Pow((repulsionDistance - hit.distance) / repulsionDistance, 2);
            playerRigidbody.AddForce(-playerRigidbody.velocity * proportionalHeightSquared * repulsionForce);
        }
    }
}
