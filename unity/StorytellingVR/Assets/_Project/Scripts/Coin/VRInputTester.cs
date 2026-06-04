using UnityEngine;

public class VRInputTester : MonoBehaviour
{
    void Update()
    {
        Vector2 rightStick =
            OVRInput.Get(
                OVRInput.Axis2D.SecondaryThumbstick
            );


        Vector2 leftStick =
            OVRInput.Get(
                OVRInput.Axis2D.PrimaryThumbstick
            );


        if (rightStick.magnitude > 0.1f)
        {
            Debug.Log(
                "RIGHT STICK " + rightStick
            );
        }


        if (leftStick.magnitude > 0.1f)
        {
            Debug.Log(
                "LEFT STICK " + leftStick
            );
        }
    }
}