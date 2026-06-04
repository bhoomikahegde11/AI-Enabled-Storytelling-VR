using System.Collections;
using TMPro;
using UnityEngine;

public class CoinSceneManager : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;
    public TMP_Text dialogueText;


    [Header("Coins")]
    public GameObject varahaCoin;


    [Header("Sequence")]
    public CoinSequenceManager coinSequence;



    void Start()
    {
        // listen for coin tutorial ending
        coinSequence.OnCoinSequenceFinished += EndCoinTutorial;


        StartCoroutine(
            StartCoinScene()
        );
    }



    IEnumerator StartCoinScene()
    {
        yield return new WaitForSeconds(2);


        yield return ShowDialogue(
            "A fair bargain, merchant. Here is your payment.",
            3
        );


        // NPC raises hand
        npc.GiveCoin();



        // wait for hand animation
        yield return new WaitForSeconds(1.2f);



        // reveal Varaha
        varahaCoin.SetActive(true);



        yield return ShowDialogue(
            "Take a closer look at this coin.",
            3
        );


        /*
         Player now:
         
         Trigger
            ↓
         CoinInteraction
            ↓
         Inspect mode
            ↓
         CoinSequence starts
        */
    }



    // called from CoinInteraction
    public void NarrateVaraha()
    {
        StartCoroutine(
            ShowDialogue(
                "The Varaha was a gold coin used for important trade and represented the wealth of the Vijayanagara Empire.",
                6
            )
        );
    }



    // called from CoinSequenceManager
    public void NarrateKasu()
    {
        StartCoroutine(
            ShowDialogue(
                "The Kasu was a bronze coin used by common people for everyday marketplace transactions.",
                6
            )
        );
    }




    void EndCoinTutorial()
    {
        StartCoroutine(
            EndDialogue()
        );
    }



    IEnumerator EndDialogue()
    {
        yield return ShowDialogue(
            "You now understand the coins used in Vijayanagara markets.",
            4
        );


        yield return ShowDialogue(
            "Now use this knowledge while trading with the customers ahead.",
            4
        );


        Debug.Log(
            "START CUSTOMER GAME LOOP HERE"
        );


        // later:
        // customerSpawner.StartCustomers();
    }




    IEnumerator ShowDialogue(
        string line,
        float time
    )
    {
        dialogueText.text = line;


        yield return
            new WaitForSeconds(time);


        dialogueText.text = "";
    }
}