using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//an object with this script can be eaten by the player

public class EdibleObject : MonoBehaviour {
    //ref to player and spawner on this planet
    GameObject player;
    PlayerController pc;

    public bool edible, beingConsumed;
    public float consumptionSpeed = 30;

    //ref to orbit script (uses this during swimming state)
    Orbit myOrbiter;

    //set publicly to determine place in ecosystem
    public enum CreatureTypes
    {
        SMALLFISH, CUDDLE, FAIRYDRAGON,
    }

    //actual enum holders
    public CreatureTypes myType;
    public ThirdPersonController.CreatureType predatorType;

    //creature animation
    Animator objectAnimator;
    public GameObject eatingChunks;

    //essenz tracking
    public int essenzCount;
    public GameObject essenzPrefab;

    GameObject edibleObjectVisual;
    public float resetTime;
    Vector3 originalPos;

    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        //setting this for now
        predatorType = ThirdPersonController.CreatureType.SMALLFISH;
        myOrbiter = GetComponent<Orbit>();
        objectAnimator = GetComponent<Animator>();
        originalPos = transform.position;
        edibleObjectVisual = transform.GetChild(0).gameObject;
        edible = true;
    }

    //check distance for fleeing
    void Update()
    {
        //vortex towards player mouth
        //if(beingConsumed)
        //{
        //    Vector3 mouthPos = playerMouth.position;

        //    transform.position = Vector3.MoveTowards(transform.position, mouthPos, consumptionSpeed * Time.deltaTime);

        //    if(Vector3.Distance(transform.position, mouthPos) < 1f)
        //    {
        //        if (!tpc.eating)
        //        {
        //            PlayerEatsMe();
        //        }
        //        else
        //        {
        //            Debug.Log("I'm already eating!");
        //        }
        //    }
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerMouth" && edible)
        {
            //get eaten when player is cuddle
            //if (currentCreature == ThirdPersonController.CreatureType.SMALLFISH)
            //{
            //    beingConsumed = true;
            //    edible = false;
            //    //myOrbiter.swimming = false;
            //}
        }
    }

    //call when player is eating
    //This should only happen if player  is NOT currently eating
    void PredatorEatsMe()
    {
        //play eating animation for this one, eating sound
        beingConsumed = false;
        //pc.animator.SetTrigger("eat");
        //pc.PlaySound(tpc.eatingSounds);

        //spawn eating chunks
        //Instantiate(eatingChunks, playerMouth.position, Quaternion.identity);

        //spawn essenz
        for (int i = 0; i < essenzCount; i++)
        {
            Vector3 randomSpawnPos = transform.position + Random.insideUnitSphere * 3;
            Instantiate(essenzPrefab, randomSpawnPos, Quaternion.identity);
        }

        edibleObjectVisual.SetActive(false);
        StartCoroutine(ResetObject());
    }

    //called when object is eaten to wait then bring it back to life.
    IEnumerator ResetObject()
    {
        yield return new WaitForSeconds(resetTime);
        transform.position = originalPos;
        edible = true;
        edibleObjectVisual.SetActive(true);
    }

}
