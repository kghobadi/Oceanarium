using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttleKiller : MonoBehaviour {
    //player ref
    GameObject player;
    ThirdPersonController tpc;

    public AudioClip eatingSound;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        tpc = player.GetComponent<ThirdPersonController>();

    }
    
    void FixedUpdate () {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 250 * Time.deltaTime);
	}

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //for the school surrounding you
            for (int i = 0; i < tpc.extraFishAnimators.Length; i++)
            {
                tpc.extraFishAnimators[i].gameObject.SetActive(false);
            }

            tpc.playerVoices[0].PlayOneShot(eatingSound);
            tpc.ChangeCreature(1);
            tpc.transform.localScale *= 5;
            tpc.clampingHorizontal = false;
            tpc.jumpForce = 2200;

            Destroy(gameObject);
        }
    }
}
