using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class SpiceIntroSequence : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource narratorAudioSource;

    [Header("Narration Clips")]
    public AudioClip intro1;
    public AudioClip intro2;
    public AudioClip intro3;
    public AudioClip intro4;

    public AudioClip pepperClip;
    public AudioClip turmericClip;
    public AudioClip cardamomClip;
    public AudioClip cinnamonClip;

    public AudioClip endingClip;
    [Header("UI")]
    public CanvasGroup subtitleCanvas;
    public TMP_Text subtitleText;

    [Header("Spice Info UI")]
    public CanvasGroup spiceInfoCanvas;
    public TMP_Text spiceNameText;
    public TMP_Text spicePriceText;

    [Header("Camera / Focus")]
    public Volume globalVolume;
    private DepthOfField dof;

    [Header("UI Positions")]
    public Transform pepperTarget;
    public Transform turmericTarget;
    public Transform cardamomTarget;
    public Transform cinnamonTarget;

    [Header("UI Anchors")]
    public Transform pepperUIAnchor;
    public Transform turmericUIAnchor;
    public Transform cardamomUIAnchor;
    public Transform cinnamonUIAnchor;

    [Header("Camera")]
    public Camera mainCamera;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        if (globalVolume.profile.TryGet(out dof))
        {
            dof.active = true;
        }

        yield return FadeCanvas(subtitleCanvas, 1f, 1f);

        yield return ShowSubtitle(
            "Welcome, traveller. Before you stands the great bazaar of Hampi, where voices from distant lands mingle with the scent of spice and dust.",
            intro1
        );

        yield return ShowSubtitle(
            "Here, merchants gather with horses, silk, gems, and goods from distant kingdoms.",
            intro2
        );

        yield return ShowSubtitle(
            "But among all treasures of the market, few are as valuable as spices.",
            intro3
        );

        yield return ShowSubtitle(
            "The stall before you is yours.",
            intro4
        );

        yield return FocusOnSpice(
            pepperTarget,
            "Pepper",
            "12 Varahas / Veesai",
            "Pepper is among the most sought-after goods in the market, prized by traders from distant lands.",
            pepperClip
        );

        yield return FocusOnSpice(
            turmericTarget,
            "Turmeric",
            "5 Varahas / Veesai",
            "Turmeric is valued for its colour, flavour, and medicinal use.",
            turmericClip
        );

        yield return FocusOnSpice(
            cardamomTarget,
            "Cardamom",
            "18 Varahas / Veesai",
            "Cardamom is rare and fragrant, often found in royal kitchens and temple offerings.",
            cardamomClip
        );

        yield return FocusOnSpice(
            cinnamonTarget,
            "Cinnamon",
            "20 varahas / veesai",
            "Cinnamon travels through long trade routes, making it one of the most precious goods in the market.",
            cinnamonClip
        );

        yield return ShowSubtitle(
            "Remember these goods well. Knowing their worth may decide the success of your trade.",
            endingClip
        );
    }

    IEnumerator FocusOnSpice(
    Transform target,
    string spiceName,
    string spicePrice,
    string narration,
    AudioClip narrationClip)
    {
        // Move popup to current spice location
        if (target != null)
        {
            spiceInfoCanvas.transform.position =
                target.position;
        }

        if (dof != null)
        {
            dof.focusDistance.value = 2f;
            dof.gaussianStart.value = 1f;
            dof.gaussianEnd.value = 3f;
        }

        spiceNameText.text = spiceName;
        spicePriceText.text = spicePrice;

        yield return FadeCanvas(spiceInfoCanvas, 1f, 0.5f);

        yield return ShowSubtitle(narration, narrationClip);

        yield return new WaitForSeconds(1f);

        yield return FadeCanvas(spiceInfoCanvas, 0f, 0.5f);
    }

    IEnumerator ShowSubtitle(string message, AudioClip clip)
    {
        subtitleText.text = message;

        if (clip != null && narratorAudioSource != null)
        {
            narratorAudioSource.clip = clip;
            narratorAudioSource.Play();

            yield return new WaitWhile(() => narratorAudioSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator FadeCanvas(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}