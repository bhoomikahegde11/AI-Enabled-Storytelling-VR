using UnityEngine;

public class VRInspectRotate : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 120f;
    public float smoothSpeed = 8f;


    private Quaternion targetRotation;


    void OnEnable()
    {
        Debug.Log("Joystick Inspect Enabled");

        targetRotation = transform.rotation;
    }


    void Update()
    {
        // Right joystick
        Vector2 stick =
            OVRInput.Get(
                OVRInput.Axis2D.SecondaryThumbstick
            );


        if (stick.magnitude > 0.1f)
        {
            float horizontal =
                -stick.x * rotationSpeed * Time.deltaTime;


            float vertical =
                stick.y * rotationSpeed * Time.deltaTime;


            Quaternion rotation =
                Quaternion.AngleAxis(
                    horizontal,
                    Vector3.up
                )
                *
                Quaternion.AngleAxis(
                    vertical,
                    Vector3.left
                );


            targetRotation =
                rotation * targetRotation;
        }


        // smoothing
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * smoothSpeed
            );
    }
}