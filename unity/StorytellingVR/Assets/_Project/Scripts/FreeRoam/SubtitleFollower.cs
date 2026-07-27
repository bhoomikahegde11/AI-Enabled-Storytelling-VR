using UnityEngine;

public class SubtitleFollower : MonoBehaviour
{
    public enum SubtitlePositionMode
    {
        FollowPlayerGaze,
        FixedAnchor
    }

    [Header("References")]
    [SerializeField]
    private Transform head;

    [SerializeField]
    private Transform fixedAnchor;

    [Header("Current Mode")]
    [SerializeField]
    private SubtitlePositionMode currentMode =
        SubtitlePositionMode.FollowPlayerGaze;

    [Header("Follow Position")]
    [SerializeField]
    private float distance = 1.8f;

    [SerializeField]
    private float heightOffset = -0.25f;

    [Header("Follow Behaviour")]
    [Tooltip("Degrees the player can turn before the subtitle recenters.")]
    [SerializeField]
    private float recenterAngle = 25f;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float rotateSpeed = 8f;

    [Header("Fixed Position Behaviour")]
    [SerializeField]
    private bool smoothlyMoveToFixedAnchor = true;

    [SerializeField]
    private float fixedMoveSpeed = 8f;

    private Vector3 targetPosition;

    private void Start()
    {
        if (head == null)
        {
            Debug.LogError(
                "[SUBTITLE FOLLOWER] Head reference is missing."
            );

            enabled = false;
            return;
        }

        targetPosition = GetDesiredFollowPosition();
        transform.position = targetPosition;
    }

    private void LateUpdate()
    {
        switch (currentMode)
        {
            case SubtitlePositionMode.FollowPlayerGaze:
                UpdateFollowMode();
                break;

            case SubtitlePositionMode.FixedAnchor:
                UpdateFixedMode();
                break;
        }
    }

    private void UpdateFollowMode()
    {
        Vector3 desiredPosition =
            GetDesiredFollowPosition();

        Vector3 toSubtitle =
            transform.position - head.position;

        toSubtitle.y = 0f;

        Vector3 headForward = head.forward;
        headForward.y = 0f;

        if (toSubtitle.sqrMagnitude > 0.001f &&
            headForward.sqrMagnitude > 0.001f)
        {
            float angle = Vector3.Angle(
                headForward.normalized,
                toSubtitle.normalized
            );

            if (angle > recenterAngle)
            {
                targetPosition = Vector3.Lerp(
                    targetPosition,
                    desiredPosition,
                    Time.unscaledDeltaTime * moveSpeed
                );
            }
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.unscaledDeltaTime
        );

        FacePlayer();
    }

    private void UpdateFixedMode()
    {
        if (fixedAnchor == null)
            return;

        if (smoothlyMoveToFixedAnchor)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                fixedAnchor.position,
                fixedMoveSpeed * Time.unscaledDeltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                fixedAnchor.rotation,
                fixedMoveSpeed * Time.unscaledDeltaTime
            );
        }
        else
        {
            transform.SetPositionAndRotation(
                fixedAnchor.position,
                fixedAnchor.rotation
            );
        }
    }

    private Vector3 GetDesiredFollowPosition()
    {
        Vector3 forward = head.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f &&
            head.parent != null)
        {
            forward = head.parent.forward;
            forward.y = 0f;
        }

        forward.Normalize();

        Vector3 position =
            head.position + forward * distance;

        position.y =
            head.position.y + heightOffset;

        return position;
    }

    private void FacePlayer()
    {
        Vector3 lookDirection =
            transform.position - head.position;

        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.unscaledDeltaTime
        );
    }

    public void UseFollowMode()
    {
        currentMode =
            SubtitlePositionMode.FollowPlayerGaze;

        targetPosition =
            GetDesiredFollowPosition();

        Debug.Log(
            "[SUBTITLE FOLLOWER] Follow mode enabled."
        );
    }

    public void UseFixedMode()
    {
        if (fixedAnchor == null)
        {
            Debug.LogWarning(
                "[SUBTITLE FOLLOWER] Cannot use fixed mode. " +
                "Fixed Anchor is missing."
            );

            return;
        }

        currentMode =
            SubtitlePositionMode.FixedAnchor;

        Debug.Log(
            "[SUBTITLE FOLLOWER] Fixed mode enabled."
        );
    }

    public void SetFixedAnchor(Transform newAnchor)
    {
        fixedAnchor = newAnchor;
    }
}