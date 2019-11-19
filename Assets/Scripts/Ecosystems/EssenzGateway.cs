using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EssenzGateway : MonoBehaviour {

    ThirdPersonController tpc;
    public ThirdPersonController.CreatureType necessaryType;

    public int essenzToll;
    public bool takeAllEssenz;
    public GameObject essenzGagu, currentPath;

    Text myText;
    public string essenzMessage, notEnoughEssenz;

    public GameObject yesDonate, noDonate;

    DialogueText myDialogueText;

	void Start () {
        //turn off gate objects
        currentPath.SetActive(false);
        yesDonate.SetActive(false);
        noDonate.SetActive(false);

        //set text
        myText = GetComponent<Text>();
        myDialogueText = GetComponent<DialogueText>();

        //player script ref
        tpc = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
	}

    public void ActivateDonationButtons()
    {
        //player has enough
        if (tpc.essenzCounter >= essenzToll)
        {
            //set dialogue
            myText.text = essenzMessage + "If you wish to proceed, you must donate your Essenz";
            myDialogueText.ResetStringText();
            myDialogueText.EnableDialogue();
            myDialogueText.waitTime = 10;

            //while choice is active turn off movement, activate cursor
            tpc.canMove = false;
            tpc.playerRigidbody.velocity = Vector3.zero;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //turn on world space buttons
            yesDonate.SetActive(true);
            noDonate.SetActive(true);
        }
        //player doesn't have enough
        else
        {
            //set dialogue
            myText.text = notEnoughEssenz + "\n" + "If you wish to proceed," + "\n" +
                "Return when your Aura has grown";
            myDialogueText.ResetStringText();
            myDialogueText.EnableDialogue();
            myDialogueText.waitTime = 1;
        }
    }

    //called to donate and activate current gate
    public void YesDonate()
    {
        //extra check
        if(tpc.essenzCounter >= essenzToll)
        {
            StartCoroutine(DonateEssenz());
        }
    }

    IEnumerator DonateEssenz()
    {
        //we set the toll to the same amount as player has
        if (takeAllEssenz)
        {
            essenzToll = tpc.myEssenz.Count - 3;
        }
        //loop through essenz sending them towards donation source
        for(int i = 0; i < essenzToll; i++)
        {
            tpc.myEssenz[0].DonateEssenz(transform);
            tpc.myEssenz.Remove(tpc.myEssenz[0]);
            tpc.essenzCounter = tpc.myEssenz.Count;
            yield return new WaitForSeconds(0.1f);
        }

        //activate particles and deactive mouthman
        currentPath.SetActive(true);

        essenzGagu.SetActive(false);

        tpc.canMove = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //don't donate and turn this off until later
    public void NoDonate()
    {
        tpc.canMove = true;
        //deactivate everything
        yesDonate.SetActive(false);
        noDonate.SetActive(false);

        myDialogueText.DisableDialogue();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
}
