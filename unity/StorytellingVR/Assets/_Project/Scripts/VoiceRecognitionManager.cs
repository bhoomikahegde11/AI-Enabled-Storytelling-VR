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

    private bool waitingForInput = false;

    
    void Start()
{
        voicePromptText.text = "";
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

    public void ListenForPrice()
    {
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
            waitingForInput = false;

            spokenPriceText.text =
                "Spoken Price: " + number + " Varahas";
            voicePromptText.text = "";

            tutorialManager.HandlePlayerOffer(number);
        }
        else
        {
            voicePromptText.text = "Please say a number.";

            StartCoroutine(ClearPromptAfterDelay());

            StartCoroutine(RestartListening());
        }
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
            voicePromptText.text = "Speak now";
        else
            voicePromptText.text = "";
    }
}
