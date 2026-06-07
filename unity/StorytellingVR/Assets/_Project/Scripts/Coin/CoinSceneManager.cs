using System.Collections;
using TMPro;
using UnityEngine;

public class CoinSceneManager : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;

    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    public AudioSource voiceSource;

    [Header("Subtitle UI")]
    public GameObject subtitlePanel;

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
        subtitlePanel.SetActive(false);

        coinSequence.OnCoinSequenceFinished += EndCoinTutorial;

        StartCoroutine(
            StartCoinScene()
        );
    }



    IEnumerator StartCoinScene()
    {
        yield return new WaitForSeconds(2);


        yield return ShowDialogue(
            "Customer:",
            "A pleasure doing business with you, trader. Here is your payment.",
            npcPaymentClip
        );


        npc.GiveCoin();


        yield return new WaitForSeconds(1.2f);


        varahaCoin.SetActive(true);


        yield return ShowDialogue(
            "Narrator:",
            "Take a closer look at this coin.",
            inspectCoinClip
        );
    }



    public void NarrateVaraha()
    {
        StartCoroutine(
            ShowDialogue(
                "Narrator:",
                "The Varaha was a gold coin used for important trade and represented the wealth of the Vijayanagara Empire.",
                varahaClip
            )
        );
    }



    public void NarrateKasu()
    {
        StartCoroutine(
            ShowDialogue(
                "Narrator:",
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
            "Narrator:",
            "You now understand the coins used in Vijayanagara markets.",
            understandCoinsClip
        );


        yield return ShowDialogue(
            "Narrator:",
            "Now use this knowledge while trading with the customers ahead.",
            tradingAheadClip
        );


        Debug.Log(
            "[SCENE FLOW] CoinScene complete"
        );


        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
    }




    IEnumerator ShowDialogue(
    string speaker,
    string line,
    AudioClip clip
)
    {
        subtitlePanel.SetActive(true);


        speakerNameText.text = speaker;


        dialogueText.text = line;


        if (
            clip != null &&
            voiceSource != null
        )
        {
            voiceSource.clip = clip;
            voiceSource.Play();

            yield return new WaitForSeconds(
                clip.length
            );
        }
        else
        {
            yield return new WaitForSeconds(4);
        }


        subtitlePanel.SetActive(false);
    }

}