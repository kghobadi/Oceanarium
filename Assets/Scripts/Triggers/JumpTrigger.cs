using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrigger : AudioHandler {
    PlayerController pc;
    Rigidbody rBody;
    GravityBody grav;
    CreatureAnimation cAnimation;
    ParticleSystem bubbleBounce;

    [Header("Jump Settings")]
    public float jumpForce;
    public AudioClip[] jumpSounds;
    
    public override void Awake ()
    {
        base.Awake();

        pc = FindObjectOfType<PlayerController>();
        rBody = GetComponent<Rigidbody>();
        grav = GetComponent<GravityBody>();
        cAnimation = GetComponent<CreatureAnimation>();
        bubbleBounce = GetComponentInChildren<ParticleSystem>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            JumpPlayer();
        }
    }
    
    void JumpPlayer()
    {
        //get force
        Vector3 upForce = transform.up * jumpForce;
        //apply to player
        pc.playerRigidbody.AddForce(upForce);
        //player bounce anim
        pc.animator.Animator.SetTrigger("bounce");
        //reset meditation timer
        pc.idleTimer = 0;
        //play sound
        PlayRandomSoundRandomPitch(jumpSounds, 1f);
        //trigger anim
        cAnimation.Animator.SetTrigger("jump");
        //jump particles
        bubbleBounce.Play();

        Debug.Log(gameObject.name + " bounced player");
    }
    
}
