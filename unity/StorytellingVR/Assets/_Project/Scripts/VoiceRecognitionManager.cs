using UnityEngine;
using TMPro;
using Meta.WitAi;
using Meta.WitAi.Json;

public class VoiceRecognitionManager : MonoBehaviour
{
    public AppVoiceExperience voiceExperience;
    public TutorialManager tutorialManager;
    public TMP_Text spokenPriceText;

    void Start()
    {
        voiceExperience.VoiceEvents.OnResponse.AddListener(OnWitResponse);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            voiceExperience.Activate();
            Debug.Log("Listening...");
        }
    }

    void OnWitResponse(WitResponseNode response)
    {
        if (response == null)
        {
            tutorialManager.ShowNarratorMessage("Say a number.");
            return;
        }

        var entities = response["entities"]["wit$number:number"];

        if (entities == null || entities.Count == 0)
        {
            tutorialManager.ShowNarratorMessage("Say a number.");
            return;
        }

        int recognizedNumber = entities[0]["value"].AsInt;

        spokenPriceText.text = "Spoken Price: " + recognizedNumber + " Varahas";

        tutorialManager.HandlePlayerOffer(recognizedNumber);
    }
}