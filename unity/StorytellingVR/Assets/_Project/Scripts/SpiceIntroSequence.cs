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

    [Header("Spice UI (World Space)")]
    public CanvasGroup pepperUI;
    public CanvasGroup turmericUI;
    public CanvasGroup cardamomUI;
    public CanvasGroup cinnamonUI;

    [Header("DOF (Optional)")]
    public Volume globalVolume;
    private DepthOfField dof;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    public IEnumerator PlaySequence()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out dof))
        {
            dof.active = true;
        }

        yield return FadeCanvas(subtitleCanvas, 1f, 1f);

        yield return ShowSubtitle(
            "Welcome, traveller. Before you stands the great bazaar of Hampi...",
            4f
        );

        yield return ShowSubtitle(
            "Here, merchants gather with goods from distant kingdoms.",
            3f
        );

        yield return ShowSubtitle(
            "But among all treasures, few are as valuable as spices.",
            3f
        );

        yield return ShowSubtitle(
            "The stall before you is yours.",
            2f
        );

        yield return ShowSpice(pepperUI, "Pepper", "12 Gold Coins / Sack",
            "Pepper is among the most sought-after goods.");

        yield return ShowSpice(turmericUI, "Turmeric", "5 Gold Coins / Sack",
            "Turmeric is valued for its colour and medicinal use.");

        yield return ShowSpice(cardamomUI, "Cardamom", "18 Gold Coins / Sack",
            "Cardamom is rare and found in royal kitchens.");

        yield return ShowSpice(cinnamonUI, "Cinnamon", "20 Gold Coins / Sack",
            "Cinnamon travels long routes and is highly precious.");

        yield return ShowSubtitle(
            "Knowing their worth may decide your success.",
            3f
        );
    }

    IEnumerator ShowSpice(CanvasGroup ui, string name, string price, string narration)
    {
        // Get child text components
        TMP_Text nameText = ui.transform.Find("Name").GetComponent<TMP_Text>();
        TMP_Text priceText = ui.transform.Find("Price").GetComponent<TMP_Text>();

        nameText.text = name;
        priceText.text = price;

        // Reset scale for pop effect
        ui.transform.localScale = Vector3.zero;

        // Fade + scale in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            ui.alpha = Mathf.Lerp(0f, 1f, t);
            ui.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        yield return ShowSubtitle(narration, 3.5f);

        yield return new WaitForSeconds(0.5f);

        // Fade out
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