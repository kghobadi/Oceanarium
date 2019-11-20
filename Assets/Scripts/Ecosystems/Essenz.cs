using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essenz : MonoBehaviour {
    GameObject player;
    ThirdPersonController tpc;
    Orbit orbitScript;

    public bool vortexing, orbiting, donating;
    Vector3 donationFinal;
    public float vortexSpeed = 100f, donationSpeed = 15f;
    public float minDist = 0.25f;

    AudioSource myAudio;
    public AudioClip[] donationSounds;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        tpc = player.GetComponent<ThirdPersonController>();
        vortexSpeed = Random.Range(75, 100);
        transform.SetParent(player.transform);
        myAudio = GetComponent<AudioSource>();

        orbitScript = GetComponent<Orbit>();
        orbitScript.orbiting = false;
        orbitScript.orbitalSpeed = vortexSpeed;
        orbitScript.planetToOrbit = player.transform;
	}

    void Update()
    {
        //orbiting player
        if (vortexing)
        {
            Vector3 mouthCenter = GameObject.FindGameObjectWithTag("PlayerMouth").transform.position;

            transform.position = Vector3.Lerp(transform.position, mouthCenter, donationSpeed * Time.deltaTime);

            //when reaches mouth, send it into orbit
            if (Vector3.Distance(transform.position, mouthCenter) < minDist)
            {
                vortexing = false;
                orbitScript.orbiting = true;
            }
        }

        //donating to some source
        if (donating)
        {
            transform.position = Vector3.Lerp(transform.position, donationFinal, donationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, donationFinal) < 1f)
            {
                Destroy(gameObject);
            }
        }
    }

    //called on donation decider
    public void DonateEssenz(Transform donationDest)
    {
        orbitScript.orbiting = false;

        donationFinal = donationDest.position;

        donating = true;

        //play a sound
        tpc.PlaySound(donationSounds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !vortexing)
        {
            //vortex towards player
            orbitScript.orbiting = false;
            vortexing = true;

            tpc.myEssenz.Add(this);

            tpc.essenzCounter = tpc.myEssenz.Count;
        }
    }

}
