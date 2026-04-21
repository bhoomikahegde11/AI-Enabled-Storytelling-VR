using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpiceIntroSequence : MonoBehaviour
{
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

    [Header("Focus Targets")]
    public Transform pepperTarget;
    public Transform turmericTarget;
    public Transform cardamomTarget;
    public Transform cinnamonTarget;

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
            5f
        );

        yield return ShowSubtitle(
            "Here, merchants gather with horses, silk, gems, and goods from distant kingdoms.",
            4f
        );

        yield return ShowSubtitle(
            "But among all treasures of the market, few are as valuable as spices.",
            3.5f
        );

        yield return ShowSubtitle(
            "The stall before you is yours.",
            2.5f
        );

        yield return FocusOnSpice(
            pepperTarget,
            "Pepper",
            "12 Gold Coins / Sack",
            "Pepper is among the most sought-after goods in the market, prized by traders from distant lands."
        );

        yield return FocusOnSpice(
            turmericTarget,
            "Turmeric",
            "5 Gold Coins / Sack",
            "Turmeric is valued for its colour, flavour, and medicinal use."
        );

        yield return FocusOnSpice(
            cardamomTarget,
            "Cardamom",
            "18 Gold Coins / Sack",
            "Cardamom is rare and fragrant, often found in royal kitchens and temple offerings."
        );

        yield return FocusOnSpice(
            cinnamonTarget,
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

        // Optional:
        // Trigger trader intro scene or trader sequence here
        // traderIntroSequence.PlaySequence();
    }

    IEnumerator FocusOnSpice(Transform target, string spiceName, string spicePrice, string narration)
    {
        if (target != null)
        {
            Vector3 lookDirection = target.position - mainCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            float timer = 0f;
            Quaternion startRotation = mainCamera.transform.rotation;

            while (timer < 1f)
            {
                timer += Time.deltaTime;
                mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timer);
                yield return null;
            }
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