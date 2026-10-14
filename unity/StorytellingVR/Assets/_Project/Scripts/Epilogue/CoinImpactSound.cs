using System.Collections;
using UnityEngine;

public class CoinImpactSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip impactClip;

    [Header("Final Resting Position")]
    [SerializeField] private Transform landingTarget;

    [Tooltip("Small delay after impact before settling.")]
    [SerializeField] private float settleDelay = 0.18f;

    [Tooltip("How quickly the coin moves into its final pose.")]
    [SerializeField] private float settleDuration = 0.18f;

    private Rigidbody rb;

    private bool armed;
    private bool hasImpacted;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Required by ModernStudyRoomEndingController.
    public void Configure(
        AudioSource source,
        AudioClip clip
    )
    {
        audioSource = source;
        impactClip = clip;
    }

    // Optional overload in case we later want the controller
    // to provide the landing target too.
    public void Configure(
        AudioSource source,
        AudioClip clip,
        Transform target
    )
    {
        audioSource = source;
        impactClip = clip;
        landingTarget = target;
    }

    // Required by ModernStudyRoomEndingController.
    public void Arm()
    {
        armed = true;
        hasImpacted = false;
    }

    public void Disarm()
    {
        armed = false;
    }

    public void SetLandingTarget(Transform target)
    {
        landingTarget = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!armed || hasImpacted)
            return;

        hasImpacted = true;
        armed = false;

        // Play the clink on the real physics impact.
        if (audioSource != null && impactClip != null)
        {
            audioSource.PlayOneShot(impactClip);
        }

        // Prevent the coin from escaping sideways after impact,
        // while still allowing the initial spinning fall.
        if (rb != null)
        {
            rb.constraints |=
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ;
        }

        StartCoroutine(SettleIntoPlace());
    }

    private IEnumerator SettleIntoPlace()
    {
        yield return new WaitForSeconds(settleDelay);

        if (rb == null)
            yield break;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // If no landing target is assigned, just freeze
        // wherever the coin landed.
        if (landingTarget == null)
        {
            rb.isKinematic = true;
            yield break;
        }

        rb.isKinematic = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Vector3 finalPosition = landingTarget.position;
        Quaternion finalRotation = landingTarget.rotation;

        float elapsed = 0f;

        while (elapsed < settleDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / settleDuration
            );

            // Smoothly ease into the resting pose.
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(
                startPosition,
                finalPosition,
                t
            );

            transform.rotation = Quaternion.Slerp(
                startRotation,
                finalRotation,
                t
            );

            yield return null;
        }

        transform.position = finalPosition;
        transform.rotation = finalRotation;
    }
}