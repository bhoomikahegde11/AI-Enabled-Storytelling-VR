using UnityEngine;
using System.Collections;

public class HandBagAnimation : MonoBehaviour
{
    private Animator animator;

    public GameObject subtitleCanvas;

    [Header("Bag")]
    public GameObject handBag;

    public Transform bagFillPosition;

    private Vector3 originalBagPos;
    private Quaternion originalBagRot;
    public GameObject spiceVisual;

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
        subtitleCanvas.SetActive(true);

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
            t += Time.deltaTime;

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
            t += Time.deltaTime;

            handBag.transform.position =
                Vector3.Lerp(
                    startPos,
                    originalBagPos,
                    t
                );

            yield return null;
        }
        handBag.SetActive(false);

        spiceVisual.SetActive(false);

        ResumeAnimation();
    }
    IEnumerator StartOrderRoutine()
    {
        yield return new WaitForSeconds(2f);

        StartOrder();
    }
    public void FillBag()
    {
        spiceVisual.SetActive(true);

        subtitleCanvas.SetActive(false);

        StartCoroutine(FillAndReturn());
    }
    IEnumerator FillAndReturn()
    {
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(ReturnBag());
    }

}