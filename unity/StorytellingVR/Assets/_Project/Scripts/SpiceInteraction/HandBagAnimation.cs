using UnityEngine;
using System.Collections;

public class HandBagAnimation : MonoBehaviour
{
    private const string MarketplaceHandoffTargetName = "CustomerHandTarget";

    private Animator animator;
    private Renderer[] actorRenderers;
    public BagReceiver bagReceiver;
    public GameObject subtitleCanvas;

    [Header("Bag")]
    public GameObject handBag;

    public Transform bagFillPosition;

    [Header("Tutorial Timing")]
    public float initialOrderDelay = 2f;
    public float bagMoveDuration = 1.2f;

    private Vector3 originalBagPos;
    private Quaternion originalBagRot;
    public SpiceVisualSet[] spiceVisuals;
    private Transform marketplaceHandoffOrigin;
    private bool useMarketplaceCustomerVisuals;
    private Coroutine bagMoveCoroutine;
    private Coroutine bagFillCoroutine;
    public bool UsesMarketplaceCustomerVisuals => useMarketplaceCustomerVisuals;
    private TradeTermsSnapshot marketplaceAgreement;

    public void BindMarketplaceAgreement(TradeTermsSnapshot agreement)
    {
        marketplaceAgreement = agreement;
    }

    private bool CanReceiveHandoffEvent()
    {
        if (OrderManager.Instance != null && OrderManager.Instance.tutorialMode) return true;
        return marketplaceAgreement != null && OrderManager.Instance != null &&
            OrderManager.Instance.MarketplaceOrder != null &&
            OrderManager.Instance.MarketplaceOrder.BelongsTo(Level1GameState.ExistingInstance?.ActiveTrade, marketplaceAgreement);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        actorRenderers = GetComponentsInChildren<Renderer>(true);

        if (handBag != null)
        {
            handBag.SetActive(false);
        }
    }

    void Start()
    {
        if (handBag == null) return;
        if (!useMarketplaceCustomerVisuals)
        {
            originalBagPos = handBag.transform.position;
            originalBagRot = handBag.transform.rotation;
        }

        if (OrderManager.Instance != null && OrderManager.Instance.tutorialMode)
        {
            StartCoroutine(StartOrderRoutine());
        }
    }

    public void StartOrder()
    {
        ResetHandoffState();

        if (bagReceiver != null)
            bagReceiver.ResetBag();

        bool tutorialModeActive = OrderManager.Instance != null && OrderManager.Instance.tutorialMode;

        if (subtitleCanvas != null &&
            tutorialModeActive)
            subtitleCanvas.SetActive(true);

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyCustomerHandedBag();

        if (!tutorialModeActive)
        {
            if (CanPlayMarketplaceHandoffAnimation())
            {
                GiveHandBag();
                return;
            }

            ShowMarketplaceBagOnly();
            return;
        }

        GiveHandBag();
    }

    public void GiveHandBag()
    {
        Debug.Log("GiveHandBag called");

        SetActorVisualsVisible(true);

        if (animator == null)
        {
            ShowMarketplaceBagOnly();
            return;
        }

        animator.speed = 1f;
        animator.SetTrigger("GiveCoin");
    }

    public void FreezeHand()
    {
        if (!CanReceiveHandoffEvent()) return;
        Debug.Log("Freeze called");

        if (animator != null)
        {
            animator.speed = 0f;
        }

        handBag.SetActive(true);

        StartBagMoveCoroutine(MoveBagForward());
    }

    IEnumerator MoveBagForward()
    {
        Vector3 startPos = handBag.transform.position;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, bagMoveDuration);

            handBag.transform.position =
                Vector3.Lerp(
                    startPos,
                    bagFillPosition.position,
                    t
                );

            yield return null;
        }
    }

    public void ResumeAnimation()
    {
        SetActorVisualsVisible(true);
        if (animator != null)
        {
            animator.speed = 1f;
        }
    }
    public void ReceiveBag()
    {
        if (!CanReceiveHandoffEvent()) return;
        StartBagMoveCoroutine(ReturnBag());
    }
    IEnumerator ReturnBag()
    {
        Vector3 startPos =
            handBag.transform.position;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, bagMoveDuration);

            handBag.transform.position =
                Vector3.Lerp(
                    startPos,
                    originalBagPos,
                    t
                );

            yield return null;
        }
        ShowBagSpice(SpiceType.None);
        handBag.SetActive(false);

        ResumeAnimation();
    }
    IEnumerator StartOrderRoutine()
    {
        yield return new WaitForSeconds(initialOrderDelay);

        StartOrder();
    }
    public void FillBag(SpiceType spice)
    {
        ShowBagSpice(spice);

        if (subtitleCanvas != null &&
            (OrderManager.Instance == null || !OrderManager.Instance.tutorialMode))
        {
            subtitleCanvas.SetActive(false);
        }

        if (bagFillCoroutine != null)
        {
            StopCoroutine(bagFillCoroutine);
        }

        bagFillCoroutine = StartCoroutine(FillAndReturn());
    }
    IEnumerator FillAndReturn()
    {
        yield return new WaitForSeconds(0.5f);

        bagFillCoroutine = null;
        StartBagMoveCoroutine(ReturnBag());
    }
    void ShowBagSpice(SpiceType spice)
    {
        if (spiceVisuals == null) return;
        foreach (SpiceVisualSet item in spiceVisuals)
        {
            if (item == null || item.visual == null) continue;
            if (spice == SpiceType.None)
            {
                item.visual.SetActive(false);
            }
            else
            {
                item.visual.SetActive(item.spiceType == spice);
            }
        }
    }

    private void ShowMarketplaceBagOnly()
    {
        if (handBag == null || bagFillPosition == null)
        {
            return;
        }

        if (useMarketplaceCustomerVisuals)
        {
            SetActorVisualsVisible(true);

            if (marketplaceHandoffOrigin != null)
            {
                handBag.transform.position = marketplaceHandoffOrigin.position;
                handBag.transform.rotation = marketplaceHandoffOrigin.rotation;
            }
            else
            {
                handBag.transform.position = transform.position;
                handBag.transform.rotation = transform.rotation;
            }

            handBag.SetActive(true);
            StartBagMoveCoroutine(MoveBagForward());
            return;
        }

        SetActorVisualsVisible(false);
        handBag.transform.position = bagFillPosition.position;
        handBag.transform.rotation = bagFillPosition.rotation;
        handBag.SetActive(true);
    }

    public void ConfigureMarketplaceCustomerHandoff(
        BagReceiver sharedBagReceiver,
        GameObject sharedSubtitleCanvas,
        GameObject sharedHandBag,
        Transform sharedBagFillPosition,
        SpiceVisualSet[] sharedSpiceVisuals,
        Vector3 localHandOffset)
    {
        bagReceiver = sharedBagReceiver;
        subtitleCanvas = sharedSubtitleCanvas;
        handBag = sharedHandBag;
        bagFillPosition = sharedBagFillPosition;
        spiceVisuals = sharedSpiceVisuals;
        useMarketplaceCustomerVisuals = true;

        marketplaceHandoffOrigin = transform.Find(MarketplaceHandoffTargetName);
        if (marketplaceHandoffOrigin == null)
        {
            GameObject target = new GameObject(MarketplaceHandoffTargetName);
            marketplaceHandoffOrigin = target.transform;
            marketplaceHandoffOrigin.SetParent(transform, false);
        }

        marketplaceHandoffOrigin.localPosition = localHandOffset;
        marketplaceHandoffOrigin.localRotation = Quaternion.identity;

        if (handBag != null)
        {
            handBag.SetActive(false);
            originalBagPos = marketplaceHandoffOrigin.position;
            originalBagRot = marketplaceHandoffOrigin.rotation;
            ShowBagSpice(SpiceType.None);
        }
    }

    public void ResetHandoffState()
    {
        marketplaceAgreement = null;
        if (bagMoveCoroutine != null)
        {
            StopCoroutine(bagMoveCoroutine);
            bagMoveCoroutine = null;
        }

        if (bagFillCoroutine != null)
        {
            StopCoroutine(bagFillCoroutine);
            bagFillCoroutine = null;
        }

        if (handBag != null)
        {
            handBag.transform.position = originalBagPos;
            handBag.transform.rotation = originalBagRot;
            handBag.SetActive(false);
        }

        ShowBagSpice(SpiceType.None);
        if (animator != null) animator.speed = 1f;
    }

    public void SetActorVisualsVisible(bool visible)
    {
        if (actorRenderers == null)
        {
            return;
        }

        foreach (Renderer renderer in actorRenderers)
        {
            if (renderer == null || handBag != null && renderer.gameObject == handBag)
            {
                continue;
            }

            renderer.enabled = visible;
        }
    }

    private bool CanPlayMarketplaceHandoffAnimation()
    {
        if (!useMarketplaceCustomerVisuals || animator == null || animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger &&
                parameter.name == "GiveCoin")
            {
                return true;
            }
        }

        return false;
    }

    private void StartBagMoveCoroutine(IEnumerator routine)
    {
        if (bagMoveCoroutine != null)
        {
            StopCoroutine(bagMoveCoroutine);
        }

        bagMoveCoroutine = StartCoroutine(RunBagMoveRoutine(routine));
    }

    private IEnumerator RunBagMoveRoutine(IEnumerator routine)
    {
        yield return routine; // Keep nested movement owned by the handle that reset cancels.
        bagMoveCoroutine = null;
    }
}
