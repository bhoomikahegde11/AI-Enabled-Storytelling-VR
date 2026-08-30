using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private const string RahimIntroLineId = "RAHIM_TRANSACTION_INTRO_01";
    private const string BhaskaraTeachTradeLineId = "BHASKARA_TRANSACTION_STUDY_TRADE_01";
    private const string BhaskaraExplainTradePanelLineId = "BHASKARA_TRANSACTION_PANEL_01";
    private const string BhaskaraExplainNegotiationLineId = "BHASKARA_TRANSACTION_NEGOTIATION_01";
    private const string BhaskaraAskHighPriceLineId = "BHASKARA_TRANSACTION_ASK_HIGH_PRICE_01";
    private const string RahimAngryHighPriceLineId = "RAHIM_TRANSACTION_HIGH_PRICE_01";
    private const string BhaskaraHighPriceLessonLineId = "BHASKARA_TRANSACTION_HIGH_PRICE_LESSON_01";
    private const string RahimAcceptFairPriceLineId = "RAHIM_TRANSACTION_ACCEPT_FAIR_01";
    private const string BhaskaraFairPriceEndingLineId = "BHASKARA_TRANSACTION_FAIR_PRICE_ENDING_01";
    private const string RahimStillTooHighLineId = "RAHIM_TRANSACTION_STILL_TOO_HIGH_01";
    private const string BhaskaraStillTooHighLessonLineId = "BHASKARA_TRANSACTION_STILL_TOO_HIGH_LESSON_01";
    private const string RahimAcceptLowPriceLineId = "RAHIM_TRANSACTION_ACCEPT_LOW_01";
    private const string BhaskaraLowProfitLessonLineId = "BHASKARA_TRANSACTION_LOW_PROFIT_LESSON_01";
    private const string TutorialMerchantSpeaker = "Bhaskara";

    //==================================================
    // REFERENCES
    //==================================================

    [Header("Dialogue UI")]
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;

    [Header("Managers")]
    public VoiceRecognitionManager voiceRecognitionManager;
    public Level1HUDManager hudManager;
    public RespectUIManager respectUIManager;
    [SerializeField] private DialogueVoiceDatabase voiceDatabase;

    //==================================================
    // AUDIO SOURCES
    //==================================================

    [Tooltip("Audio source used for Rahim's dialogue.")]
    public AudioSource customerAudioSource;

    [Tooltip("Audio source used for the stall owner's dialogue.")]
    public AudioSource merchantAudioSource;


    //==================================================
    // RAHIM AUDIO
    //==================================================

    [Header("Rahim - Customer Audio")]

    [Tooltip("Rahim introduces himself and asks for one veesai of cardamom.")]
    public AudioClip rahimIntroClip;

    [Tooltip("Rahim reacts angrily to the deliberately high tutorial price.")]
    public AudioClip rahimAngryHighPriceClip;

    [Tooltip("Rahim accepts a fair price.")]
    public AudioClip rahimAcceptFairPriceClip;

    [Tooltip("Rahim warns that the second offer is still too expensive.")]
    public AudioClip rahimStillTooHighClip;

    [Tooltip("Rahim happily accepts a very low price.")]
    public AudioClip rahimAcceptLowPriceClip;


    //==================================================
    // STALL OWNER AUDIO
    //==================================================

    [Header("Stall Owner - Tutorial Mentor Audio")]

    [Tooltip("Stall owner introduces the idea of studying a trade before answering.")]
    public AudioClip merchantTeachTradeClip;

    [Tooltip("Stall owner explains the Current Trade panel after the player opens it.")]
    public AudioClip merchantExplainTradePanelClip;

    [Tooltip("Stall owner explains cost price, earnings and reputation.")]
    public AudioClip merchantExplainNegotiationClip;

    [Tooltip("Stall owner asks the player to deliberately offer 70 Varahas.")]
    public AudioClip merchantAskHighPriceClip;

    [Tooltip("Stall owner explains reputation and customer trust after the high price.")]
    public AudioClip merchantHighPriceLessonClip;

    [Tooltip("Stall owner concludes the tutorial after a successful fair trade.")]
    public AudioClip merchantFairPriceEndingClip;

    [Tooltip("Stall owner explains why the second offer is still too high.")]
    public AudioClip merchantStillTooHighLessonClip;

    [Tooltip("Stall owner explains the danger of selling below value.")]
    public AudioClip merchantLowProfitLessonClip;

    //==================================================
    // UI
    //==================================================

    [Header("UI")]
    public TMP_Text coinsEarnedText;
    public TMP_Text spokenPriceText;


    //==================================================
    // TUTORIAL STATE
    //==================================================

    private int respect = 100;
    private int coins = 0;

    private bool waitingForHighPrice = false;
    private bool waitingForFairPrice = false;
    private bool tutorialFinished = false;

    private bool currentTradePanelOpened = false;
    private bool currentTradePanelVisible = false;
    private AudioSource fallbackAudioSource;


    //==================================================
    // TUTORIAL TRADE DATA
    //==================================================

    private const string TutorialBuyer = "Rahim";
    private const string TutorialSpice = "Cardamom";
    private const string TutorialQuantity = "1 Veesai";
    private const int TutorialCostPrice = 18;


    //==================================================
    // UNITY METHODS
    //==================================================

    void Start()
    {
        if (hudManager == null)
            hudManager = FindFirstObjectByType<Level1HUDManager>();

        UpdateMoneyUI();
        UpdateRespectUI();

        if (hudManager != null)
            hudManager.HideCurrentTrade();

        StartCoroutine(TutorialSequence());
    }


    void Update()
    {
        if ((OVRInput.GetDown(OVRInput.Button.Four) ||
             Input.GetKeyDown(KeyCode.Y)) &&
            hudManager != null)
        {
            currentTradePanelOpened = true;
            currentTradePanelVisible = !currentTradePanelVisible;

            hudManager.SetTutorialTrade(
                TutorialBuyer,
                TutorialSpice,
                TutorialQuantity,
                TutorialCostPrice,
                0,
                currentTradePanelVisible
            );
        }
    }


    //==================================================
    // CHARACTER DIALOGUE
    //==================================================

    IEnumerator ShowDialogueSequence(
        string speaker,
        AudioSource audioSource,
        AudioClip audioClip,
        string lineId,
        params string[] lines)
    {
        if (speakerNameText != null)
            speakerNameText.text = speaker + ":";

        AudioClip resolvedClip = ResolveVoiceClip(lineId, audioClip);
        AudioSource playbackSource = GetPlaybackSource(audioSource, resolvedClip);

        if (resolvedClip != null && playbackSource != null)
        {
            playbackSource.clip = resolvedClip;
            playbackSource.Play();
        }

        float clipLength = resolvedClip != null
            ? resolvedClip.length
            : lines.Length * 3f;

        float timePerLine = clipLength / lines.Length;

        foreach (string line in lines)
        {
            if (dialogueText != null)
                dialogueText.text = line;

            if (hudManager != null)
                hudManager.ShowSubtitle(speaker, line);

            yield return new WaitForSeconds(timePerLine);
        }

        while (playbackSource != null && playbackSource.isPlaying)
            yield return null;

        if (hudManager != null)
            hudManager.HideSubtitle();
    }


    IEnumerator ShowDialogueSequenceWithTimings(
        string speaker,
        AudioSource audioSource,
        AudioClip audioClip,
        string lineId,
        string[] lines,
        float[] startTimes)
    {
        if (speakerNameText != null)
            speakerNameText.text = speaker + ":";

        AudioClip resolvedClip = ResolveVoiceClip(lineId, audioClip);
        AudioSource playbackSource = GetPlaybackSource(audioSource, resolvedClip);

        if (resolvedClip != null && playbackSource != null)
        {
            playbackSource.clip = resolvedClip;
            playbackSource.Play();
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (playbackSource != null &&
                resolvedClip != null &&
                playbackSource.isPlaying)
            {
                yield return new WaitUntil(() =>
                    playbackSource.time >= startTimes[i] ||
                    !playbackSource.isPlaying
                );
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }

            if (dialogueText != null)
                dialogueText.text = lines[i];

            if (hudManager != null)
                hudManager.ShowSubtitle(speaker, lines[i]);
        }

        while (playbackSource != null && playbackSource.isPlaying)
            yield return null;

        if (hudManager != null)
            hudManager.HideSubtitle();
    }



    //==================================================
    // MAIN TUTORIAL SEQUENCE
    //==================================================

    IEnumerator TutorialSequence()
    {
        //--------------------------------------------------
        // RAHIM ARRIVES
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequenceWithTimings(
                "Rahim",
                customerAudioSource,
                rahimIntroClip,
                RahimIntroLineId,

                new string[]
                {
                    "Greetings, merchant!",
                    "My name is Rahim.",
                    "I have journeyed here from the Deccan Sultanate to trade in the markets of Vijayanagara.",
                    "I am looking to purchase one veesai of cardamom today, if the price is fair."
                },

                new float[]
                {
                    0.0f,
                    1.8f,
                    3.6f,
                    8.8f
                }
            )
        );


        //--------------------------------------------------
        // CURRENT TRADE PANEL TUTORIAL
        //--------------------------------------------------

        yield return StartCoroutine(TeachCurrentTradePanel());


        //--------------------------------------------------
        // STALL OWNER EXPLAINS TRADING
        //--------------------------------------------------

        yield return StartCoroutine(
    MerchantExplainNegotiationSequence()
);


        //--------------------------------------------------
        // STALL OWNER ASKS PLAYER TO TRY HIGH PRICE
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantAskHighPriceClip,
                BhaskaraAskHighPriceLineId,

                "Now, you must decide what to charge Rahim.",
                "For your first attempt, ask for 70 Varahas.",
                "Watch carefully how the customer responds."
            )
        );


        //--------------------------------------------------
        // CONTROL INSTRUCTIONS
        //--------------------------------------------------

        PromptManager.Instance.ShowPrompt(
            "Hold Left Trigger and say '70 Varahas'",
            PromptManager.Instance.leftTriggerButton
        );

        


        //--------------------------------------------------
        // START VOICE RECOGNITION
        //--------------------------------------------------

        voiceRecognitionManager.ListenForPrice("70 Varahas");

        waitingForHighPrice = true;
        
    }


    //==================================================
    // CURRENT TRADE PANEL TUTORIAL
    //==================================================

    private IEnumerator TeachCurrentTradePanel()
    {
        //--------------------------------------------------
        // MERCHANT EXPLAINS WHY PLAYER SHOULD CHECK TRADE
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantTeachTradeClip,
                BhaskaraTeachTradeLineId,

                "Before you answer, take a moment to study the trade.",
                "A wise merchant always knows what the customer wants and what his goods have cost him."
            )
        );


        //--------------------------------------------------
        // NARRATOR EXPLAINS BUTTON
        //--------------------------------------------------

        PromptManager.Instance.ShowPrompt(
            "Press Y to open the Current Trade panel.",
            PromptManager.Instance.yButton
        );

        yield return new WaitUntil(() => currentTradePanelOpened);
        PromptManager.Instance.HidePrompt();

        //--------------------------------------------------
        // MERCHANT EXPLAINS THE PANEL
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantExplainTradePanelClip,
                BhaskaraExplainTradePanelLineId,

                "There. This will help you keep track of the trade.",
                "You can see the customer's request and the cost of the spice here.",
                "Check it whenever you need before making an offer."
            )
        );
    }


    //==================================================
    // PLAYER OFFER HANDLING
    //==================================================

    public void HandlePlayerOffer(int offer)
    {
        if (tutorialFinished)
            return;

        if (hudManager != null)
        {
            hudManager.SetTutorialTrade(
                TutorialBuyer,
                TutorialSpice,
                TutorialQuantity,
                TutorialCostPrice,
                offer,
                currentTradePanelVisible
            );
        }

        if (spokenPriceText != null)
            spokenPriceText.text =
                "Spoken Price: " + offer + " Varahas";

        if (waitingForHighPrice)
        {
            HandleHighPriceStage(offer);
            return;
        }

        if (waitingForFairPrice)
        {
            HandleFairPriceStage(offer);
            return;
        }
    }


    //==================================================
    // HIGH PRICE STAGE
    //==================================================

    void HandleHighPriceStage(int offer)
    {
        if (offer >= 60)
        {
            PromptManager.Instance.HidePrompt();
            waitingForHighPrice = false;

            ChangeRespect(-40);

            StartCoroutine(
                HighPriceReactionSequence(offer)
            );
        }
        else
        {
            PromptManager.Instance.ShowPrompt(
                "Hold Left Trigger and say '70 Varahas'",
                PromptManager.Instance.leftTriggerButton
            );
            voiceRecognitionManager.ListenForPrice(
                "70"
            );

            waitingForHighPrice = true;
        }
    }


    IEnumerator HighPriceReactionSequence(int offer)
    {
        //--------------------------------------------------
        // RAHIM REACTS
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Rahim",
                customerAudioSource,
                rahimAngryHighPriceClip,
                RahimAngryHighPriceLineId,

                "That price is outrageous, merchant.",
                "Surely you can offer something more reasonable."
            )
        );


        //--------------------------------------------------
        // STALL OWNER EXPLAINS THE CONSEQUENCE
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantHighPriceLessonClip,
                BhaskaraHighPriceLessonLineId,

                "You see? Push a customer too far and you may lose his trust.",
                "And look. Your Reputation has suffered.",
                "Word travels quickly through the markets of Vijayanagara.",
                "If traders believe you are unfair, fewer customers will choose to do business with you.",
                "Profit matters, but so does the trust of those who trade with us.",
                "Try again. This time, offer Rahim a fairer price while still earning a profit."
            )
        );


        //--------------------------------------------------
        // LISTEN FOR FAIR PRICE
        //--------------------------------------------------

        voiceRecognitionManager.ListenForPrice(
            "a fair price"
        );

        waitingForFairPrice = true;
    }


    //==================================================
    // FAIR PRICE STAGE
    //==================================================

    void HandleFairPriceStage(int offer)
    {
        waitingForFairPrice = false;

        if (offer >= 22 && offer <= 30)
        {
            StartCoroutine(
                FairPriceSequence(offer)
            );
        }
        else if (offer > 30)
        {
            ChangeRespect(-20);

            StartCoroutine(
                TooHighAgainSequence(offer)
            );
        }
        else if (offer < 18)
        {
            coins += offer;
            UpdateMoneyUI();

            StartCoroutine(
                TooLowSequence(offer)
            );
        }
        else
        {
            StartCoroutine(
                FairPriceSequence(offer)
            );
        }
    }


    //==================================================
    // FAIR PRICE SUCCESS
    //==================================================

    IEnumerator FairPriceSequence(int offer)
    {
        //--------------------------------------------------
        // RAHIM ACCEPTS
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Rahim",
                customerAudioSource,
                rahimAcceptFairPriceClip,
                RahimAcceptFairPriceLineId,

                "Hmm... that seems much more reasonable.",
                "Very well. I accept your offer."
            )
        );


        //--------------------------------------------------
        // TRANSACTION VALUES CHANGE
        //--------------------------------------------------

        AddMoney(offer);
        ChangeRespect(+20);

        yield return new WaitForSeconds(0.75f);


        //--------------------------------------------------
        // STALL OWNER CONCLUDES TUTORIAL
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantFairPriceEndingClip,
                BhaskaraFairPriceEndingLineId,

                "Well done.",
                "Your Reputation has improved, and you have earned Varahas from the trade.",
                "Remember what you have learned.",
                "Ask too much, and you may lose the customer.",
                "Ask too little, and there is hardly a profit to be made.",
                "A good merchant builds wealth. A great merchant builds trust as well.",
                "Learn to balance both, and you will do well in this market."
            )
        );

        FinishTutorial();
    }


    //==================================================
    // PRICE STILL TOO HIGH
    //==================================================

    IEnumerator TooHighAgainSequence(int offer)
    {
        //--------------------------------------------------
        // RAHIM REACTS
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Rahim",
                customerAudioSource,
                rahimStillTooHighClip,
                RahimStillTooHighLineId,

                "That price is still too expensive.",
                "At those rates, I may take my business elsewhere."
            )
        );


        //--------------------------------------------------
        // STALL OWNER GIVES ADVICE
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantStillTooHighLessonClip,
                BhaskaraStillTooHighLessonLineId,

                "Still too high.",
                "Notice how your Reputation continues to fall.",
                "Even a wealthy merchant cannot prosper without the trust of his customers.",
                "Think about the 18 Varahas the cardamom cost us.",
                "Try proposing a price closer to 25 Varahas."
            )
        );


        //--------------------------------------------------
        // TRY AGAIN
        //--------------------------------------------------

        voiceRecognitionManager.ListenForPrice(
            "around 25 Varahas"
        );

        waitingForFairPrice = true;
    }


    //==================================================
    // PRICE TOO LOW
    //==================================================

    IEnumerator TooLowSequence(int offer)
    {
        //--------------------------------------------------
        // RAHIM ACCEPTS
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                "Rahim",
                customerAudioSource,
                rahimAcceptLowPriceClip,
                RahimAcceptLowPriceLineId,

                "That is a very generous offer.",
                "I happily accept your price."
            )
        );


        //--------------------------------------------------
        // TRANSACTION VALUES CHANGE
        //--------------------------------------------------

        AddMoney(offer);
        ChangeRespect(+30);

        yield return new WaitForSeconds(0.75f);


        //--------------------------------------------------
        // STALL OWNER EXPLAINS LOW PROFIT
        //--------------------------------------------------

        yield return StartCoroutine(
            ShowDialogueSequence(
                TutorialMerchantSpeaker,
                merchantAudioSource,
                merchantLowProfitLessonClip,
                BhaskaraLowProfitLessonLineId,

                "Rahim is certainly pleased.",
                "But look at what you have earned from this trade.",
                "A merchant who constantly sells below value may gain customers, but he will struggle to build wealth.",
                "You must satisfy your customers without sacrificing every chance of profit.",
                "To thrive in these markets, learn to balance both."
            )
        );

        FinishTutorial();
    }


    //==================================================
    // FINISH TUTORIAL
    //==================================================

    void FinishTutorial()
    {
        tutorialFinished = true;

        Debug.Log(
            "TRANSACTION TUTORIAL FINISHED"
        );

        Debug.Log(
            "[SCENE FLOW] Transaction complete -> CoinScene"
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
    }


    //==================================================
    // VALUE CHANGES
    //==================================================

    private void ChangeRespect(int amount)
    {
        respect = Mathf.Clamp(
            respect + amount,
            0,
            100
        );

        UpdateRespectUI();
    }


    private void AddMoney(int amount)
    {
        int oldCoins = coins;

        coins += amount;

        Debug.Log(
            $"[TUTORIAL MONEY] {oldCoins} -> {coins} | Earned: {amount}"
        );

        UpdateMoneyUI();
    }

    //==================================================
    // MONEY UI
    //==================================================

    private void UpdateMoneyUI()
    {
        if (hudManager != null)
            hudManager.UpdateMoney(coins);

        if (coinsEarnedText != null)
            coinsEarnedText.text = coins.ToString();
    }


    //==================================================
    // RESPECT UI
    //==================================================

    private void UpdateRespectUI()
    {
        if (hudManager != null)
            hudManager.UpdateRespect(respect);

        if (respectUIManager != null)
            respectUIManager.SetRespect(respect);
    }
    IEnumerator MerchantExplainNegotiationSequence()
    {
        string speaker = TutorialMerchantSpeaker;

        string[] lines =
        {
        "Good. Rahim wants one veesai of cardamom.",

        "The cardamom has cost us 18 Varahas. Keep that in mind when naming your price.",

        "To your right, you can see the Varahas you earn from each successful trade.",

        "Beside it is your Reputation in the market.",

        "A merchant must earn a profit, but the trust of his customers matters just as much.",

        "Treat people unfairly, and word will travel quickly through these markets."
    };


        //==================================================
        // AUDIO TIMINGS
        //==================================================

        float[] startTimes =
        {
        0.0f,   // Good. Rahim wants...
        3.0f,   // The cardamom has cost...
        8.0f,   // To your right...
        13.0f,  // Beside it is your Reputation...
        17.0f,  // A merchant must earn...
        22.0f   // Treat people unfairly...
    };


        //==================================================
        // SET SPEAKER
        //==================================================

        if (speakerNameText != null)
            speakerNameText.text = speaker + ":";


        //==================================================
        // PLAY MERCHANT AUDIO
        //==================================================

        AudioClip resolvedClip = ResolveVoiceClip(BhaskaraExplainNegotiationLineId, merchantExplainNegotiationClip);
        AudioSource playbackSource = GetPlaybackSource(merchantAudioSource, resolvedClip);

        if (resolvedClip != null &&
            playbackSource != null)
        {
            playbackSource.clip =
                resolvedClip;

            playbackSource.Play();
        }


        //==================================================
        // SHOW EACH LINE AT ITS AUDIO TIMESTAMP
        //==================================================

        for (int i = 0; i < lines.Length; i++)
        {
            //--------------------------------------------------
            // WAIT FOR CORRECT AUDIO TIME
            //--------------------------------------------------

            if (resolvedClip != null &&
                playbackSource != null &&
                playbackSource.isPlaying)
            {
                yield return new WaitUntil(() =>
                    playbackSource.time >= startTimes[i] ||
                    !playbackSource.isPlaying
                );
            }
            else
            {
                //--------------------------------------------------
                // FALLBACK WHEN NO AUDIO IS ASSIGNED
                //--------------------------------------------------

                if (i > 0)
                    yield return new WaitForSeconds(3f);
            }


            //--------------------------------------------------
            // SHOW SUBTITLE
            //--------------------------------------------------

            ShowTutorialDialogue(
                speaker,
                lines[i]
            );


            //--------------------------------------------------
            // TRIGGER PANEL PULSE
            //--------------------------------------------------

            if (i == 2)
            {
                // "To your right, you can see the Varahas..."
                if (hudManager != null)
                    hudManager.PulseMoneyPanel(3f);
            }
            else if (i == 3)
            {
                // "Beside it is your Reputation..."
                if (hudManager != null)
                    hudManager.PulseRespectPanel(3f);
            }
        }


        //==================================================
        // WAIT FOR AUDIO TO FINISH
        //==================================================

        while (playbackSource != null &&
               playbackSource.isPlaying)
        {
            yield return null;
        }


        //==================================================
        // FALLBACK WAIT IF THERE IS NO AUDIO
        //==================================================

        if (resolvedClip == null)
        {
            yield return new WaitForSeconds(3f);
        }


        //==================================================
        // HIDE SUBTITLE
        //==================================================

        if (hudManager != null)
            hudManager.HideSubtitle();
    }
    private void ShowTutorialDialogue(
    string speaker,
    string line)
    {
        if (dialogueText != null)
            dialogueText.text = line;

        if (hudManager != null)
        {
            hudManager.ShowSubtitle(
                speaker,
                line
            );
        }
    }

    private AudioClip ResolveVoiceClip(string lineId, AudioClip fallbackClip)
    {
        if (voiceDatabase == null)
            return fallbackClip;

        AudioClip databaseClip = voiceDatabase.GetAudioClip(lineId);
        return databaseClip != null ? databaseClip : fallbackClip;
    }

    private AudioSource GetPlaybackSource(AudioSource preferredSource, AudioClip clip)
    {
        if (preferredSource != null || clip == null)
            return preferredSource;

        if (fallbackAudioSource == null)
        {
            fallbackAudioSource = gameObject.AddComponent<AudioSource>();
            fallbackAudioSource.playOnAwake = false;
        }

        return fallbackAudioSource;
    }
}
