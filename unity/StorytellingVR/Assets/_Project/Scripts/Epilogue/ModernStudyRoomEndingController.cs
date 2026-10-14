using System.Collections;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Final ending in Epilogue_ModernStudyRoom. No input required:
/// white -> room -> Varaha coin drop + clink -> black -> ending card -> MainMenu_Optimized.
/// Reuses the scene's IntroWhiteFadeCanvas for both the white fade-in and the black fade-out.
/// </summary>
public class ModernStudyRoomEndingController : MonoBehaviour
{
    [Header("Fade (reuse IntroWhiteFadeCanvas)")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;   // IntroWhiteFadeCanvas
    [SerializeField] private Image fadeImage;               // WhiteFadeImage
    [SerializeField] private float whiteFadeInDuration = 1.2f;
    [SerializeField] private float blackFadeOutDuration = 2.5f;

    [Header("Coin")]
    [SerializeField] private Rigidbody coinRigidbody;
    [SerializeField] private Transform coinDropSpawn;
    [SerializeField] private AudioSource coinAudioSource;
    [SerializeField] private AudioClip coinClinkClip;
    [SerializeField] private Vector3 coinSpin = new Vector3(1.5f, 0.5f, 2f);

    [Header("Timing")]
    [SerializeField] private float delayBeforeDrop = 1.25f;
    [SerializeField] private float holdAfterDrop = 4f;
    [SerializeField] private float cardFadeInDuration = 1f;
    [SerializeField] private float cardHoldDuration = 4f;
    [SerializeField] private float cardFadeOutDuration = 0.75f;

    [Header("Ending Card (optional - auto-built if empty)")]
    [SerializeField] private CanvasGroup endingCardGroup;

    // ─────────────────────────────────────────────────────────────────────────
    // NEW: AUTO-BUILT ENDING CARD STYLE
    // These only affect the automatically generated ending card.
    // Leave font fields empty to use TextMeshPro's default font.
    // ─────────────────────────────────────────────────────────────────────────

    [Header("Auto-Built Ending Card Style")]

    [Tooltip("TMP font used for ANUBHAVA. Leave empty to use the default TMP font.")]
    [SerializeField] private TMP_FontAsset endingTitleFont;

    [Tooltip("TMP font used for both smaller ending lines. Leave empty to use the default TMP font.")]
    [SerializeField] private TMP_FontAsset endingBodyFont;

    [Tooltip("Title size relative to the fade canvas height.")]
    [SerializeField] private float endingTitleSizeRatio = 0.085f;

    [Tooltip("First sentence size relative to the fade canvas height.")]
    [SerializeField] private float endingBodySizeRatio = 0.04f;

    [Tooltip("Final sentence size relative to the fade canvas height.")]
    [SerializeField] private float endingFinalLineSizeRatio = 0.032f;

    [Tooltip("Vertical position of ANUBHAVA relative to canvas height.")]
    [SerializeField] private float endingTitleYRatio = 0.10f;

    [Tooltip("Vertical position of the first sentence relative to canvas height.")]
    [SerializeField] private float endingBodyYRatio = -0.02f;

    [Tooltip("Vertical position of the final sentence relative to canvas height.")]
    [SerializeField] private float endingFinalLineYRatio = -0.09f;

    [Header("Scene Cleanup")]
    [Tooltip("Extra GameObjects to deactivate on load (e.g. tutorial prompts).")]
    [SerializeField] private GameObject[] objectsToDisable;

    [Tooltip("Extra components to disable on load (e.g. Grabbable on the notebook).")]
    [SerializeField] private Behaviour[] componentsToDisable;

    [Header("Destination")]
    [SerializeField] private string mainMenuSceneName = GameManager.DefaultMainMenuSceneName;

    private void Awake()
    {
        // Start fully white before the first rendered frame so the hand-off from
        // the historical white fade is seamless.
        if (fadeImage != null)
            fadeImage.color = Color.white;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
        }

        PrepareCoin();

        if (endingCardGroup != null)
            endingCardGroup.alpha = 0f;
    }

    private void Start()
    {
        DisableGameplayInteraction();
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // Wait for GameManager's load routine (which fades the persistent fader in)
        // to finish, then restore that fader to black for every future transition.
        while (
            GameManager.ExistingInstance != null &&
            GameManager.ExistingInstance.IsLoading
        )
        {
            yield return null;
        }

        RestorePersistentFaderToBlack();

        // Re-apply in case anything was re-enabled during scene start.
        DisableGameplayInteraction();

        // Fade from white into the study room.
        yield return FadeOverlay(
            1f,
            0f,
            whiteFadeInDuration
        );

        yield return new WaitForSeconds(
            delayBeforeDrop
        );

        // Drop the Varaha.
        DropCoin();

        yield return new WaitForSeconds(
            holdAfterDrop
        );

        // Black fade: same overlay, recoloured to black while still transparent.
        if (fadeImage != null)
            fadeImage.color = Color.black;

        yield return FadeOverlay(
            0f,
            1f,
            blackFadeOutDuration
        );

        // Build ending card automatically if no custom card was supplied.
        if (endingCardGroup == null)
            endingCardGroup = BuildEndingCard();

        yield return FadeGroup(
            endingCardGroup,
            0f,
            1f,
            cardFadeInDuration
        );

        yield return new WaitForSeconds(
            cardHoldDuration
        );

        yield return FadeGroup(
            endingCardGroup,
            1f,
            0f,
            cardFadeOutDuration
        );

        // Screen stays black (overlay alpha 1).
        // The persistent fader fades out over black, the menu loads,
        // then the menu fades in. Save data is untouched.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneByName(
                mainMenuSceneName
            );
        }
        else
        {
            Debug.LogWarning(
                "[ENDING] GameManager not found - cannot load main menu."
            );
        }
    }

    // ─── Coin ────────────────────────────────────────────────────────────────

    private void PrepareCoin()
    {
        if (coinRigidbody == null)
            return;

        if (coinDropSpawn != null)
        {
            coinRigidbody.transform.SetPositionAndRotation(
                coinDropSpawn.position,
                coinDropSpawn.rotation
            );
        }

        coinRigidbody.isKinematic = true;
        coinRigidbody.useGravity = false;

        coinRigidbody.interpolation =
            RigidbodyInterpolation.Interpolate;

        coinRigidbody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        Collider coinCollider =
            coinRigidbody.GetComponentInChildren<Collider>();

        if (coinCollider == null)
        {
            MeshCollider mc =
                coinRigidbody.gameObject.AddComponent<MeshCollider>();

            mc.convex = true;
            coinCollider = mc;
        }

        // Low bounce so the coin settles on the desk
        // instead of flying away.
        coinCollider.sharedMaterial =
            new PhysicsMaterial("EndingCoin")
            {
                bounciness = 0.05f,
                dynamicFriction = 0.6f,
                staticFriction = 0.7f,

                bounceCombine =
                    PhysicsMaterialCombine.Minimum,

                frictionCombine =
                    PhysicsMaterialCombine.Average
            };

        CoinImpactSound impact =
            coinRigidbody.GetComponent<CoinImpactSound>();

        if (impact == null)
        {
            impact =
                coinRigidbody.gameObject
                    .AddComponent<CoinImpactSound>();
        }

        if (coinAudioSource == null)
        {
            coinAudioSource =
                coinRigidbody.GetComponent<AudioSource>();
        }

        impact.Configure(
            coinAudioSource,
            coinClinkClip
        );

        coinRigidbody.gameObject.SetActive(false);
    }

    private void DropCoin()
    {
        if (coinRigidbody == null)
        {
            Debug.LogWarning(
                "[ENDING] Coin Rigidbody not assigned - skipping coin drop."
            );

            return;
        }

        coinRigidbody.gameObject.SetActive(true);

        coinRigidbody
            .GetComponent<CoinImpactSound>()
            ?.Arm();

        coinRigidbody.isKinematic = false;
        coinRigidbody.useGravity = true;

        coinRigidbody.linearVelocity =
            Vector3.zero;

        // Keep the visible spinning fall.
        coinRigidbody.angularVelocity =
            coinSpin;

        Debug.Log(
            "[ENDING] Coin dropped."
        );
    }

    // ─── Interaction lockdown (head tracking untouched) ──────────────────────

    private void DisableGameplayInteraction()
    {
        TeleportManager.Instance?.DisableAll();

        DisableAll<Grabbable>();
        DisableAll<GrabInteractable>();
        DisableAll<GrabInteractor>();

        DisableAll<HandGrabInteractable>();
        DisableAll<HandGrabInteractor>();

        DisableAll<RayInteractable>();
        DisableAll<RayInteractor>();

        if (objectsToDisable != null)
        {
            foreach (GameObject go in objectsToDisable)
            {
                if (go != null)
                    go.SetActive(false);
            }
        }

        if (componentsToDisable != null)
        {
            foreach (Behaviour b in componentsToDisable)
            {
                if (b != null)
                    b.enabled = false;
            }
        }
    }

    private static void DisableAll<T>()
        where T : Behaviour
    {
        foreach (
            T component in
            FindObjectsByType<T>(
                FindObjectsSortMode.None
            )
        )
        {
            component.enabled = false;
        }
    }

    private static void RestorePersistentFaderToBlack()
    {
        ScreenFader fader =
            FindFirstObjectByType<ScreenFader>();

        if (fader == null)
            return;

        fader.fadeColor = Color.black;

        if (fader.fadeImage != null)
        {
            float a =
                fader.fadeImage.color.a;

            fader.fadeImage.color =
                new Color(
                    0f,
                    0f,
                    0f,
                    a
                );
        }
    }

    // ─── Fades / card ────────────────────────────────────────────────────────

    private IEnumerator FadeOverlay(
        float from,
        float to,
        float duration
    )
    {
        yield return FadeGroup(
            fadeCanvasGroup,
            from,
            to,
            duration
        );
    }

    private static IEnumerator FadeGroup(
        CanvasGroup group,
        float from,
        float to,
        float duration
    )
    {
        if (group == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / duration
                );

            group.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        progress
                    )
                );

            yield return null;
        }

        group.alpha = to;
    }

    // ─── Auto-built ending card ──────────────────────────────────────────────

    private CanvasGroup BuildEndingCard()
    {
        if (fadeCanvasGroup == null)
            return null;

        RectTransform parent =
            (RectTransform)fadeCanvasGroup.transform;

        GameObject root =
            new GameObject(
                "EndingCard",
                typeof(RectTransform),
                typeof(CanvasGroup)
            );

        RectTransform rt =
            (RectTransform)root.transform;

        rt.SetParent(
            parent,
            false
        );

        rt.anchorMin =
            Vector2.zero;

        rt.anchorMax =
            Vector2.one;

        rt.offsetMin =
            Vector2.zero;

        rt.offsetMax =
            Vector2.zero;

        // Draw above the black fade image.
        rt.SetAsLastSibling();

        float h =
            Mathf.Max(
                400f,
                parent.rect.height
            );

        // ANUBHAVA
        AddLine(
            rt,
            "ANUBHAVA",
            h * endingTitleSizeRatio,
            h * endingTitleYRatio,
            1f,
            FontStyles.Bold,
            0.25f,
            endingTitleFont
        );

        // Main line
        AddLine(
            rt,
            "Some experiences leave more than memories.",
            h * endingBodySizeRatio,
            h * endingBodyYRatio,
            1f,
            FontStyles.Normal,
            0f,
            endingBodyFont
        );

        // Final line
        AddLine(
            rt,
            "Perhaps some are still waiting.",
            h * endingFinalLineSizeRatio,
            h * endingFinalLineYRatio,
            0.75f,
            FontStyles.Italic,
            0f,
            endingBodyFont
        );

        CanvasGroup group =
            root.GetComponent<CanvasGroup>();

        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        return group;
    }

    private static void AddLine(
        RectTransform parent,
        string text,
        float size,
        float y,
        float alpha,
        FontStyles style,
        float spacing,
        TMP_FontAsset font
    )
    {
        GameObject go =
            new GameObject(
                text.Length > 12
                    ? "Line"
                    : text,
                typeof(RectTransform)
            );

        RectTransform rt =
            (RectTransform)go.transform;

        rt.SetParent(
            parent,
            false
        );

        rt.anchorMin =
            new Vector2(
                0.1f,
                0.5f
            );

        rt.anchorMax =
            new Vector2(
                0.9f,
                0.5f
            );

        rt.sizeDelta =
            new Vector2(
                0f,
                size * 1.6f
            );

        rt.anchoredPosition =
            new Vector2(
                0f,
                y
            );

        TextMeshProUGUI tmp =
            go.AddComponent<TextMeshProUGUI>();

        tmp.text = text;

        // Use custom font only if one has been assigned.
        // Otherwise TMP keeps its normal default font.
        if (font != null)
            tmp.font = font;

        tmp.fontSize = size;
        tmp.fontStyle = style;

        tmp.characterSpacing =
            spacing * 40f;

        tmp.alignment =
            TextAlignmentOptions.Center;

        tmp.color =
            new Color(
                1f,
                1f,
                1f,
                alpha
            );

        tmp.raycastTarget = false;

        tmp.textWrappingMode =
            TextWrappingModes.Normal;
    }
}