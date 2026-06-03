using UnityEngine;
using System.Collections;

public class CoinInteraction : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;

    [Header("Inspection")]
    public Transform inspectPoint;

    public Vector3 inspectScale =
        new Vector3(
            0.3548979f,
            0.02304457f,
            0.3307208f
        );

    public float transitionDuration = 1.2f;


    private bool taken = false;
    private Collider coinCollider;


    void Awake()
    {
        coinCollider = GetComponent<Collider>();

        VRInspectRotate rotate =
            GetComponent<VRInspectRotate>();

        if (rotate != null)
        {
            rotate.enabled = false;
        }

        Debug.Log("CoinInteraction Ready");
    }


    void Update()
    {
        // RIGHT INDEX TRIGGER
        float triggerValue =
            OVRInput.Get(
                OVRInput.Axis1D.SecondaryIndexTrigger
            );


        if (triggerValue > 0.5f)
        {
            Debug.Log("RIGHT TRIGGER DETECTED");

            TakeCoin();
        }
    }


    public void TakeCoin()
    {
        if (taken)
            return;


        Debug.Log("Coin Taken");


        taken = true;


        // Remove from NPC hand
        transform.SetParent(null);


        // Let NPC finish animation
        if (npc != null)
        {
            npc.ResumeAnimation();
        }
        else
        {
            Debug.LogWarning("NPC missing");
        }


        if (coinCollider != null)
        {
            coinCollider.enabled = false;
        }


        StartCoroutine(
            MoveToInspectMode()
        );
    }


    IEnumerator MoveToInspectMode()
    {
        Vector3 startPos =
            transform.position;


        Quaternion startRot =
            transform.rotation;


        Vector3 startScale =
            transform.localScale;


        float elapsed = 0f;


        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;


            float t =
                elapsed / transitionDuration;


            t = Mathf.SmoothStep(
                0,
                1,
                t
            );


            Vector3 targetPos =
                Vector3.Lerp(
                    startPos,
                    inspectPoint.position,
                    t
                );


            // small floating arc
            targetPos +=
                Vector3.up *
                Mathf.Sin(t * Mathf.PI)
                * 0.15f;


            transform.position =
                targetPos;


            transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    inspectPoint.rotation,
                    t
                );


            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    inspectScale,
                    t
                );


            yield return null;
        }


        // final snap
        transform.position =
            inspectPoint.position;


        transform.rotation =
            inspectPoint.rotation;


        transform.localScale =
            inspectScale;


        if (coinCollider != null)
        {
            coinCollider.enabled = true;
        }


        VRInspectRotate rotate =
            GetComponent<VRInspectRotate>();


        if (rotate != null)
        {
            rotate.enabled = true;

            Debug.Log(
                "VR Inspect Enabled"
            );
        }
        else
        {
            Debug.LogError(
                "NO VRInspectRotate FOUND"
            );
        }


        Debug.Log(
            "Inspect Mode Started"
        );
    }
}