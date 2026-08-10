using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class SpiceMerchantGuideSequence : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform merchant;
    [SerializeField] private string merchantObjectName = "SpiceMerchantGuide";
    [SerializeField] private Transform stallDestination;
    [SerializeField] private Transform guideStopSpot;
    [SerializeField] private GameObject stallEntryHotspot;
    [SerializeField] private GameObject fallbackTalkPrompt;
    [SerializeField] private GameObject teleportSystem;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent merchantAgent;
    [SerializeField] private float walkSpeed = 1.4f;
    [SerializeField] private float stoppingDistance = 0.35f;
    [SerializeField] private float hotspotClearanceDistance = 16f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float maxWalkTime = 20f;

    [Header("Animation")]
    [SerializeField] private Animator merchantAnimator;
    [SerializeField] private bool freezeAnimatorUntilMovement = false;
    [SerializeField] private bool disableAnimatorRootMotion = false;
    [SerializeField] private string walkingBool = "isWalking";
    [SerializeField] private string talkingBool = "isTalking";
    [SerializeField] private string walkingStateName = "Standard Walk";
    [SerializeField] private string idleStateName = "";
    [SerializeField] private string talkingTrigger = "";
    [SerializeField] private string signalTrigger = "Signal";

    [Header("Dialogue")]

    private string merchantName = "Spice Merchant";

    private string greetingLine =
        "Greetings, traveler! I am a spice merchant. I sell a variety of exotic spices from around the world.";

    private string playerLine =
        "Good day. I'm looking for work to earn some money. Is there anything I can help you with?";

    private string merchantReplyLine =
        "You're in luck! I could use an extra pair of hands at my spice stall today.";

    private string followLine =
        "Come with me. I'll show you where you'll be working.";

    private string stallArrivalLine =
        "Here we are! Come inside the stall, and I'll show you what needs to be done.";

    [Header("Timing")]
    [SerializeField] private float greetingDuration = 5f;
    [SerializeField] private float playerLineDuration = 5f;
    [SerializeField] private float replyDuration = 5f;
    [SerializeField] private float followLineDuration = 3f;
    private float stallArrivalLineDuration = 3f;
    [SerializeField] private float lineGap = 0.25f;

    [Header("Prompt Text")]
    [SerializeField] private string talkPromptTitle = "Interact";
    [SerializeField] private string talkPromptBody = "Press X to interact.";

    private bool playerInside;
    private bool buttonHeld;
    private bool sequenceStarted;
    private bool sequenceComplete;

    private bool stallArrivalDialoguePlayed = false;

    private float defaultAnimatorSpeed = 1f;

    private void Awake()
    {
        ResolveMissingReferences();

        if (fallbackTalkPrompt != null)
            fallbackTalkPrompt.SetActive(false);

        if (stallEntryHotspot != null)
            stallEntryHotspot.SetActive(false);

        ResolveMerchantComponents();
        PrepareMerchantForIntro();
    }

    private void ResolveMissingReferences()
    {
        if (merchant == null && !string.IsNullOrEmpty(merchantObjectName))
        {
            GameObject merchantObject = GameObject.Find(merchantObjectName);

            if (merchantObject != null)
                merchant = merchantObject.transform;
        }

        if (stallEntryHotspot == null)
        {
            GameObject hotspot = GameObject.Find("PlayerStall_Hotspot");

            if (hotspot != null)
                stallEntryHotspot = hotspot;
        }

        if (stallDestination == null && stallEntryHotspot != null)
            stallDestination = stallEntryHotspot.transform;

        if (merchant != null)
        {
            Transform stopSpot = merchant.transform.Find("GuideStopSpot");
            if (stopSpot != null)
                guideStopSpot = stopSpot.transform;
        }

        if (stallEntryHotspot != null)
        {
            StallEntryTrigger trigger = stallEntryHotspot.GetComponent<StallEntryTrigger>();
            if (trigger != null && trigger.promptCanvas != null)
            {
                trigger.promptCanvas.SetActive(false);
            }
        }
    }

    private void ResolveMerchantComponents()
    {
        if (merchantAgent == null && merchant != null)
            merchantAgent = merchant.GetComponent<NavMeshAgent>();

        if (merchantAnimator == null && merchant != null)
        {
            Animator[] animators = merchant.GetComponentsInChildren<Animator>(false);
            foreach (var anim in animators)
            {
                if (anim.runtimeAnimatorController != null)
                {
                    merchantAnimator = anim;
                    break;
                }
            }
            if (merchantAnimator == null)
                merchantAnimator = merchant.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (!playerInside || sequenceStarted || sequenceComplete)
            return;

        bool xPressed = GetXPressed();

        if (xPressed && !buttonHeld)
        {
            buttonHeld = true;
            StartCoroutine(RunSequence());
        }

        if (!xPressed)
            buttonHeld = false;
    }

    private IEnumerator RunSequence()
    {
        sequenceStarted = true;

        if (FreeRoamStoryManager.Instance != null)
            FreeRoamStoryManager.Instance.NotifyMerchantConversationStarted();

        if (merchantAnimator != null && freezeAnimatorUntilMovement)
            merchantAnimator.speed = defaultAnimatorSpeed;

        HideTalkPrompt();

        if (teleportSystem != null)
            teleportSystem.SetActive(false);

        // Merchant introduces himself
        yield return Say(
            merchantName,
            greetingLine,
            greetingDuration
        );

        // Player asks for work
        yield return Say(
            "You",
            playerLine,
            playerLineDuration
        );

        // Merchant offers work
        yield return Say(
            merchantName,
            merchantReplyLine,
            replyDuration
        );

        // Merchant signals player to follow

        yield return Say(
            merchantName,
            followLine
        );

        if (merchantAnimator != null)
        {
            Debug.Log(
                $"[MERCHANT ANIM] Starting walk. Animator={merchantAnimator}, " +
                $"enabled={merchantAnimator?.enabled}, " +
                $"controller={merchantAnimator?.runtimeAnimatorController}, " +
                $"parameter={walkingBool}"
            );

            merchantAnimator.SetBool(walkingBool, true);

            Debug.Log(
                $"[MERCHANT ANIM] isWalking after SetBool: " +
                $"{merchantAnimator.GetBool(walkingBool)}"
            );
        }

        // Allow player to follow merchant
        if (teleportSystem != null)
            teleportSystem.SetActive(true);

        if (FreeRoamStoryManager.Instance != null)
            FreeRoamStoryManager.Instance.NotifyMerchantStartedWalking();

        // Merchant walks to stall
        yield return MoveMerchantToStall();

        // Activate stall entry trigger
        if (stallEntryHotspot != null)
            stallEntryHotspot.SetActive(true);

        if (FreeRoamStoryManager.Instance != null)
            FreeRoamStoryManager.Instance.NotifyMerchantReachedStall();

        sequenceComplete = true;
    }

    public void PlayStallArrivalDialogue(System.Action onDialogueFinished)
    {
        if (stallArrivalDialoguePlayed)
        {
            onDialogueFinished?.Invoke();
            return;
        }

        stallArrivalDialoguePlayed = true;

        StartCoroutine(
            StallArrivalDialogueSequence(onDialogueFinished)
        );
    }

    private IEnumerator StallArrivalDialogueSequence(
        System.Action onDialogueFinished)
    {
        yield return Say(
            merchantName,
            stallArrivalLine,
            stallArrivalLineDuration
        );

        onDialogueFinished?.Invoke();
    }

    private IEnumerator Say(
        string speaker,
        string text,
        float duration = -1f)
    {
        if (NarratorUIManager.Instance != null)
        {
            yield return NarratorUIManager.Instance.PlayNarration(
                speaker,
                text
            );
        }
        else
        {
            Debug.Log($"[{speaker}] {text}");
            yield return new WaitForSecondsRealtime(2f);
        }

        if (lineGap > 0f)
        {
            yield return new WaitForSecondsRealtime(lineGap);
        }
    }

    private IEnumerator MoveMerchantToStall()
    {
        if (merchant == null || stallDestination == null)
            yield break;

        Vector3 stopPosition = GetMerchantStopPosition();

        if (merchantAgent != null
            && merchantAgent.enabled
            && merchantAgent.isOnNavMesh)
        {
            merchantAgent.stoppingDistance = stoppingDistance;
            merchantAgent.speed = walkSpeed;
            merchantAgent.isStopped = false;

            merchantAgent.SetDestination(stopPosition);

            float timer = 0f;

            while (
                (merchantAgent.pathPending
                || merchantAgent.remainingDistance > stoppingDistance)
                && timer < maxWalkTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            merchantAgent.isStopped = true;
        }
        else
        {
            float timer = 0f;

            while (
                Vector3.Distance(
                    merchant.position,
                    stopPosition
                ) > stoppingDistance
                && timer < maxWalkTime)
            {
                timer += Time.deltaTime;

                merchant.position = Vector3.MoveTowards(
                    merchant.position,
                    stopPosition,
                    walkSpeed * Time.deltaTime
                );

                Vector3 direction =
                    stopPosition - merchant.position;

                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation =
                        Quaternion.LookRotation(direction);

                    merchant.rotation = Quaternion.Slerp(
                        merchant.rotation,
                        targetRotation,
                        turnSpeed * Time.deltaTime
                    );
                }

                yield return null;
            }
        }

        merchant.position = new Vector3(
            stopPosition.x,
            merchant.position.y,
            stopPosition.z
        );

        FaceStall();
        
        if (merchantAnimator != null)
        {
            Debug.Log(
                $"[MERCHANT ANIM] Arrived. isWalking before reset: " +
                $"{merchantAnimator.GetBool(walkingBool)}"
            );
            merchantAnimator.SetBool(walkingBool, false);
        }
    }

    private Vector3 GetMerchantStopPosition()
    {
        if (guideStopSpot != null)
            return guideStopSpot.position;

        Vector3 offsetFromHotspot =
            merchant.position - stallDestination.position;

        offsetFromHotspot.y = 0f;

        if (offsetFromHotspot.sqrMagnitude < 0.001f)
        {
            offsetFromHotspot = -stallDestination.forward;
            offsetFromHotspot.y = 0f;
        }

        Vector3 stopPosition =
            stallDestination.position
            + offsetFromHotspot.normalized
            * hotspotClearanceDistance;

        stopPosition.y = merchant.position.y;

        return stopPosition;
    }

    private void FaceStall()
    {
        Vector3 lookDirection =
            stallDestination.position - merchant.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            merchant.rotation =
                Quaternion.LookRotation(lookDirection);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)
            || sequenceStarted
            || sequenceComplete)
            return;

        playerInside = true;

        ShowTalkPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = false;

        if (!sequenceStarted)
            HideTalkPrompt();
    }

    private void ShowTalkPrompt()
    {
        if (playerInside && !sequenceStarted)
        {
            TutorialPromptUIManager.Instance.ShowPrompt(
                talkPromptTitle,
                talkPromptBody,
                this
            );

            return;
        }

        if (fallbackTalkPrompt != null)
            fallbackTalkPrompt.SetActive(true);
    }

    private void HideTalkPrompt()
    {
        if (TutorialPromptUIManager.Instance != null)
            TutorialPromptUIManager.Instance.HidePrompt(this);

        if (fallbackTalkPrompt != null)
            fallbackTalkPrompt.SetActive(false);
    }

    private bool GetXPressed()
    {
        InputDevice leftHand =
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        return leftHand.TryGetFeatureValue(
            CommonUsages.primaryButton,
            out bool pressed
        ) && pressed;
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player")
            || other.transform.root.CompareTag("Player")
            || other.GetComponentInParent<CharacterController>() != null;
    }

    private void TriggerAnimation(string triggerName)
    {
        if (merchantAnimator == null
            || string.IsNullOrEmpty(triggerName))
            return;

        if (!HasAnimatorParameter(
            triggerName,
            AnimatorControllerParameterType.Trigger))
            return;

        merchantAnimator.SetTrigger(triggerName);
    }

    private void SetWalking(bool isWalking)
    {
        if (merchantAnimator == null)
            return;

        if (freezeAnimatorUntilMovement)
        {
            merchantAnimator.speed =
                isWalking ? defaultAnimatorSpeed : 0f;
        }

        if (!string.IsNullOrEmpty(walkingBool)
            && HasAnimatorParameter(
                walkingBool,
                AnimatorControllerParameterType.Bool))
        {
            merchantAnimator.SetBool(
                walkingBool,
                isWalking
            );

            return;
        }

        string stateName =
            isWalking ? walkingStateName : idleStateName;

        if (!string.IsNullOrEmpty(stateName))
        {
            merchantAnimator.CrossFade(
                stateName,
                0.1f
            );
        }
    }

    private void SetTalking(bool isTalking)
    {
        if (merchantAnimator == null
            || string.IsNullOrEmpty(talkingBool))
            return;

        if (HasAnimatorParameter(
            talkingBool,
            AnimatorControllerParameterType.Bool))
        {
            merchantAnimator.SetBool(
                talkingBool,
                isTalking
            );
        }
    }

    private bool HasAnimatorParameter(
        string parameterName,
        AnimatorControllerParameterType parameterType)
    {
        foreach (
            AnimatorControllerParameter parameter
            in merchantAnimator.parameters)
        {
            if (parameter.name == parameterName
                && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }

    private void PrepareMerchantForIntro()
    {
        if (merchantAgent != null
            && merchantAgent.enabled
            && merchantAgent.isOnNavMesh)
        {
            merchantAgent.isStopped = true;
            merchantAgent.ResetPath();
        }

        if (merchantAnimator == null)
            return;

        merchantAnimator.enabled = true;
        merchantAnimator.SetBool(walkingBool, false);
    }
}