using UnityEngine;

public class VRInspectRotate : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;
    [SerializeField] private OVRInput.Button inspectButton = OVRInput.Button.PrimaryIndexTrigger;

    [Header("Rotation")]
    [SerializeField] private float rotationSensitivity = 1.2f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Haptics")]
    [SerializeField] private float enterHapticAmplitude = 0.45f;
    [SerializeField] private float enterHapticFrequency = 0.35f;
    [SerializeField] private float rotateHapticAmplitude = 0.12f;
    [SerializeField] private float rotateHapticFrequency = 0.15f;
    [SerializeField] private float rotateHapticCooldown = 0.25f;

    private bool isInspecting;
    private Quaternion lastHandRotation;
    private Quaternion targetRotation;
    private float hapticStopTime;
    private float nextRotateHapticTime;

    private void OnEnable()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (rightHandAnchor == null)
            return;

        if (OVRInput.GetDown(inspectButton, controller))
        {
            EnterInspectMode();
        }

        if (OVRInput.Get(inspectButton, controller) && isInspecting)
        {
            RotateUsingWrist();
        }

        if (OVRInput.GetUp(inspectButton, controller))
        {
            ExitInspectMode();
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );

        StopHapticsIfNeeded();
    }

    private void EnterInspectMode()
    {
        isInspecting = true;
        lastHandRotation = rightHandAnchor.rotation;
        targetRotation = transform.rotation;

        PlayHaptic(enterHapticFrequency, enterHapticAmplitude, 0.08f);
        Debug.Log("Coin inspect started");
    }

    private void ExitInspectMode()
    {
        isInspecting = false;
        OVRInput.SetControllerVibration(0, 0, controller);

        Debug.Log("Coin inspect ended");
    }

    private void RotateUsingWrist()
    {
        Quaternion currentHandRotation = rightHandAnchor.rotation;
        Quaternion delta = currentHandRotation * Quaternion.Inverse(lastHandRotation);

        Vector3 euler = delta.eulerAngles;
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        Quaternion rotation =
            Quaternion.AngleAxis(-euler.y * rotationSensitivity, Vector3.up) *
            Quaternion.AngleAxis(euler.x * rotationSensitivity, Vector3.left) *
            Quaternion.AngleAxis(-euler.z * rotationSensitivity * 0.35f, Vector3.forward);

        targetRotation = rotation * targetRotation;

        float movementAmount =
            Mathf.Abs(euler.x) +
            Mathf.Abs(euler.y) +
            Mathf.Abs(euler.z);

        if (movementAmount > 2.5f && Time.time >= nextRotateHapticTime)
        {
            PlayHaptic(rotateHapticFrequency, rotateHapticAmplitude, 0.035f);
            nextRotateHapticTime = Time.time + rotateHapticCooldown;
        }

        lastHandRotation = currentHandRotation;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }

    private void PlayHaptic(float frequency, float amplitude, float duration)
    {
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        hapticStopTime = Time.time + duration;
    }

    private void StopHapticsIfNeeded()
    {
        if (hapticStopTime > 0f && Time.time >= hapticStopTime)
        {
            OVRInput.SetControllerVibration(0, 0, controller);
            hapticStopTime = 0f;
        }
    }
}