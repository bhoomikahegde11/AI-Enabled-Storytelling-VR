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
    private string narratorLineId =
        "GATE_INTRO_NARRATOR_01";

    [SerializeField]
    private string narratorLine =
        "You seem to be lost, traveler. Let me be your guide. Welcome to Hampi, the roaring heart of the Vijayanagara Empire.";

    [Header("Timing")]
    [SerializeField] private float continuePromptDelay = 2f;

    private bool canContinue;
    private bool triggerReleased = true;

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(startupDelay);

        // ------------------------------------------------
        // OBJECTIVE
        // ------------------------------------------------

        if (ObjectiveUIManager.Instance != null)
        {
            ObjectiveUIManager.Instance.SetObjective(
                objectiveText
            );
        }

        // ------------------------------------------------
        // LOOK AROUND PROMPT
        // ------------------------------------------------

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowLeftJoystickPrompt(
                lookAroundTitle,
                lookAroundBody,
                this
            );
        }

        // ------------------------------------------------
        // NARRATION + VOICE
        // NarratorUIManager handles the voice database lookup.
        // ------------------------------------------------

        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                narratorLineId,
                narratorName,
                narratorLine
            );
        }
        else
        {
            Debug.LogWarning(
                "[GATE INTRO] NarratorUIManager.Instance is missing."
            );
        }

        // ------------------------------------------------
        // WAIT BEFORE CONTINUE PROMPT
        // ------------------------------------------------

        yield return new WaitForSecondsRealtime(
            continuePromptDelay
        );

        // ------------------------------------------------
        // CHANGE PROMPT TO RIGHT TRIGGER
        // ------------------------------------------------

        if (TutorialPromptUIManager.Instance != null)
        {
            TutorialPromptUIManager.Instance.ShowRightTriggerPrompt(
                continueTitle,
                continueBody,
                this
            );
        }

        canContinue = true;

        // Prevent a trigger press used during narration
        // from immediately dismissing this prompt.
        triggerReleased = !IsRightTriggerPressed();
    }

    private void Update()
    {
        if (!canContinue)
            return;

        bool pressed = IsRightTriggerPressed();

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

            Debug.Log(
                "[GATE INTRO] Intro completed."
            );

            Debug.Log("[GATE INTRO] Requesting next scene from GameManager.");
            GameManager.Instance.LoadNextScene();
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
