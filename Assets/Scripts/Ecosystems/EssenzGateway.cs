using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// A generic gateway object that requires a certain # of essenz for player to progress.
/// Still need to rewrite dialogue logic to work with Monologues
/// </summary>
public class EssenzGateway : MonoBehaviour {

    PlayerController pc;

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
        pc = FindObjectOfType<PlayerController>();
	}

    public void ActivateDonationButtons()
    {
        //player has enough
        if (pc.essenceInventory.collectedEssenz.Count >= essenzToll)
        {
            //set dialogue
            myText.text = essenzMessage + "If you wish to proceed, you must donate your Essenz";
            myDialogueText.ResetStringText();
            myDialogueText.EnableDialogue();
            myDialogueText.waitTime = 10;

            //while choice is active turn off movement, activate cursor
            pc.canMove = false;
            pc.playerRigidbody.velocity = Vector3.zero;
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
        if (pc.essenceInventory.collectedEssenz.Count >= essenzToll)
        {
            StartCoroutine(DonateEssenz());
        }
    }

    IEnumerator DonateEssenz()
    {
        //we set the toll to the same amount as player has
        if (takeAllEssenz)
        {
            essenzToll = pc.essenceInventory.collectedEssenz.Count - 1;
        }
        //loop through essenz sending them towards donation source
        for(int i = 0; i < essenzToll; i++)
        {
            Essenz essenz = pc.essenceInventory.collectedEssenz[i];
            essenz.DonateEssenz(transform);
            pc.essenceInventory.collectedEssenz.Remove(essenz);
            yield return new WaitForSeconds(0.1f);
        }

        //activate particles and deactive mouthman
        currentPath.SetActive(true);

        essenzGagu.SetActive(false);

        pc.canMove = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    //don't donate and turn this off until later
    public void NoDonate()
    {
        pc.canMove = true;
        //deactivate everything
        yesDonate.SetActive(false);
        noDonate.SetActive(false);

        myDialogueText.DisableDialogue();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
}
