using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class QuestionButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Visual References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private TMP_Text targetText;

    [Header("Background Colors")]
    [SerializeField]
    private Color normalBackgroundColor =
        new Color(1f, 1f, 1f, 0.08f);

    [SerializeField]
    private Color hoverBackgroundColor =
        new Color(1f, 0.82f, 0.35f, 0.35f);

    [Header("Text Colors")]
    [SerializeField] private Color normalTextColor = Color.white;

    [SerializeField]
    private Color hoverTextColor =
        new Color(1f, 0.92f, 0.65f, 1f);

    [Header("Scale")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float animationSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSfx;
    [SerializeField] private AudioClip clickSfx;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private Color targetBackgroundColor;
    private Color targetTextColor;

    private bool isHovered;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>(true);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        originalScale = transform.localScale;
        targetScale = originalScale;

        targetBackgroundColor = normalBackgroundColor;
        targetTextColor = normalTextColor;

        ApplyVisualsImmediately();
    }

    private void OnEnable()
    {
        ResetVisualState();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            animationSpeed * Time.unscaledDeltaTime
        );

        if (targetImage != null)
        {
            targetImage.color = Color.Lerp(
                targetImage.color,
                targetBackgroundColor,
                animationSpeed * Time.unscaledDeltaTime
            );
        }

        if (targetText != null)
        {
            targetText.color = Color.Lerp(
                targetText.color,
                targetTextColor,
                animationSpeed * Time.unscaledDeltaTime
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActiveAndEnabled)
            return;

        isHovered = true;

        targetScale = originalScale * hoverScale;
        targetBackgroundColor = hoverBackgroundColor;
        targetTextColor = hoverTextColor;

        PlaySfx(hoverSfx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActiveAndEnabled)
            return;

        isHovered = false;

        targetScale = originalScale;
        targetBackgroundColor = normalBackgroundColor;
        targetTextColor = normalTextColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySfx(clickSfx);

        /*
         * No click animation coroutine here.
         *
         * The dialogue system immediately disables the selected button
         * or closes QuestionCanvas, so starting a coroutine on this object
         * would cause an inactive GameObject error.
         */
    }

    private void OnDisable()
    {
        ResetVisualState();
    }

    private void ResetVisualState()
    {
        isHovered = false;

        targetScale = originalScale;
        targetBackgroundColor = normalBackgroundColor;
        targetTextColor = normalTextColor;

        transform.localScale = originalScale;
        ApplyVisualsImmediately();
    }

    private void ApplyVisualsImmediately()
    {
        if (targetImage != null)
            targetImage.color = targetBackgroundColor;

        if (targetText != null)
            targetText.color = targetTextColor;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}