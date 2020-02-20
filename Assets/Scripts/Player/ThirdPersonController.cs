using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdPersonController : MonoBehaviour
{
    //turn this off at start
    public GameObject[] stuffToTurnOffAtStart;
    public CreatureSpawner ghostPlanetSpawner;
    public GameObject startScreen;

    public int essenzCounter = 0;
    public List<Essenz> myEssenz = new List<Essenz>();

    public float elevateSpeed;
    //for smallFish character
    public float fishSwimSpeed;
    public float fishDriftSpeed, fishSprintSpeed;
    float sprintTimer;
    public float sprintTimeTotal;

    public bool boatArrived, clampingHorizontal;
    public GameObject theBoat, audioSpectrum;
    public DialogueText[] introDialogues;
    public DialogueText[] boatDialogues;

    //control bools
    public bool canMove = true, canMoveCam = true, isMovingCam, eating;
    Vector3 lastCamRot, currentCamRot;

    public float mouseSensitivityX = 20;
    public float mouseSensitivityY = 20;
    public float jumpForce = 220;
    float jumpTimer;
    public float jumpMin, jumpMax, forceMultiplier;

    public float hoverForce = 65f;
    public float repulsionForce = 200f, repulsionDistance = 5f;
    public float hoverHeight = 3.5f;
    public bool infiniteJump = false;
    public float jumpGroundMinDistance = 20f;
    public float maxVerticalLookAngle = 75f;
    public float minVerticalLookAngle = -75f;
    public LayerMask groundedMask, planetMask, spriteFadeMask;

    Transform cameraT;
    Vector3 smoothCameraVelocity;
    public Rigidbody playerRigidbody; // Public because of currents.
    GravityBody gravityBody;
    CapsuleCollider capCollider;

    float cameraDistance = 20f;
    float vRot = 0, hRot;

    public bool canJump;

    //public so we can call eating animations from EdibleCreature scripts
    public Animator animator;

    public float maxSpeed = 10f;

    //ecosystem stuff, creature type
    public enum CreatureType
    {
        SMALLFISH, CUDDLE, FAIRYDRAGON,
    }

    GameObject myMouth;
    public GameObject [] creatureSpriteHolders;
    //these 2 will become lists once the fish can be eaten :o
    public GameObject[] extraSchoolFish;
    public Animator[] extraFishAnimators;

    public CreatureType currentCreature;

    //player audio stuff
    public AudioSource[] playerVoices;
    public int voiceCounter = 0;
    public AudioClip[] eatingSounds, swimmingSounds, outOfBreathSounds;
    public GameObject bubbleParticles;
    
    void Start()
    {
        cameraT = Camera.main.transform;
        playerRigidbody = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        capCollider = GetComponent<CapsuleCollider>();
        myMouth = transform.GetChild(0).gameObject;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //turn off lotsa stuff
        for(int i = 0; i < stuffToTurnOffAtStart.Length; i++)
        {
            stuffToTurnOffAtStart[i].SetActive(false);  
        }
        //all the spawned fish
        for(int i = 0; i < ghostPlanetSpawner.activeCreatures.Count; i++)
        {
            ghostPlanetSpawner.activeCreatures[i].SetActive(false);
        }

        startScreen.SetActive(true);
        canMove = false;

        //start as small fish
        if (currentCreature == CreatureType.SMALLFISH)
        {
            fishSwimSpeed = fishDriftSpeed;
            sprintTimer = sprintTimeTotal;
            ChangeCreature(0);
            //set animation state back to DRIFT
            animator.SetBool("sprinting", false);
            theBoat.SetActive(false);
            theBoat.GetComponent<Boat>().cuttle.SetActive(false);
            audioSpectrum.GetComponent<PostProcessor>().ResetPostProcessing();
            audioSpectrum.SetActive(false);
            
        }
        //start as cuddle
        if (currentCreature == CreatureType.CUDDLE)
        {
            ChangeCreature(1);
        }
    }

    void Update()
    {

        if (startScreen.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //fade out start screen
                //then playerCan move
                startScreen.GetComponent<FadeUI>().fadingOut = true;
                canMove = true;
            }
        }

        //check if eating
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("eating"))
        {
            eating = true;
        }
        else
        {
            eating = false;
        }

        if (canMoveCam)
        {
            //allows user to move camera and focuses it on player
            CameraControls();
        }

        if (canMove)
        {
            // fade a sprite if it's in front on your camera
            FadeCamObstructions();

            //for moving up and down
            ChangeElevation();

            //while small fish
            if (currentCreature == CreatureType.SMALLFISH)
            {
                //constantly swim forward
                playerRigidbody.AddForce(transform.forward * fishSwimSpeed);
            }

            //called while spacebar is pressed
            TakeJumpInput();

            //called when spacebar is released
            OnJumpRelease();

            //correlates jumping logic with animations
            JumpDetection();

            //repulses player from planet when they land too hard
            RepulsionLogic();
        }

        //after fish eat at least 15 plankton --
        if(essenzCounter > 10 && currentCreature == CreatureType.SMALLFISH && !boatArrived)
        {
            //queue the boat...
            theBoat.SetActive(true);
            theBoat.GetComponent<Boat>().SetMovement();
            //turn on post Processing
            audioSpectrum.SetActive(true);

            //turn of intro dialogues
            for (int i = 0; i < introDialogues.Length; i++)
            {
                introDialogues[i].gameObject.SetActive(false);
            }

            //start boat dialogue
            for (int i = 0; i < boatDialogues.Length; i++)
            {
                boatDialogues[i].EnableDialogue();
            }

            boatArrived = true;
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
        if (playerRigidbody.velocity.magnitude > maxSpeed) {
            playerRigidbody.velocity = Vector3.ClampMagnitude(playerRigidbody.velocity, maxSpeed);
        }
    }

    void CameraControls()
    {
        // Rotate player around Y axis
        if(!clampingHorizontal)
            hRot = Input.GetAxis("Mouse X") * mouseSensitivityX;
        //only happens during fish chase
        else
        {
            hRot = 0;
        }
        Vector3 oldCameraPosition = cameraT.position;
        transform.RotateAround(transform.position, transform.up, hRot);
        cameraT.position = oldCameraPosition;

        // Rotate camera around X axis
        // Position player
        vRot += -1 * Input.GetAxis("Mouse Y") * mouseSensitivityY;
        vRot = Mathf.Clamp(vRot, minVerticalLookAngle, maxVerticalLookAngle);
        Vector3 toCamera = Quaternion.AngleAxis(vRot, Vector3.right) * -Vector3.forward;
        Vector3 futureCameraPosition = transform.TransformPoint(toCamera * cameraDistance);
        cameraT.position = Vector3.SmoothDamp(cameraT.position, futureCameraPosition, ref smoothCameraVelocity, .5f);
        cameraT.LookAt(transform, gravityBody.GetUp());
        currentCamRot = cameraT.localEulerAngles;

        //check if cam pos or cam rot has changed
        if(Vector3.Distance(cameraT.position, oldCameraPosition) > 0.1f ||
            Vector3.Distance(currentCamRot, lastCamRot) > 0.1f)
        {
            isMovingCam = true;
        }
        else
        {
            isMovingCam = false;
        }

        lastCamRot = cameraT.localEulerAngles;
    }

    void FadeCamObstructions()
    {
        //Ray camRay = new Ray(cameraT.position, cameraT.TransformDirection(Vector3.forward));
        //RaycastHit camHit;

        //if (Physics.Raycast(camRay, out camHit, 100f, spriteFadeMask))
        //{
        //    //Debug.DrawLine(transform.position, camHit.point, Color.white, 2.5f);
        //    //Debug.Log("Hit " + camHit.transform.gameObject.name);

        //    camHit.transform.GetComponent<FadeForCamera>().Fade();
        //}
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
        if (Input.GetButton("Jump"))
        {
            //while small fish, SPRINT 
            if (currentCreature == CreatureType.SMALLFISH && !clampingHorizontal)
            {
                sprintTimer -= Time.deltaTime;
                if (sprintTimer > 0)
                {
                    fishSwimSpeed = fishSprintSpeed;
                    //set animation state to SPRINT
                    animator.SetBool("sprinting", true);
                    //for the school surrounding you
                    for (int i = 0; i < extraFishAnimators.Length; i++)
                    {
                        extraFishAnimators[i].SetBool("sprinting", true);
                    }
                }
                //out of breath!!!
                else
                {
                    fishSwimSpeed = fishDriftSpeed;
                    //set animation state back to DRIFT
                    animator.SetBool("sprinting", false);
                    //for the school surrounding you
                    for (int i = 0; i < extraFishAnimators.Length; i++)
                    {
                        extraFishAnimators[i].SetBool("sprinting", false);
                    }

                    //to indicate we cant sprint anymore!
                    if (!playerVoices[voiceCounter].isPlaying)
                    {
                        PlaySound(outOfBreathSounds);
                    }
                }

            }

            //while cuttle
            if (currentCreature == CreatureType.CUDDLE)
            {
                if (jumpTimer > jumpMin && animator.GetBool("warmingUp") == false)
                {
                    animator.SetBool("warmingUp", true);
                }

                //as long as it is less than the max, it goes up
                if (jumpTimer < jumpMax)
                {
                    jumpTimer += Time.deltaTime;
                    //Debug.Log(jumpTimer);
                }
            }

        }
    }

    void OnJumpRelease()
    {
        //on button up
        if (Input.GetButtonUp("Jump"))
        {
            //while small fish
            if (currentCreature == CreatureType.SMALLFISH)
            {
                fishSwimSpeed = fishDriftSpeed;
                //set animation state back to DRIFT
                animator.SetBool("sprinting", false);
                //for the school surrounding you
                for (int i = 0; i < extraFishAnimators.Length; i++)
                {
                    extraFishAnimators[i].SetBool("sprinting", false);
                }
                sprintTimer = sprintTimeTotal;
            }

            //while cuttle fish
            if (currentCreature == CreatureType.CUDDLE)
            {
                //can jump
                if (infiniteJump || canJump)
                {
                    //just pressed, so normal jump
                    if (jumpTimer <= jumpMin)
                    {
                        playerRigidbody.AddForce(transform.forward * jumpForce);
                    }

                    //held, so we add to the force
                    if (jumpTimer > jumpMin)
                    {
                        float powerJumpForce = jumpForce + (jumpTimer * forceMultiplier);
                        //Debug.Log(powerJumpForce);
                        playerRigidbody.AddForce(transform.forward * powerJumpForce);

                        //spawn bubble particles
                        GameObject bubbles = Instantiate(bubbleParticles, transform.position, Quaternion.identity, transform);
                        bubbles.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }

                    //animate and play sound, reset jump timer
                    animator.SetBool("warmingUp", false);
                    animator.SetTrigger("flutter");
                    PlaySound(swimmingSounds);
                    jumpTimer = 0;
                }
            }
        }
    }

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
                animator.SetBool("grounded", true);
                float proportionalHeightSquared = Mathf.Pow((hoverHeight - hit.distance) / hoverHeight, 2);
                // Add more force the lower we are.
                playerRigidbody.AddForce(transform.up * proportionalHeightSquared * hoverForce);
            }
            else
            {
                animator.SetBool("grounded", false);
            }
        }
        else
        {
            animator.SetBool("grounded", false);
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

    public void ChangeCreature(int desiredCreature)
    {
        //turn off all spriteHolders besides desiredCreature
        for(int i = 0; i < creatureSpriteHolders.Length; i++)
        {
            if(i != desiredCreature)
                creatureSpriteHolders[i].SetActive(false);
            else
            {
                creatureSpriteHolders[i].SetActive(true);
            }
        }

        if (desiredCreature != 0)
        {
            //turn off extra fish in school 
            for (int i = 0; i < extraSchoolFish.Length; i++)
            {
                extraSchoolFish[i].SetActive(false);
            }
        }

        //set current Creature type and grab its animator
        switch (desiredCreature)
        {
            case 0:
                currentCreature = CreatureType.SMALLFISH;
                animator = creatureSpriteHolders[0].GetComponent<Animator>();
                //turn on extra fish in school and set animator bools
                for(int i = 0; i < extraSchoolFish.Length; i++)
                {
                    extraSchoolFish[i].SetActive(true);
                    extraFishAnimators[i].SetBool("sprinting", false);
                }
                capCollider.radius = 5;
                elevateSpeed = 35;
                myMouth.GetComponent<SphereCollider>().radius = 1.75f;
                break;
            case 1:
                currentCreature = CreatureType.CUDDLE;
                animator = creatureSpriteHolders[1].GetComponent<Animator>();
                capCollider.radius = 0.5f;
                elevateSpeed = 50;
                myMouth.GetComponent<SphereCollider>().radius = 0.5f;
                //transform.localScale *= 5;
                break;
            case 2:
                currentCreature = CreatureType.FAIRYDRAGON;
                animator = creatureSpriteHolders[2].GetComponent<Animator>();
                break;
        }
    }

    //called to play sounds 
    public void PlaySound(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);

        if(voiceCounter < playerVoices.Length - 1)
        {
            voiceCounter++;
        }
        else
        {
            voiceCounter = 0;
        }

        playerVoices[voiceCounter].PlayOneShot(soundArray[randomSound]);
    }
}
