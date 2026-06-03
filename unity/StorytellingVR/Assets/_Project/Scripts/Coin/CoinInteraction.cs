using UnityEngine;
using System.Collections;

public class CoinInteraction : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;

    [Header("Inspection")]
    public Transform inspectPoint;

    [Header("Animation")]
    public float transitionDuration = 1.2f;

    public Vector3 inspectScale =
        new Vector3(
            0.3548979f,
            0.02304457f,
            0.3307208f
        );

    private bool taken = false;

    private Collider coinCollider;


    void Start()
    {
        coinCollider = GetComponent<Collider>();

        // Coin should not be rotatable while in NPC hand
        InspectRotate inspectRotate = GetComponent<InspectRotate>();

        if (inspectRotate != null)
            inspectRotate.enabled = false;
    }


    public void TakeCoin()
    {
        if (taken) return;

        taken = true;

        // Detach from NPC hand
        transform.SetParent(null);

        // NPC lowers hand
        if (npc != null)
            npc.ResumeAnimation();

        // Disable collider while moving
        if (coinCollider != null)
            coinCollider.enabled = false;

        StartCoroutine(MoveToInspectMode());

        Debug.Log("Coin Taken");
    }


    IEnumerator MoveToInspectMode()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / transitionDuration;

            // Smooth easing
            t = Mathf.SmoothStep(0f, 1f, t);

            // Movement
            Vector3 pos =
                Vector3.Lerp(
                    startPos,
                    inspectPoint.position,
                    t
                );

            // Add a slight arc
            pos += Vector3.up *
                   Mathf.Sin(t * Mathf.PI) *
                   0.15f;

            transform.position = pos;

            // Rotate toward inspection orientation
            transform.rotation =
                Quaternion.Slerp(
                    startRot,
                    inspectPoint.rotation,
                    t
                );

            // Scale up while moving
            transform.localScale =
                Vector3.Lerp(
                    startScale,
                    inspectScale,
                    t
                );

            yield return null;
        }

        // Snap to final values
        transform.position = inspectPoint.position;
        transform.rotation = inspectPoint.rotation;
        transform.localScale = inspectScale;

        // Re-enable collider
        if (coinCollider != null)
            coinCollider.enabled = true;

        // Enable inspection rotation
        InspectRotate inspectRotate = GetComponent<InspectRotate>();

        if (inspectRotate != null)
            inspectRotate.enabled = true;
    }


    // TEMP TEST ONLY
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeCoin();
        }
    }
}