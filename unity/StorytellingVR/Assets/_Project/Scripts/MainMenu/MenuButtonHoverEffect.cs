using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MenuButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Header("Visual Target")]
    [SerializeField] private Image targetImage;
    [Tooltip("Main Menu only: animate the label, keeping the XR collider and button root fixed.")]
    [SerializeField] private bool stableHitbox;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(1f, 0.92f, 0.65f, 1f);

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float animationSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color targetColor;
    private bool isHovered;
    private Coroutine clickRoutine;
    private Selectable selectable;
    private RectTransform label;
    private Vector3 labelScale;
    private bool isSelected, isPressed;
    private float pressUntil;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        originalScale = transform.localScale;
        if (stableHitbox)
        {
            selectable = GetComponent<Selectable>();
            TMP_Text text = GetComponentInChildren<TMP_Text>(true);
            label = text != null ? text.rectTransform : null;
            if (label != null) labelScale = label.localScale;
        }
        targetScale = originalScale;
        targetColor = normalColor;

        if (targetImage != null)
            targetImage.color = normalColor;
    }

    private void Update()
    {
        if (stableHitbox)
        {
            AdvanceVisuals(Time.unscaledDeltaTime);
            return;
        }
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            animationSpeed * Time.unscaledDeltaTime
        );

        if (targetImage != null)
        {
            targetImage.color = Color.Lerp(
                targetImage.color,
                targetColor,
                animationSpeed * Time.unscaledDeltaTime
            );
        }
    }

    internal void AdvanceVisuals(float unscaledDeltaTime)
    {
        bool available = selectable == null || selectable.IsInteractable();
        bool highlighted = available && (isHovered || isSelected);
        // The last click may remain visible during an outgoing fade, whose input is already blocked.
        bool pressed = isPressed || Time.unscaledTime < pressUntil;
        float amount = pressed ? 0.985f : highlighted ? 1.018f : 1f;
        float t = 1f - Mathf.Exp(-animationSpeed * Mathf.Max(0f, unscaledDeltaTime));
        if (label != null)
        {
            Vector3 target = labelScale * amount;
            Vector3 next = Vector3.Lerp(label.localScale, target, t);
            if ((next - target).sqrMagnitude < 0.000001f) next = target;
            if (label.localScale != next) label.localScale = next;
        }
        if (targetImage != null)
        {
            Color target = pressed ? new Color(0.88f, 0.81f, 0.65f, 1f) : highlighted ? hoverColor : normalColor;
            Color next = Color.Lerp(targetImage.color, target, t);
            if (Mathf.Abs(next.r - target.r) + Mathf.Abs(next.g - target.g) + Mathf.Abs(next.b - target.b) < 0.001f) next = target;
            if (targetImage.color != next) targetImage.color = next;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered)
            return;

        isHovered = true;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;

        if (eventData == null || eventData.pointerId >= 0)
        {
            MainMenuAudioController.Instance?.PlayHover();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
        targetColor = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (stableHitbox)
        {
            pressUntil = Time.unscaledTime + 0.09f; // Quest sends click directly, without pointer down/up.
            return;
        }
        if (clickRoutine != null)
            StopCoroutine(clickRoutine);

        clickRoutine = StartCoroutine(PlayClickEffect());
    }

    public void OnPointerDown(PointerEventData eventData) { if (stableHitbox) isPressed = true; }
    public void OnPointerUp(PointerEventData eventData) { if (stableHitbox) isPressed = false; }
    public void OnSelect(BaseEventData eventData)
    {
        if (stableHitbox) isSelected = EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
    }
    public void OnDeselect(BaseEventData eventData) { if (stableHitbox) isSelected = false; }

    private void OnDisable()
    {
        if (!stableHitbox) return; // Preserve the existing effects in the other storytelling scenes.
        isHovered = isSelected = isPressed = false;
        pressUntil = 0f;
        if (label != null) label.localScale = labelScale;
        if (targetImage != null) targetImage.color = normalColor;
    }

    private IEnumerator PlayClickEffect()
    {
        targetScale = originalScale * clickScale;

        yield return new WaitForSecondsRealtime(0.08f);

        targetScale = isHovered
            ? originalScale * hoverScale
            : originalScale;

        clickRoutine = null;
    }
}
