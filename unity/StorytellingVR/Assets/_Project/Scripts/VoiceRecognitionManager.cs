using UnityEngine;
using TMPro;
using Oculus.Voice;
using System.Text.RegularExpressions;
using System.Collections;

public class VoiceRecognitionManager : MonoBehaviour
{
    public AppVoiceExperience voiceExperience;

    public TutorialManager tutorialManager;

    public TMP_Text spokenPriceText;

    public TMP_Text voicePromptText;
    [Tooltip("The live UI System HUD used for all tutorial prompts.")]
    public Level1HUDManager hudManager;

    [Header("Tutorial Controls")]
    [Tooltip("Match the game input flow: hold the left trigger to speak, then press A to submit the recognised offer.")]
    public bool requireTriggerAndConfirm = true;

    private bool waitingForInput = false;
    private bool isListeningForPrice = false;
    private bool hasPendingOffer = false;
    private int pendingOffer;
    private string offerHint = "a price";
    private Color promptBaseColor;

    
    void Start()
    {
        if (hudManager == null)
            hudManager = FindFirstObjectByType<Level1HUDManager>();

        if (voicePromptText != null)
        {
            voicePromptText.text = "";
            promptBaseColor = voicePromptText.color;
        }
        Debug.Log("VOICE MANAGER STARTED");

    

    voiceExperience.VoiceEvents.OnStartListening.AddListener(() =>
    {
        Debug.Log("STARTED LISTENING");
    });

    voiceExperience.VoiceEvents.OnStoppedListening.AddListener(() =>
    {
        Debug.Log("STOPPED LISTENING");
        StartCoroutine(ResetVoiceState());
    });

    voiceExperience.VoiceEvents.OnPartialTranscription.AddListener((text) =>
    {
        Debug.Log("PARTIAL: " + text);
    });

    voiceExperience.VoiceEvents.OnFullTranscription.AddListener((text) =>
    {
        Debug.Log("FULL: " + text);
        OnTranscription(text);
    });
    }

    private void Update()
    {
        if (!requireTriggerAndConfirm || !waitingForInput)
            return;

        PulsePrompt();

        if (hasPendingOffer)
        {
            if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmPendingOffer();
            }
            else if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.R))
            {
                DiscardPendingOffer();
            }
            return;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyDown(KeyCode.V))
        {
            PromptManager.Instance.HidePrompt();
            BeginListeningForPrice();
        }

        if (isListeningForPrice && (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKeyUp(KeyCode.V)))
        {
            isListeningForPrice = false;
            SetPrompt("Understanding your offer...");
            voiceExperience.Deactivate();
        }
    }

    public void ListenForPrice(string priceHint = "")
    {
        offerHint = string.IsNullOrWhiteSpace(priceHint) ? "a price" : priceHint;
        hasPendingOffer = false;
        pendingOffer = 0;

        if (requireTriggerAndConfirm)
        {
            waitingForInput = true;
            SetPrompt("Hold LEFT TRIGGER, say " + offerHint + ", then release.");
            return;
        }

        StartCoroutine(StartFreshListening());
    }

    void OnTranscription(string transcription)
    {
        Debug.Log("TRANSCRIPTION  WAS RECEIVED: " + transcription);
        if (!waitingForInput)
            return;

        Debug.Log("Player said: " + transcription);

        int number;

        if (TryExtractNumber(transcription, out number))
        {
            if (requireTriggerAndConfirm)
            {
                hasPendingOffer = true;
                pendingOffer = number;
                isListeningForPrice = false;
                if (spokenPriceText != null)
                    spokenPriceText.text = "Spoken Price: " + number + " Varahas";
                if (hudManager != null && hudManager.playerInput != null)
                    hudManager.playerInput.text = number + " Varahas";

                SetPrompt("You said " + number + ". Press A to confirm or B to try again.");
            }
            else
            {
                SubmitOffer(number);
            }
        }
        else
        {
            SetPrompt("I did not hear a price. Hold LEFT TRIGGER and try again.");

            StartCoroutine(ClearPromptAfterDelay());
            if (!requireTriggerAndConfirm)
                StartCoroutine(RestartListening());
        }
    }

    private void BeginListeningForPrice()
    {
        if (isListeningForPrice || voiceExperience == null)
            return;

        isListeningForPrice = true;
        SetPrompt("Listening... say " + offerHint + ".");
        voiceExperience.Activate();
    }

    private void ConfirmPendingOffer()
    {
        if (!hasPendingOffer)
            return;

        SubmitOffer(pendingOffer);
    }

    private void DiscardPendingOffer()
    {
        hasPendingOffer = false;
        pendingOffer = 0;
        if (spokenPriceText != null)
            spokenPriceText.text = "Spoken Price: --";
        if (hudManager != null && hudManager.playerInput != null)
            hudManager.playerInput.text = "";
        SetPrompt("Hold LEFT TRIGGER, say " + offerHint + ", then release.");
    }

    private void SubmitOffer(int offer)
    {
        waitingForInput = false;
        hasPendingOffer = false;
        pendingOffer = 0;
        SetPrompt("");

        if (spokenPriceText != null)
            spokenPriceText.text = "Spoken Price: " + offer + " Varahas";

        if (tutorialManager != null)
            tutorialManager.HandlePlayerOffer(offer);
    }

    private void SetPrompt(string message)
    {
        if (voicePromptText != null)
            voicePromptText.text = message;

        if (hudManager != null)
        {
            if (string.IsNullOrEmpty(message))
                hudManager.HidePlayerInputPanel();
            else
            {
                hudManager.ShowPlayerInputPanel();
                hudManager.SetVoiceStatus(message);
            }
        }
    }

    private void PulsePrompt()
    {
        if (voicePromptText == null)
            return;

        Color color = promptBaseColor;
        color.a = 0.7f + Mathf.Sin(Time.time * 4f) * 0.3f;
        voicePromptText.color = color;
    }

    bool TryExtractNumber(string text, out int number)
    {
        Match match = Regex.Match(text, @"\d+");

        if (match.Success)
        {
            number = int.Parse(match.Value);
            return true;
        }

        number = 0;
        return false;
    }
    IEnumerator RestartListening()
    {
        waitingForInput = false ;
        voiceExperience.Deactivate();
        yield return new WaitForSeconds(1.0f);
        waitingForInput = true;
        voiceExperience.Activate();
    }
    IEnumerator StartFreshListening()
    {
        voiceExperience.Deactivate();
        yield return new WaitForSeconds(0.5f);
        waitingForInput = true;
        Debug.Log("Listening for player price");
        voiceExperience.Activate();
    }
    IEnumerator ResetVoiceState()
    {
        yield return new WaitForSeconds(0.5f);
        voiceExperience.Deactivate();
    }
    IEnumerator ClearPromptAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (waitingForInput)
            SetPrompt(requireTriggerAndConfirm
                ? "Hold LEFT TRIGGER and try again."
                : "Speak now");
        else
            SetPrompt("");
    }
}
