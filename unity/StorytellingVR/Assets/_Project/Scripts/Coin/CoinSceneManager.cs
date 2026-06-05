using System.Collections;
using TMPro;
using UnityEngine;

public class CoinSceneManager : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;
    public TMP_Text dialogueText;
    public AudioSource voiceSource;

    [Header("Coins")]
    public GameObject varahaCoin;


    [Header("Sequence")]
    public CoinSequenceManager coinSequence;

    [Header("Voice Lines")]
    public AudioClip npcPaymentClip;
    public AudioClip inspectCoinClip;
    public AudioClip varahaClip;
    public AudioClip kasuClip;
    public AudioClip understandCoinsClip;
    public AudioClip tradingAheadClip;


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
            "A pleasure doing business with you, trader. Here is your payment.",
            npcPaymentClip
            
        );


        // NPC raises hand
        npc.GiveCoin();



        // wait for hand animation
        yield return new WaitForSeconds(1.2f);



        // reveal Varaha
        varahaCoin.SetActive(true);



        yield return ShowDialogue(
            "Take a closer look at this coin.",
            inspectCoinClip
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
                varahaClip
            )
        );
    }



    // called from CoinSequenceManager
    public void NarrateKasu()
    {
        StartCoroutine(
            ShowDialogue(
                "The Kasu was a bronze coin used by common people for everyday marketplace transactions.",
                kasuClip
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
            understandCoinsClip
        );


        yield return ShowDialogue(
            "Now use this knowledge while trading with the customers ahead.",
            tradingAheadClip
        );

        GameManager.Instance.LoadNextScene();

        Debug.Log(
            "START CUSTOMER GAME LOOP HERE"
        );

    }




    IEnumerator ShowDialogue(string line, AudioClip clip)
    {
        dialogueText.text = line;

        voiceSource.clip = clip;
        voiceSource.Play();

        yield return new WaitForSeconds(clip.length);

        dialogueText.text = "";
    }
}