using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class GateIntroFlow : MonoBehaviour
{
    [SerializeField] private float startupDelay = 1f;

    [Header("Look Around Tutorial")]
    [SerializeField] private string lookAroundTitle = "Prompt";
    [SerializeField]
    private string lookAroundBody =
        "Use the Left Joystick to look at your surroundings.";

    [Header("Continue Tutorial")]
    [SerializeField] private string continueTitle = "Prompt";
    [SerializeField]
    private string continueBody =
        "Press the Right Trigger to continue.";

    [Header("Objective")]
    [SerializeField] private string objectiveText = "Look around";

    [Header("Narration")]
    [SerializeField] private string narratorName = "Narrator (V.O.)";
    [SerializeField]
    private string narratorLine =
        "You seem to be lost, traveler. Let me be your guide. Welcome to Hampi, the roaring heart of the Vijayanagara Empire.";

    [SerializeField] private AudioClip narratorAudio;

    [Header("Timing")]
    [SerializeField] private float continuePromptDelay = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private bool canContinue = false;
    private bool triggerWasReleased = true;

    private IEnumerator Start()
    {
        // Initial delay
        yield return new WaitForSecondsRealtime(startupDelay);

        // Objective
        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(objectiveText);
        }

        // Show LEFT JOYSTICK tutorial
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowLeftJoystickPrompt(
                lookAroundTitle,
                lookAroundBody
            );
        }

        // Start narration audio
        if (audioSource != null && narratorAudio != null)
        {
            audioSource.clip = narratorAudio;
            audioSource.Play();
        }

        // Start narration subtitle
        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                narratorName,
                narratorLine
            );
        }

        // Make sure audio has finished
        if (audioSource != null && audioSource.isPlaying)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        // Wait a few seconds after narration
        yield return new WaitForSecondsRealtime(continuePromptDelay);

        // Change tutorial to RIGHT TRIGGER
        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowRightTriggerPrompt(
                continueTitle,
                continueBody
            );
        }

        canContinue = true;
    }

    private void Update()
    {
        if (!canContinue)
            return;

        bool triggerPressed = IsRightTriggerPressed();

        // Require the player to release the trigger first.
        if (!triggerPressed)
        {
            triggerWasReleased = true;
            return;
        }

        // New right-trigger press
        if (triggerWasReleased)
        {
            triggerWasReleased = false;
            canContinue = false;

            if (TutorialPromptUIManager.Instance != null)
            {
                TutorialPromptUIManager.Instance.HidePrompt();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextScene();
            }
        }
    }

    private bool IsRightTriggerPressed()
    {
        InputDevice rightHand =
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.isValid &&
            rightHand.TryGetFeatureValue(
                CommonUsages.triggerButton,
                out bool pressed))
        {
            return pressed;
        }

        return false;
    }
}