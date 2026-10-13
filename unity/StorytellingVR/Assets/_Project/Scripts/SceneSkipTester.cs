using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SceneSkipTester : MonoBehaviour
{
    private bool bWasPressed;

    void Update()
    {
        if (PauseMenuController.GameplayInputBlocked) return;
        // Keyboard skip lives only in GameManager (F8, Editor/debug); a letter key here fired while typing in chat.

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed))
        {
            // Same scene gate as the Y skip: Level 1 uses B for voice reset.
            if (bPressed && !bWasPressed &&
                GameManager.ControllerSceneSkipAllowedIn(SceneManager.GetActiveScene().name))
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