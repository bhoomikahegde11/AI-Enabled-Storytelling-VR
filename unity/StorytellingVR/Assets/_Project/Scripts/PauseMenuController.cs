using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class PauseMenuController : MonoBehaviour
{
    private const string PauseCanvasName = "PauseMenuCanvas";
    private const string PausePanelName = "PauseMenuPanel";
    private const string PauseLaserName = "PauseMenuLaserPointer";
    private const float MenuDistance = 1.35f;
    private const float MenuVerticalOffset = -0.05f;
    private const float MenuFollowSmoothing = 18f;
    private const float CanvasScale = 0.0015f;
    private const float TriggerThreshold = 0.75f;
    private const string RightControllerAnchorName = "RightControllerAnchor";
    private const string RightHandAnchorName = "RightHandAnchor";
    private const string RightTouchControllerWorldName = "right_touch_controller_world";

    private static PauseMenuController instance;

    private Camera currentMainCamera;
    private Transform currentRightControllerAnchor;
    private Canvas pauseCanvas;
    private RectTransform pauseCanvasRect;
    private GameObject pausePanel;
    private Button resumeButton;
    private Button returnToMainMenuButton;
    private PauseMenuLaserPointer pauseLaserPointer;
    private Behaviour[] suspendedSceneLasers;
    private bool[] suspendedSceneLaserStates;
    private bool isPaused;
    private float previousTimeScale = 1f;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisible;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("[PAUSE] Duplicate PauseMenuController detected. Destroying duplicate component.");
            Destroy(this);
            return;
        }

        instance = this;
        Debug.Log($"[PAUSE] Controller Awake on '{gameObject.name}'.");

        BuildPauseMenu();
        HidePauseMenuVisuals();
        RebindSceneReferences();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        HidePauseMenuVisuals();
        RebindSceneReferences();
    }

    private void Update()
    {
        if (IsPauseBlockedInCurrentScene())
        {
            if (isPaused)
            {
                ForceClosePauseMenu(resetTimeScaleToOne: true);
            }

            return;
        }

        if (ShouldTogglePause())
        {
            TogglePause();
        }

        if (!isPaused)
        {
            return;
        }

        if (currentMainCamera == null || !currentMainCamera.isActiveAndEnabled)
        {
            RebindSceneReferences();
        }

        PositionPauseMenu();

        if (pauseLaserPointer == null && currentRightControllerAnchor != null)
        {
            AttachPauseLaserPointer();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"[PAUSE] Scene loaded: {scene.name}");
        RebindSceneReferences();

        if (string.Equals(scene.name, GameManager.DefaultMainMenuSceneName, System.StringComparison.Ordinal))
        {
            ForceClosePauseMenu(resetTimeScaleToOne: true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if (isPaused)
        {
            CaptureAndDisableSceneLasers();
            PositionPauseMenu(snapToTarget: true);
            AttachPauseLaserPointer();
        }
        else
        {
            HidePauseMenuVisuals();
            DetachPauseLaserPointer();
        }
    }

    private bool ShouldTogglePause()
    {
        bool escapePressed = false;

        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            escapePressed = Input.GetKeyDown(KeyCode.Escape);
            if (escapePressed)
            {
                Debug.Log("[PAUSE] Escape detected");
            }
        }

        bool questStartPressed = OVRInput.GetDown(OVRInput.Button.Start);
        return escapePressed || questStartPressed;
    }

    private bool IsPauseBlockedInCurrentScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return string.Equals(activeSceneName, GameManager.DefaultMainMenuSceneName, System.StringComparison.Ordinal);
    }

    private void TogglePause()
    {
        Debug.Log("[PAUSE] Toggle requested");

        if (isPaused)
        {
            ResumeGameplay();
            return;
        }

        PauseGameplay();
    }

    private void PauseGameplay()
    {
        if (isPaused)
        {
            return;
        }

        if (IsPauseBlockedInCurrentScene())
        {
            Debug.Log($"[PAUSE] Pause blocked because: active scene is '{SceneManager.GetActiveScene().name}'.");
            return;
        }

        RebindSceneReferences();
        previousTimeScale = Time.timeScale;
        previousCursorVisible = Cursor.visible;
        previousCursorLockMode = Cursor.lockState;

        isPaused = true;
        Time.timeScale = 0f;

        CaptureAndDisableSceneLasers();
        ShowPauseMenuVisuals();
        PositionPauseMenu(snapToTarget: true);
        AttachPauseLaserPointer();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log($"[PAUSE] Paused. CanvasCreated={pauseCanvas != null}, CanvasActive={(pauseCanvas != null && pauseCanvas.gameObject.activeSelf)}");
    }

    private void ResumeGameplay()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = previousTimeScale;

        HidePauseMenuVisuals();
        DetachPauseLaserPointer();
        RestoreSceneLasers();

        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLockMode;
        Debug.Log("[PAUSE] Resumed");
    }

    private void ReturnToMainMenu()
    {
        ForceClosePauseMenu(resetTimeScaleToOne: true);
        GameManager.Instance.LoadSceneByName(GameManager.DefaultMainMenuSceneName);
    }

    private void ForceClosePauseMenu(bool resetTimeScaleToOne)
    {
        isPaused = false;
        Time.timeScale = resetTimeScaleToOne ? 1f : previousTimeScale;
        previousTimeScale = 1f;

        HidePauseMenuVisuals();
        DetachPauseLaserPointer();
        RestoreSceneLasers();

        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLockMode;
    }

    private void RebindSceneReferences()
    {
        currentMainCamera = ResolveMainCamera();
        currentRightControllerAnchor = ResolveRightControllerAnchor();

        if (pauseCanvas != null)
        {
            pauseCanvas.worldCamera = currentMainCamera;
        }
    }

    private Camera ResolveMainCamera()
    {
        if (Camera.main != null)
        {
            return Camera.main;
        }

        Camera[] cameras = Object.FindObjectsByType<Camera>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        foreach (Camera cameraInstance in cameras)
        {
            if (cameraInstance != null && cameraInstance.isActiveAndEnabled)
            {
                return cameraInstance;
            }
        }

        return null;
    }

    private Transform ResolveRightControllerAnchor()
    {
        Transform[] transforms = Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None);

        Transform fallbackRightHandAnchor = null;
        Transform fallbackTouchController = null;

        foreach (Transform candidate in transforms)
        {
            if (candidate == null)
            {
                continue;
            }

            string candidateName = candidate.name;

            if (string.Equals(candidateName, RightControllerAnchorName, System.StringComparison.Ordinal))
            {
                return candidate;
            }

            if (fallbackRightHandAnchor == null &&
                string.Equals(candidateName, RightHandAnchorName, System.StringComparison.Ordinal))
            {
                fallbackRightHandAnchor = candidate;
            }

            if (fallbackTouchController == null &&
                string.Equals(candidateName, RightTouchControllerWorldName, System.StringComparison.Ordinal))
            {
                fallbackTouchController = candidate;
            }
        }

        return fallbackRightHandAnchor != null ? fallbackRightHandAnchor : fallbackTouchController;
    }

    private void BuildPauseMenu()
    {
        GameObject canvasObject = new GameObject(PauseCanvasName, typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        pauseCanvasRect = canvasObject.GetComponent<RectTransform>();
        pauseCanvasRect.sizeDelta = new Vector2(900f, 560f);
        pauseCanvasRect.localScale = Vector3.one * CanvasScale;

        pauseCanvas = canvasObject.GetComponent<Canvas>();
        pauseCanvas.renderMode = RenderMode.WorldSpace;
        pauseCanvas.sortingOrder = 5000;

        GameObject panelObject = CreateUiObject(PausePanelName, canvasObject.transform);
        pausePanel = panelObject;

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.08f, 0.08f, 0.94f);

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(900f, 560f);

        CreateLabel("Title", panelObject.transform, "Paused", 54, new Vector2(0f, 180f), new Vector2(620f, 90f));
        CreateLabel("Subtitle", panelObject.transform, "Gameplay is frozen while this menu is open.", 24, new Vector2(0f, 90f), new Vector2(720f, 60f));

        resumeButton = CreateButton("ResumeButton", panelObject.transform, "Resume", new Vector2(0f, -20f));
        returnToMainMenuButton = CreateButton("ReturnToMainMenuButton", panelObject.transform, "Return To Main Menu", new Vector2(0f, -145f));

        resumeButton.onClick.AddListener(ResumeGameplay);
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void ShowPauseMenuVisuals()
    {
        if (pauseCanvas == null)
        {
            return;
        }

        pauseCanvas.gameObject.SetActive(true);
        pausePanel.SetActive(true);
        pauseCanvas.worldCamera = currentMainCamera;
    }

    private void HidePauseMenuVisuals()
    {
        if (pauseCanvas != null)
        {
            pauseCanvas.gameObject.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void PositionPauseMenu(bool snapToTarget = false)
    {
        if (!isPaused || pauseCanvasRect == null || currentMainCamera == null)
        {
            return;
        }

        Transform cameraTransform = currentMainCamera.transform;
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = cameraTransform.forward;
        }

        forward.Normalize();

        Vector3 targetPosition =
            cameraTransform.position +
            (forward * MenuDistance) +
            (cameraTransform.up * MenuVerticalOffset);

        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - cameraTransform.position, Vector3.up);

        if (snapToTarget)
        {
            pauseCanvasRect.position = targetPosition;
            pauseCanvasRect.rotation = targetRotation;
            return;
        }

        pauseCanvasRect.position = Vector3.Lerp(
            pauseCanvasRect.position,
            targetPosition,
            Time.unscaledDeltaTime * MenuFollowSmoothing);

        pauseCanvasRect.rotation = Quaternion.Slerp(
            pauseCanvasRect.rotation,
            targetRotation,
            Time.unscaledDeltaTime * MenuFollowSmoothing);
    }

    private void AttachPauseLaserPointer()
    {
        DetachPauseLaserPointer();

        if (!isPaused || currentRightControllerAnchor == null)
        {
            return;
        }

        pauseLaserPointer = currentRightControllerAnchor.gameObject.AddComponent<PauseMenuLaserPointer>();
        pauseLaserPointer.Initialize(resumeButton, returnToMainMenuButton);
    }

    private void DetachPauseLaserPointer()
    {
        if (pauseLaserPointer == null)
        {
            return;
        }

        Destroy(pauseLaserPointer);
        pauseLaserPointer = null;
    }

    private void CaptureAndDisableSceneLasers()
    {
        RestoreSceneLasers();

        List<Behaviour> lasersToSuspend = new List<Behaviour>();

        NPCDialogueVRLaserPointer[] gameplayLasers = Object.FindObjectsByType<NPCDialogueVRLaserPointer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (NPCDialogueVRLaserPointer gameplayLaser in gameplayLasers)
        {
            if (gameplayLaser != null)
            {
                lasersToSuspend.Add(gameplayLaser);
            }
        }

        suspendedSceneLasers = lasersToSuspend.ToArray();
        suspendedSceneLaserStates = new bool[suspendedSceneLasers.Length];

        for (int i = 0; i < suspendedSceneLasers.Length; i++)
        {
            suspendedSceneLaserStates[i] = suspendedSceneLasers[i].enabled;
            suspendedSceneLasers[i].enabled = false;
        }
    }

    private void RestoreSceneLasers()
    {
        if (suspendedSceneLasers == null || suspendedSceneLaserStates == null)
        {
            return;
        }

        for (int i = 0; i < suspendedSceneLasers.Length; i++)
        {
            if (suspendedSceneLasers[i] != null)
            {
                suspendedSceneLasers[i].enabled = suspendedSceneLaserStates[i];
            }
        }

        suspendedSceneLasers = null;
        suspendedSceneLaserStates = null;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static Font GetBuiltinFont()
    {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void CreateLabel(string objectName, Transform parent, string textValue, int fontSize, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject labelObject = CreateUiObject(objectName, parent);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.sizeDelta = size;
        labelRect.anchoredPosition = anchoredPosition;

        Text text = labelObject.AddComponent<Text>();
        text.font = GetBuiltinFont();
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = textValue;
    }

    private static Button CreateButton(string objectName, Transform parent, string buttonText, Vector2 anchoredPosition)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(360f, 88f);
        buttonRect.anchoredPosition = anchoredPosition;

        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.79f, 0.64f, 0.34f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.79f, 0.64f, 0.34f, 1f);
        colors.highlightedColor = new Color(0.95f, 0.82f, 0.5f, 1f);
        colors.pressedColor = new Color(0.65f, 0.51f, 0.25f, 1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.35f, 0.35f, 0.35f, 0.8f);
        button.colors = colors;
        button.targetGraphic = buttonImage;

        BoxCollider buttonCollider = buttonObject.AddComponent<BoxCollider>();
        buttonCollider.isTrigger = true;
        buttonCollider.center = Vector3.zero;
        buttonCollider.size = new Vector3(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y, 8f);

        GameObject labelObject = CreateUiObject("Label", buttonObject.transform);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text labelText = labelObject.AddComponent<Text>();
        labelText.font = GetBuiltinFont();
        labelText.fontSize = 30;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.black;
        labelText.text = buttonText;

        return button;
    }

    private sealed class PauseMenuLaserPointer : MonoBehaviour
    {
        private readonly List<Button> buttons = new List<Button>();

        private LineRenderer lineRenderer;
        private Button currentHoverButton;
        private bool wasTriggerPressed;

        public void Initialize(params Button[] buttonTargets)
        {
            buttons.Clear();

            foreach (Button buttonTarget in buttonTargets)
            {
                if (buttonTarget != null)
                {
                    buttons.Add(buttonTarget);
                }
            }

            EnsureLineRenderer();
        }

        private void OnEnable()
        {
            EnsureLineRenderer();
            wasTriggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= TriggerThreshold;
        }

        private void Update()
        {
            if (buttons.Count == 0)
            {
                HideLine();
                UpdateHoverButton(null);
                return;
            }

            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;
            Button hitButton = FindButtonHit(origin, direction, out RaycastHit hitInfo);

            UpdateHoverButton(hitButton);
            UpdateLine(origin, hitButton != null ? hitInfo.point : origin + direction * 2f, hitButton != null);
            HandleTrigger(hitButton);
        }

        private void OnDisable()
        {
            UpdateHoverButton(null);
            HideLine();
        }

        private void OnDestroy()
        {
            UpdateHoverButton(null);

            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }
        }

        private void EnsureLineRenderer()
        {
            if (lineRenderer != null)
            {
                return;
            }

            GameObject lineObject = new GameObject(PauseLaserName);
            lineObject.transform.SetParent(transform, false);

            lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.alignment = LineAlignment.View;
            lineRenderer.numCapVertices = 4;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.enabled = false;
        }

        private Button FindButtonHit(Vector3 origin, Vector3 direction, out RaycastHit closestHit)
        {
            closestHit = default;

            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                direction,
                8f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide);

            System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

            foreach (RaycastHit hit in hits)
            {
                Button button = hit.collider.GetComponentInParent<Button>();
                if (button == null || !buttons.Contains(button) || !button.isActiveAndEnabled || !button.interactable)
                {
                    continue;
                }

                closestHit = hit;
                return button;
            }

            return null;
        }

        private void HandleTrigger(Button hitButton)
        {
            bool triggerPressed = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= TriggerThreshold;
            bool freshPress = triggerPressed && !wasTriggerPressed;

            if (freshPress && hitButton != null)
            {
                hitButton.onClick.Invoke();
            }

            wasTriggerPressed = triggerPressed;
        }

        private void UpdateHoverButton(Button newHoverButton)
        {
            if (currentHoverButton == newHoverButton)
            {
                return;
            }

            ApplyButtonHoverState(currentHoverButton, false);
            currentHoverButton = newHoverButton;
            ApplyButtonHoverState(currentHoverButton, true);
        }

        private static void ApplyButtonHoverState(Button button, bool hovered)
        {
            if (button == null || button.targetGraphic == null)
            {
                return;
            }

            ColorBlock colors = button.colors;
            button.targetGraphic.color = hovered ? colors.highlightedColor : colors.normalColor;
        }

        private void UpdateLine(Vector3 start, Vector3 end, bool visible)
        {
            if (lineRenderer == null)
            {
                return;
            }

            lineRenderer.enabled = visible;

            if (!visible)
            {
                return;
            }

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        private void HideLine()
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
