using UnityEngine;

public class SubtitleFollower : MonoBehaviour
{
    [Header("References")]
    public Transform head;   // Drag Main Camera (Center Eye Anchor) here

    [Header("Position")]
    public float distance = 1.8f;
    public float heightOffset = -0.25f;

    [Header("Behaviour")]
    [Tooltip("Degrees the player can turn before the subtitle recenters.")]
    public float recenterAngle = 25f;

    [Tooltip("How quickly the subtitle moves to its new position.")]
    public float moveSpeed = 5f;

    [Tooltip("How quickly the subtitle rotates to face the player.")]
    public float rotateSpeed = 8f;

    private Vector3 targetPosition;

    void Start()
    {
        if (head == null)
        {
            Debug.LogError("SubtitleFollower: Head reference is missing.");
            enabled = false;
            return;
        }

        targetPosition = GetDesiredPosition();
        transform.position = targetPosition;
    }

    void LateUpdate()
    {
        Vector3 desiredPosition = GetDesiredPosition();

        // Horizontal direction from head to subtitle
        Vector3 toSubtitle = transform.position - head.position;
        toSubtitle.y = 0f;

        Vector3 headForward = head.forward;
        headForward.y = 0f;

        if (toSubtitle.sqrMagnitude > 0.001f && headForward.sqrMagnitude > 0.001f)
        {
            float angle = Vector3.Angle(headForward.normalized, toSubtitle.normalized);

            // Only move if player has looked away enough
            if (angle > recenterAngle)
            {
                targetPosition = Vector3.Lerp(
                    targetPosition,
                    desiredPosition,
                    Time.deltaTime * moveSpeed
                );
            }
        }

        // Smooth movement
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Face the player
        Vector3 lookDirection = transform.position - head.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
    }

    Vector3 GetDesiredPosition()
    {
        Vector3 forward = head.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = head.parent.forward;

        forward.Normalize();

        Vector3 pos = head.position + forward * distance;
        pos.y = head.position.y + heightOffset;

        return pos;
    }
}