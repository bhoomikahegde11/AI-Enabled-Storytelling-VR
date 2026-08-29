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

    private bool canContinue;
    private bool triggerReleased = true;

    private IEnumerator Start()
    {
        // Wait when the scene starts
        yield return new WaitForSecondsRealtime(startupDelay);

        // Set objective
        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(
                objectiveText
            );
        }

        // ---------------------------------------------
        // SHOW LEFT JOYSTICK PROMPT
        // ---------------------------------------------

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowLeftJoystickPrompt(
                lookAroundTitle,
                lookAroundBody,
                this
            );
        }

        // ---------------------------------------------
        // PLAY AUDIO
        // ---------------------------------------------

        if (audioSource != null && narratorAudio != null)
        {
            audioSource.clip = narratorAudio;
            audioSource.Play();
        }

        // ---------------------------------------------
        // SHOW SUBTITLE
        // ---------------------------------------------

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                narratorName,
                narratorLine
            );
        }

        // Make sure the actual audio has finished.
        if (audioSource != null &&
            audioSource.isPlaying)
        {
            yield return new WaitWhile(
                () => audioSource.isPlaying
            );
        }

        // ---------------------------------------------
        // WAIT A LITTLE AFTER AUDIO
        // ---------------------------------------------

        yield return new WaitForSecondsRealtime(
            continuePromptDelay
        );

        // ---------------------------------------------
        // CHANGE LEFT JOYSTICK → RIGHT TRIGGER
        // ---------------------------------------------

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowRightTriggerPrompt(
                continueTitle,
                continueBody,
                this
            );
        }

        canContinue = true;

        // If trigger is currently held, wait for release first.
        triggerReleased = !IsRightTriggerPressed();
    }

    private void Update()
    {
        if (!canContinue)
            return;

        bool pressed = IsRightTriggerPressed();

        // Player must release the trigger first.
        if (!pressed)
        {
            triggerReleased = true;
            return;
        }

        // New trigger press
        if (triggerReleased)
        {
            triggerReleased = false;
            canContinue = false;

            if (TutorialPromptUIManager.Instance != null)
            {
                TutorialPromptUIManager.Instance.HidePrompt(this);
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

        if (!rightHand.isValid)
            return false;

        return rightHand.TryGetFeatureValue(
            CommonUsages.triggerButton,
            out bool pressed
        ) && pressed;
    }
}