using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MenuButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Visual Target")]
    [SerializeField] private Image targetImage;

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

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        originalScale = transform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;

        if (targetImage != null)
            targetImage.color = normalColor;
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
                targetColor,
                animationSpeed * Time.unscaledDeltaTime
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered)
            return;

        isHovered = true;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;

        MainMenuAudioController.Instance?.PlayHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
        targetColor = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickRoutine != null)
            StopCoroutine(clickRoutine);

        clickRoutine = StartCoroutine(PlayClickEffect());
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
