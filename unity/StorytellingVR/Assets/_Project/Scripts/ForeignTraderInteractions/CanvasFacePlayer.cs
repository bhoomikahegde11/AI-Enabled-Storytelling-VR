using UnityEngine;

/// <summary>
/// Makes the world-space canvas face the VR camera, without mirroring the text
/// (a plain LookAt() flips text backwards — this avoids that).
/// Put this on the same GameObject as the Canvas.
/// </summary>
public class CanvasFacePlayer : MonoBehaviour
{
    [Tooltip("Leave empty to auto-find Camera.main.")]
    [SerializeField] private Transform playerCamera;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (playerCamera == null) return;

        // Face the same direction as the camera (parallel), not "look at" it,
        // which keeps the canvas readable instead of mirrored.
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
    }
}