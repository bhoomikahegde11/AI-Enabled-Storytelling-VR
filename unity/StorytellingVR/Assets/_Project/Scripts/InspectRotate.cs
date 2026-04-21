using UnityEngine;

public class InspectRotate : MonoBehaviour
{
    public float sensitivity = 2f;
    public float smoothSpeed = 10f;

    private bool isDragging = false;
    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    // Called when controller clicks coin
    public void StartRotation()
    {
        isDragging = true;
    }

    // Called when released
    public void StopRotation()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            float rotX = input.y * sensitivity * 2f;
            float rotY = -input.x * sensitivity * 2f;

            Quaternion rot =
                Quaternion.AngleAxis(rotY, Camera.main.transform.up) *
                Quaternion.AngleAxis(rotX, Camera.main.transform.right);

            targetRotation = rot * targetRotation;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}