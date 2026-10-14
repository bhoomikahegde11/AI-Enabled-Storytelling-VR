using UnityEngine;
using UnityEngine.EventSystems;

// Keeps the input-bearing hierarchy stationary; only a non-interactive heading moves.
[RequireComponent(typeof(CanvasGroup))]
public sealed class MainMenuPanelTransition : MonoBehaviour
{
    public const float Duration = 0.26f;
    public bool IsVisible { get; private set; }
    public bool IsAnimating { get; private set; }
    private CanvasGroup group;
    private RectTransform heading;
    private Vector2 headingPosition;
    private float elapsed, startAlpha;

    public void Initialize(bool visible, RectTransform title = null)
    {
        group = GetComponent<CanvasGroup>();
        heading = title;
        if (heading != null) headingPosition = heading.anchoredPosition;
        SetVisible(visible, false);
    }

    public void SetVisible(bool visible, bool animate = true)
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        if (IsVisible == visible && gameObject.activeSelf == visible && !IsAnimating && animate) return;
        IsVisible = visible;
        group.interactable = group.blocksRaycasts = false;
        if (!visible && EventSystem.current != null)
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.transform.IsChildOf(transform))
                EventSystem.current.SetSelectedGameObject(null);
        }
        if (!animate)
        {
            IsAnimating = false;
            group.alpha = visible ? 1f : 0f;
            group.interactable = group.blocksRaycasts = visible;
            if (heading != null) heading.anchoredPosition = headingPosition;
            gameObject.SetActive(visible);
            return;
        }
        if (visible && !gameObject.activeSelf) gameObject.SetActive(true);
        startAlpha = group.alpha;
        elapsed = 0f;
        IsAnimating = true;
    }

    private void Update() => Advance(Time.unscaledDeltaTime);

    // Explicit stepping also lets EditMode tests check interruption without running a scene.
    internal void Advance(float unscaledDeltaTime)
    {
        if (!IsAnimating) return;
        elapsed += Mathf.Max(0f, unscaledDeltaTime);
        float t = Mathf.Clamp01(elapsed / Duration);
        group.alpha = Mathf.Lerp(startAlpha, IsVisible ? 1f : 0f, Mathf.SmoothStep(0f, 1f, t));
        if (heading != null) heading.anchoredPosition = headingPosition + Vector2.down * (6f * (1f - group.alpha));
        if (t < 1f) return;
        IsAnimating = false;
        group.interactable = group.blocksRaycasts = IsVisible;
        if (!IsVisible) gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        IsAnimating = false;
        IsVisible = false;
        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = group.blocksRaycasts = false;
        }
        if (heading != null) heading.anchoredPosition = headingPosition;
    }
}
