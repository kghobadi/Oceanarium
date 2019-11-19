using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : AudioHandler
{
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
    float horizontalMovement;
    float verticalMovement;
    public float currentSpeed;
    public float driftSpeed;
    public float swimSpeed;
    public float elevateSpeed;
    public float maxSpeed = 10f;
    public float hoverForce = 65f;
    public float hoverHeight = 3.5f;
    public float repulsionForce = 200f, repulsionDistance = 5f;
    //player move states
    public MoveStates moveState;
    public enum MoveStates
    {
        UP, DOWN, LEFT, RIGHT, UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT, IDLE,
    }

    //for swim jumps 
    [Header("Swim Jump Variables")]
    public bool infiniteJump;
    public float jumpForce = 220;
    float jumpTimer;
    public float jumpMin, jumpMax, forceMultiplier;
    public float jumpGroundMinDistance = 20f;
    public LayerMask groundedMask, planetMask, spriteFadeMask;

    //all my body parts....
    Transform cameraT;
    GravityBody gravityBody;
    CapsuleCollider capCollider;
    GameObject playerSpriteObj;
    [HideInInspector]
    public CameraController camControls; //other things may need access to camera 
    [HideInInspector]
    public Rigidbody playerRigidbody; // Public because of currents.
    [HideInInspector]
    public PlayerAnimations animator; // can trigger animations from elsewhere 

    [Header("Audio & Vis FX")]
    public AudioClip[] swimmingSounds;
        public AudioClip[] eatingSounds, outOfBreathSounds;
    public GameObject bubbleParticles;
    
    public override void Awake()
    {
        base.Awake();

        cameraT = Camera.main.transform;
        camControls = cameraT.GetComponent<CameraController>();
        playerRigidbody = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        capCollider = GetComponent<CapsuleCollider>();
        playerSpriteObj = transform.GetChild(1).gameObject;
        animator = playerSpriteObj.GetComponent<PlayerAnimations>();
       
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove)
        {
            //for moving up and down
            ChangeElevation();

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
        }
        
        //esc to quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //del to restart
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            SceneManager.LoadScene(0);
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
        Vector3 force = Vector3.zero;

        // 2 axes 
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        //VERTICAL force checks
        if(verticalMovement > 0)
        {
            //add forward force 
            force += transform.forward * verticalMovement;
        }
        if(verticalMovement < 0)
        {
            //add backward force
            force += transform.forward * verticalMovement;
        }

        //HORIZONTAL force checks
        if(horizontalMovement > 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }
        if(horizontalMovement < 0)
        {
            //add neg x force 
            force += transform.right * horizontalMovement;
        }

        //Animator checks 
        //IDLE
        if(verticalMovement == 0 && horizontalMovement == 0)
        {
            moveState = MoveStates.IDLE;
            //diver look forward right
            animator.SetAnimator("idle");
        }
        //FORWARD
        if (verticalMovement > 0 && horizontalMovement == 0)
        {
            moveState = MoveStates.UP;
            //diver look forward
            animator.SetAnimator("forward");
        }
        //BACKWARD
        if (verticalMovement < 0 && horizontalMovement == 0)
        {
            moveState = MoveStates.DOWN;
            //diver look down
            animator.SetAnimator("down");
        }
        //RIGHT
        if (verticalMovement == 0 && horizontalMovement > 0)
        {
            moveState = MoveStates.RIGHT;
            //diver look right
            animator.SetAnimator("right");
        }
        //LEFT
        if (verticalMovement == 0 && horizontalMovement < 0)
        {
            moveState = MoveStates.DOWN;
            //diver look left
            animator.SetAnimator("left");
        }
        //FORWARD RIGHT
        if (verticalMovement > 0 && horizontalMovement > 0)
        {
            moveState = MoveStates.UPRIGHT;
            //diver look forward right
            animator.SetAnimator("forwardRight");
        }
        //FORWARD LEFT
        if (verticalMovement > 0 && horizontalMovement < 0)
        {
            moveState = MoveStates.UPLEFT;
            //diver look left
            animator.SetAnimator("forwardLeft");
        }
        //DOWN RIGHT
        if (verticalMovement < 0 && horizontalMovement > 0)
        {
            moveState = MoveStates.DOWNRIGHT;
            //diver look right
            animator.SetAnimator("downRight");
        }
        //DOWN LEFT
        if (verticalMovement < 0 && horizontalMovement < 0)
        {
            moveState = MoveStates.DOWNLEFT;
            //diver look left
            animator.SetAnimator("downLeft");
        }
        
        //apply force 
        {
            //add twice the force when you are slow
            if (playerRigidbody.velocity.magnitude < (maxSpeed / 2))
            {
                playerRigidbody.AddForce(force * swimSpeed * 2);
            }
            //only add force if velocity is less than max move speed 
            if (playerRigidbody.velocity.magnitude > (maxSpeed / 2) && playerRigidbody.velocity.magnitude < maxSpeed)
            {
                playerRigidbody.AddForce(force * swimSpeed);
            }
        }

    }

    void ChangeElevation()
    {
        //left click to move up
        if (Input.GetMouseButton(0))
        {
            playerRigidbody.AddForce(transform.up * elevateSpeed);
        }
        //right click to move down
        if (Input.GetMouseButton(1))
        {
            playerRigidbody.AddForce(-transform.up * elevateSpeed);
        }

        //mouse wheel for elevation
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            playerRigidbody.AddForce(transform.up * elevateSpeed * Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) * 20);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            playerRigidbody.AddForce(-transform.up * elevateSpeed * Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) * 20);
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
            playerRigidbody.AddForce(transform.forward * jumpForce);
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
        PlaySoundMultipleAudioSources(swimmingSounds);
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
                animator.characterAnimator.SetBool("grounded", true);
                float proportionalHeightSquared = Mathf.Pow((hoverHeight - hit.distance) / hoverHeight, 2);
                // Add more force the lower we are.
                playerRigidbody.AddForce(transform.up * proportionalHeightSquared * hoverForce);
            }
            else
            {
                animator.characterAnimator.SetBool("grounded", false);
            }
        }
        else
        {
            animator.characterAnimator.SetBool("grounded", false);
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
