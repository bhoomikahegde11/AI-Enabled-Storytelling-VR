using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles VR input for the tutorial using XR Interaction Toolkit.
/// Works with both simulated and actual VR headsets.
/// </summary>
public class TutorialVRInputHandler : MonoBehaviour
{
    [Header("XR References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rightHandRay;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor leftHandRay;
    public Transform xrOrigin;
    public Camera xrCamera;

    [Header("Slider Interaction")]
    public Slider priceSlider;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable sliderHandle;
    public float sliderSensitivity = 100f;

    [Header("Button Interaction")]
    public Button submitButton;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable submitButtonInteractable;

    [Header("Canvas Settings")]
    public Canvas[] tutorialCanvases;
    public float canvasDistance = 2.5f;
    public float canvasScale = 0.003f;
    public bool autoPositionCanvases = true;

    [Header("Simulation Mode")]
    public bool useSimulationMode = true;
    public KeyCode submitKey = KeyCode.Space;
    public KeyCode increaseKey = KeyCode.UpArrow;
    public KeyCode decreaseKey = KeyCode.DownArrow;
    public float keyboardAdjustSpeed = 5f;

    private bool isGrabbingSlider = false;
    private float lastSliderValue;

    void Start()
    {
        SetupVRCanvases();
        SetupXRInteractions();
        
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
        }
    }

    void Update()
    {
        if (useSimulationMode)
        {
            HandleSimulatedInput();
        }

        if (autoPositionCanvases)
        {
            UpdateCanvasPositions();
        }
    }

    /// <summary>
    /// Sets up all canvases for VR world-space rendering.
    /// </summary>
    void SetupVRCanvases()
    {
        foreach (Canvas canvas in tutorialCanvases)
        {
            if (canvas == null) continue;

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.localScale = Vector3.one * canvasScale;

            // Add GraphicRaycaster if not present
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            // Position in front of player
            if (xrCamera != null)
            {
                canvas.transform.position = xrCamera.transform.position + 
                    xrCamera.transform.forward * canvasDistance;
                canvas.transform.rotation = Quaternion.LookRotation(
                    canvas.transform.position - xrCamera.transform.position);
            }
        }
    }

    /// <summary>
    /// Sets up XR Interaction Toolkit components.
    /// </summary>
    void SetupXRInteractions()
    {
        // Setup slider XR interaction
        if (priceSlider != null && sliderHandle == null)
        {
            GameObject handleObj = priceSlider.handleRect?.gameObject;
            if (handleObj != null)
            {
                sliderHandle = handleObj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                if (sliderHandle == null)
                {
                    sliderHandle = handleObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                }

                sliderHandle.selectEntered.AddListener(OnSliderGrabbed);
                sliderHandle.selectExited.AddListener(OnSliderReleased);
            }
        }

        // Setup button XR interaction
        if (submitButton != null && submitButtonInteractable == null)
        {
            submitButtonInteractable = submitButton.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            if (submitButtonInteractable == null)
            {
                submitButtonInteractable = submitButton.gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            }

            submitButtonInteractable.selectEntered.AddListener(OnSubmitButtonPressed);
        }
    }

    void OnSliderGrabbed(SelectEnterEventArgs args)
    {
        isGrabbingSlider = true;
        lastSliderValue = priceSlider != null ? priceSlider.value : 0f;
    }

    void OnSliderReleased(SelectExitEventArgs args)
    {
        isGrabbingSlider = false;
    }

    void OnSubmitButtonPressed(SelectEnterEventArgs args)
    {
        if (submitButton != null)
        {
            submitButton.onClick?.Invoke();
        }
    }

    /// <summary>
    /// Handles keyboard input for testing without VR headset.
    /// </summary>
    void HandleSimulatedInput()
    {
        if (priceSlider == null) return;

        // Adjust price with arrow keys
        if (Input.GetKey(increaseKey))
        {
            priceSlider.value += keyboardAdjustSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(decreaseKey))
        {
            priceSlider.value -= keyboardAdjustSpeed * Time.deltaTime;
        }

        // Submit with spacebar
        if (Input.GetKeyDown(submitKey))
        {
            if (submitButton != null && submitButton.interactable)
            {
                submitButton.onClick?.Invoke();
            }
        }

        // Mouse wheel for precise control
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            priceSlider.value += scroll * keyboardAdjustSpeed * 10f;
        }
    }

    /// <summary>
    /// Keeps canvases facing the player and at proper distance.
    /// </summary>
    void UpdateCanvasPositions()
    {
        if (xrCamera == null) return;

        foreach (Canvas canvas in tutorialCanvases)
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy) continue;

            // Make canvas face camera
            Vector3 directionToCamera = xrCamera.transform.position - canvas.transform.position;
            directionToCamera.y = 0; // Keep canvas upright
            
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
                canvas.transform.rotation = Quaternion.Slerp(
                    canvas.transform.rotation, 
                    targetRotation, 
                    Time.deltaTime * 5f
                );
            }
        }
    }

    /// <summary>
    /// Manually positions a specific canvas in front of the player.
    /// </summary>
    public void PositionCanvasInFront(Canvas canvas, float distance = -1f)
    {
        if (canvas == null || xrCamera == null) return;

        if (distance < 0) distance = canvasDistance;

        canvas.transform.position = xrCamera.transform.position + 
            xrCamera.transform.forward * distance;
        
        canvas.transform.rotation = Quaternion.LookRotation(
            canvas.transform.position - xrCamera.transform.position);
    }

    /// <summary>
    /// Enables or disables simulation mode.
    /// </summary>
    public void SetSimulationMode(bool enabled)
    {
        useSimulationMode = enabled;
        Debug.Log($"VR Simulation Mode: {(enabled ? "Enabled" : "Disabled")}");
    }

    void OnDestroy()
    {
        if (sliderHandle != null)
        {
            sliderHandle.selectEntered.RemoveListener(OnSliderGrabbed);
            sliderHandle.selectExited.RemoveListener(OnSliderReleased);
        }

        if (submitButtonInteractable != null)
        {
            submitButtonInteractable.selectEntered.RemoveListener(OnSubmitButtonPressed);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw canvas positions in editor
        if (xrCamera != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 canvasPos = xrCamera.transform.position + xrCamera.transform.forward * canvasDistance;
            Gizmos.DrawWireSphere(canvasPos, 0.1f);
            Gizmos.DrawLine(xrCamera.transform.position, canvasPos);
        }
    }
}