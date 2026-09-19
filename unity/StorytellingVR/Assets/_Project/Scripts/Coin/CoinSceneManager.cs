using System.Collections;
using TMPro;
using UnityEngine;

public class CoinSceneManager : MonoBehaviour
{
    private const string RahimPaymentLineId = "RAHIM_COIN_PAYMENT_01";
    private const string NarratorInspectCoinLineId = "NARRATOR_COIN_INSPECT_01";
    private const string NarratorVarahaLineId = "NARRATOR_COIN_VARAHA_01";
    private const string NarratorKasuLineId = "NARRATOR_COIN_KASU_01";
    private const string NarratorUnderstandCoinsLineId = "NARRATOR_COIN_COMPLETE_01";
    private const string NarratorTradingAheadLineId = "NARRATOR_COIN_TRADING_AHEAD_01";

    [Header("References")]
    public NPCAnimationController npc;

    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    public AudioSource voiceSource;

    [Header("Subtitle UI")]
    public GameObject subtitlePanel;

    [Header("Coins")]
    public GameObject varahaCoin;

    [Header("Instruction")]
    public InstructionPromptManager instructionPrompt;

    [Header("Sequence")]
    public CoinSequenceManager coinSequence;


    [Header("Voice Lines")]
    [SerializeField] private DialogueVoiceDatabase voiceDatabase;

    public AudioClip npcPaymentClip;
    public AudioClip inspectCoinClip;
    public AudioClip varahaClip;
    public AudioClip kasuClip;
    public AudioClip understandCoinsClip;
    public AudioClip tradingAheadClip;



    void Start()
    {
        if (voiceSource == null)
            voiceSource = gameObject.AddComponent<AudioSource>();

        subtitlePanel.SetActive(false);

        if (coinSequence != null)
        {
            coinSequence.OnCoinSequenceFinished += EndCoinTutorial;
        }

        StartCoroutine(
            StartCoinScene()
        );
    }



    IEnumerator StartCoinScene()
    {
        yield return new WaitForSeconds(2);


        yield return ShowDialogue(
            "Rahim",
            "A pleasure doing business with you, trader. Here is your payment.",
            RahimPaymentLineId,
            npcPaymentClip
        );


        if (npc != null)
        {
            npc.GiveCoin();
        }


        yield return new WaitForSeconds(1.2f);


        varahaCoin.SetActive(true);


        yield return ShowDialogue(
            "Narrator",
            "Take a closer look at the coin you have received.",
            NarratorInspectCoinLineId,
            inspectCoinClip
        );


        if (instructionPrompt != null)
        {
            instructionPrompt.ShowTrigger(
                "Press right trigger to inspect coin"
            );
        }
    }



    public void NarrateVaraha()
    {
        StartCoroutine(
            VarahaRoutine()
        );
    }



    IEnumerator VarahaRoutine()
    {
        yield return ShowDialogue(
            "Narrator",
            "This is a Varaha, a gold coin used for valuable transactions and a reflection of the wealth of the Vijayanagara Empire.",
            NarratorVarahaLineId,
            varahaClip
        );


        if (instructionPrompt != null)
        {
            instructionPrompt.ShowTrigger(
                "Press right trigger to continue"
            );
        }
    }



    public void NarrateKasu()
    {
        StartCoroutine(
            ShowDialogue(
                "Narrator",
                "The Kasu, by contrast, is a smaller bronze coin commonly used for everyday transactions in the marketplace.",
                NarratorKasuLineId,
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
            "Narrator",
            "Together, coins such as these helped trade flow through the markets of Vijayanagara.",
            NarratorUnderstandCoinsLineId,
            understandCoinsClip
        );


        yield return ShowDialogue(
            "Narrator",
            "A successful trade does not end with an agreed price. You must still prepare the goods your customer has purchased.",
            NarratorTradingAheadLineId,
            tradingAheadClip
        );


        Debug.Log(
            "[SCENE FLOW] CoinScene complete"
        );


        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneByName("SpicesInteraction");
        }
    }




    IEnumerator ShowDialogue(
        string speaker,
        string line,
        string lineId,
        AudioClip clip
    )
    {
        subtitlePanel.SetActive(true);


        speakerNameText.text = speaker;


        dialogueText.text = line;


        AudioClip resolvedClip = ResolveVoiceClip(lineId, clip);

        if (
            resolvedClip != null &&
            voiceSource != null
        )
        {
            voiceSource.clip = resolvedClip;

            voiceSource.Play();


            yield return new WaitForSeconds(
                resolvedClip.length
            );
        }
        else
        {
            yield return new WaitForSeconds(4);
        }


        subtitlePanel.SetActive(false);
    }

    private AudioClip ResolveVoiceClip(string lineId, AudioClip fallbackClip)
    {
        if (voiceDatabase == null)
            return fallbackClip;

        AudioClip databaseClip = voiceDatabase.GetAudioClip(lineId);
        return databaseClip != null ? databaseClip : fallbackClip;
    }
}
