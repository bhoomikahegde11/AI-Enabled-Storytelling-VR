using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpiceIntroSequence : MonoBehaviour
{
    [Header("Subtitle UI")]
    public CanvasGroup subtitleCanvas;
    public TMP_Text subtitleText;

    [Header("Spice UI (World Space / OVR Overlay)")]
    public CanvasGroup pepperUI;
    public CanvasGroup turmericUI;
    public CanvasGroup cardamomUI;
    public CanvasGroup cinnamonUI;

    [Header("DOF (Optional)")]
    public Volume globalVolume;
    private DepthOfField dof;

    private void Start()
    {
        // Hide all spice UI initially
        pepperUI.alpha = 0f;
        turmericUI.alpha = 0f;
        cardamomUI.alpha = 0f;
        cinnamonUI.alpha = 0f;

        // Hide subtitle canvas initially
        subtitleCanvas.alpha = 0f;

        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        // Enable DOF if available
        if (globalVolume != null && globalVolume.profile.TryGet(out dof))
        {
            dof.active = true;
            dof.mode.value = DepthOfFieldMode.Gaussian;
            dof.gaussianStart.value = 4f;
            dof.gaussianEnd.value = 7f;
            dof.gaussianMaxRadius.value = 1f;
        }

        yield return FadeCanvas(subtitleCanvas, 1f, 1f);

        yield return ShowSubtitle(
            "Welcome, traveller. Before you stands the great bazaar of Hampi, where voices from distant lands mingle with the scent of spice and dust.",
            5f
        );

        yield return ShowSubtitle(
            "Here, merchants gather with horses, silk, gems, and goods from distant kingdoms.",
            4f
        );

        yield return ShowSubtitle(
            "But among all treasures of the market, few are as valuable as spices.",
            3f
        );

        yield return ShowSubtitle(
            "The stall before you is yours.",
            2.5f
        );

        yield return ShowSpice(
            pepperUI,
            "Pepper",
            "12 Varahas / Veesai",
            "Pepper is among the most sought-after goods in the market, prized by traders from distant lands."
        );

        yield return ShowSpice(
            turmericUI,
            "Turmeric",
            "5 Varahas / Veesai",
            "Turmeric is valued for its colour, flavour, and medicinal use."
        );

        yield return ShowSpice(
            cardamomUI,
            "Cardamom",
            "18 Varahas / Veesai",
            "Cardamom is rare and fragrant, often found in royal kitchens and temple offerings."
        );

        yield return ShowSpice(
            cinnamonUI,
            "Cinnamon",
            "20 Varahas / Veesai",
            "Cinnamon travels through long trade routes, making it one of the most precious goods in the market."
        );

        yield return ShowSubtitle(
            "Remember these goods well. Knowing their worth may decide the success of your trade.",
            4f
        );

       
        // Optional:
        // Trigger trader intro scene or trader sequence here
        // traderIntroSequence.PlaySequence();
    }

    IEnumerator ShowSpice(CanvasGroup ui, string spiceName, string spicePrice, string narration)
    {
        // ❌ Camera movement removed completely

        if (dof != null)
        {
            dof.focusDistance.value = 2f;
            dof.gaussianStart.value = 1f;
            dof.gaussianEnd.value = 3f;
        }

        spiceNameText.text = spiceName;
        spicePriceText.text = spicePrice;

        yield return FadeCanvas(spiceInfoCanvas, 1f, 0.5f);
        yield return ShowSubtitle(narration, 4f);
        yield return new WaitForSeconds(1f);
        yield return FadeCanvas(spiceInfoCanvas, 0f, 0.5f);
    }

    IEnumerator ShowSubtitle(string message, float duration)
    {
        subtitleText.text = message;
        yield return new WaitForSeconds(duration);
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