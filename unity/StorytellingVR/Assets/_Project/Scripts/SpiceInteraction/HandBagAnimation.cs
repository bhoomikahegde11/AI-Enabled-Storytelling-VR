using UnityEngine;
using System.Collections;

public class HandBagAnimation : MonoBehaviour
{
    private Animator animator;
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

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (handBag != null)
            handBag.SetActive(false);
    }

    void Start()
    {

            originalBagPos = handBag.transform.position;
            originalBagRot = handBag.transform.rotation;

            StartCoroutine(StartOrderRoutine());
    }

    public void StartOrder()
    {
        if (bagReceiver != null)
            bagReceiver.ResetBag();

        if (subtitleCanvas != null)
            subtitleCanvas.SetActive(true);

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyCustomerHandedBag();

        GiveHandBag();
    }

    public void GiveHandBag()
    {
        Debug.Log("GiveHandBag called");

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
}
