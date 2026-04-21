using UnityEngine;

public class InspectRotate : MonoBehaviour
{
    public float sensitivity = 0.2f;
    public float smoothSpeed = 10f;

    private Vector3 lastMousePos;
    private bool isDragging = false;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            float rotX = delta.y * sensitivity;
            float rotY = -delta.x * sensitivity;

            Quaternion rot = Quaternion.AngleAxis(rotY, Camera.main.transform.up) *
                             Quaternion.AngleAxis(rotX, Camera.main.transform.right);

            targetRotation = rot * targetRotation;

            lastMousePos = Input.mousePosition;
        }

        // Smoothly move toward target
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}