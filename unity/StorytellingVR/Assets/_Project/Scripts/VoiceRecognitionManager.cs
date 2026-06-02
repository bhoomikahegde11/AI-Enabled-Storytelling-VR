using UnityEngine;
using TMPro;
using Oculus.Voice;
using System.Text.RegularExpressions;

public class VoiceRecognitionManager : MonoBehaviour
{
    public AppVoiceExperience voiceExperience;

    public TutorialManager tutorialManager;

    public TMP_Text spokenPriceText;

    private bool waitingForInput = false;

    
    void Start()
{
    Debug.Log("VOICE MANAGER STARTED");

    

    voiceExperience.VoiceEvents.OnStartListening.AddListener(() =>
    {
        Debug.Log("STARTED LISTENING");
    });

    voiceExperience.VoiceEvents.OnStoppedListening.AddListener(() =>
    {
        Debug.Log("STOPPED LISTENING");
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
        waitingForInput = true;

        Debug.Log("Listening for player price...");

        voiceExperience.Activate();
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

            tutorialManager.HandlePlayerOffer(number);
        }
        else
        {
            tutorialManager.ShowNarrator(
                "Please say a number."
            );

            voiceExperience.Activate();
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
}