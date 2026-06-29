using UnityEngine;
using UnityEngine.XR;

public class SceneSkipTester : MonoBehaviour
{
    private bool bWasPressed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SkipScene();
        }

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed))
        {
            if (bPressed && !bWasPressed)
            {
                SkipScene();
            }

            bWasPressed = bPressed;
        }
    }

    private void SkipScene()
    {
        Debug.Log("DEV SKIP: Loading next scene");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("No GameManager found");
        }
    }
}