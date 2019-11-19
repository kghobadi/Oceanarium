using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currents : MonoBehaviour {
    //player ref
    GameObject player;
    PlayerController pc;

    //current vars
    public float currentSpeed;
    [HideInInspector]
    public Transform entrance, endPoint;
    public bool entering, hasEntered;

    [HideInInspector]
    public AudioSource ambientSource;
    public AudioClip[] currentAmbience;

    void Awake()
    {
        ambientSource = GameObject.FindGameObjectWithTag("AmbientAudio").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
    }

    void Start()
    {
        entrance = transform.GetChild(0);
        endPoint = transform.GetChild(1);
    }

    void Update()
    {
        //only runs when player tries to enter current. 
        //pulls you to public transform entrance
        if (entering)
        {
            pc.playerRigidbody.velocity = Vector3.zero;
            player.transform.position = Vector3.MoveTowards(player.transform.position, entrance.position, currentSpeed * Time.deltaTime);

            if(Vector3.Distance(player.transform.position, entrance.position) < 0.5f)
            {
                entering = false;
                pc.playerRigidbody.velocity = Vector3.zero;
                hasEntered = true;
            }
        }

        //only if hasEntered has completed
        if (hasEntered)
        {
            pc.transform.position = Vector3.MoveTowards(pc.transform.position, endPoint.position, currentSpeed * Time.deltaTime);

            if (Vector3.Distance(pc.transform.position, endPoint.position) < 1)
            {
                hasEntered = false;
                StartCoroutine(WaitToStopCurrent(player));
            }
            Debug.Log("applying force");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //we are entering the current near the entrance 
            if(Vector3.Distance(player.transform.position, entrance.position) < 25)
            {
                //reset player pos to entrance 
                entering = true;

                //reset velocity
                pc.playerRigidbody.velocity = Vector3.zero;
                pc.canMove = false;
            }

            //we have entered elsewhere
            else
            {
                //reset velocity
                pc.playerRigidbody.velocity = Vector3.zero;
                pc.canMove = false;
                hasEntered = true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!ambientSource.isPlaying)
                PlaySound(currentAmbience);
        }
    }

    //need a delay before stopping current
    IEnumerator WaitToStopCurrent(GameObject other)
    {
        yield return new WaitForSeconds(1);

        ambientSource.Stop();

        pc.canMove = true;
    }

    //called to play sounds 
    public void PlaySound(AudioClip[] soundArray)
    {
        int randomSound = Random.Range(0, soundArray.Length);
        ambientSource.clip = soundArray[randomSound];

        ambientSource.Play();
    }

   
}
