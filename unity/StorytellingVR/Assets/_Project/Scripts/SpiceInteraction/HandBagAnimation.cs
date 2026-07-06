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
        originalBagPos = handBag.transform.position;
        originalBagRot = handBag.transform.rotation;

        if (OrderManager.Instance != null && OrderManager.Instance.tutorialMode)
        {
            StartCoroutine(StartOrderRoutine());
        }
    }

    public void StartOrder()
    {
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
            ShowMarketplaceBagOnly();
            return;
        }

        GiveHandBag();
    }

    public void GiveHandBag()
    {
        Debug.Log("GiveHandBag called");

        SetActorVisualsVisible(true);

        animator.speed = 1f;
        animator.SetTrigger("GiveCoin");
    }

    public void FreezeHand()
    {
        Debug.Log("Freeze called");

        animator.speed = 0f;

        handBag.SetActive(true);

        StartCoroutine(MoveBagForward());
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
        animator.speed = 1f;
    }
    public void ReceiveBag()
    {
        StartCoroutine(ReturnBag());
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

        StartCoroutine(FillAndReturn());
    }
    IEnumerator FillAndReturn()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ReturnBag());
    }
    void ShowBagSpice(SpiceType spice)
    {
        foreach (SpiceVisualSet item in spiceVisuals)
        {
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
            StartCoroutine(MoveBagForward());
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
}
