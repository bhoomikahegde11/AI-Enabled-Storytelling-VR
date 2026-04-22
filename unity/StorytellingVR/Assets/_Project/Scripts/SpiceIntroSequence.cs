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
            "12 Gold Coins / Sack",
            "Pepper is among the most sought-after goods in the market, prized by traders from distant lands."
        );

        yield return ShowSpice(
            turmericUI,
            "Turmeric",
            "5 Gold Coins / Sack",
            "Turmeric is valued for its colour, flavour, and medicinal use."
        );

        yield return ShowSpice(
            cardamomUI,
            "Cardamom",
            "18 Gold Coins / Sack",
            "Cardamom is rare and fragrant, often found in royal kitchens and temple offerings."
        );

        yield return ShowSpice(
            cinnamonUI,
            "Cinnamon",
            "20 Gold Coins / Sack",
            "Cinnamon travels through long trade routes, making it one of the most precious goods in the market."
        );

        yield return ShowSubtitle(
            "Remember these goods well. Knowing their worth may decide the success of your trade.",
            4f
        );

        yield return ShowSubtitle(
            "And now... it seems your first customer approaches.",
            3f
        );

        yield return FadeCanvas(subtitleCanvas, 0f, 1f);
    }

    IEnumerator ShowSpice(CanvasGroup ui, string spiceName, string spicePrice, string narration)
    {
        ui.alpha = 0f;
        ui.gameObject.SetActive(true);

        TMP_Text[] texts = ui.GetComponentsInChildren<TMP_Text>(true);

        if (texts.Length >= 2)
        {
            texts[0].text = spiceName;
            texts[1].text = spicePrice;
        }

        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            ui.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        ui.alpha = 1f;

        yield return ShowSubtitle(narration, 3.5f);

        yield return new WaitForSeconds(0.5f);

        yield return FadeCanvas(ui, 0f, 0.3f);
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