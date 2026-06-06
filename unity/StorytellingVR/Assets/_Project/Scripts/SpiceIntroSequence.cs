using System.Collections;
using TMPro;
using UnityEngine;

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


    [Header("Subtitle UI")]
    public CanvasGroup subtitleCanvas;
    public TMP_Text subtitleText;


    [Header("Spice UI")]
    public CanvasGroup pepperUI;
    public CanvasGroup turmericUI;
    public CanvasGroup cardamomUI;
    public CanvasGroup cinnamonUI;


    private void Start()
    {
        StartCoroutine(PlaySequence());
    }


    IEnumerator PlaySequence()
    {
        // hide all spice panels first (guard against unassigned inspector references)
        if (pepperUI != null)    pepperUI.alpha = 0;
        if (turmericUI != null)  turmericUI.alpha = 0;
        if (cardamomUI != null)  cardamomUI.alpha = 0;
        if (cinnamonUI != null)  cinnamonUI.alpha = 0;


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
            pepperUI,
            "Pepper is among the most sought-after goods in the market, prized by traders from distant lands.",
            pepperClip
        );


        yield return FocusOnSpice(
            turmericUI,
            "Turmeric is valued for its colour, flavour, and medicinal use.",
            turmericClip
        );


        yield return FocusOnSpice(
            cardamomUI,
            "Cardamom is rare and fragrant, often found in royal kitchens and temple offerings.",
            cardamomClip
        );


        yield return FocusOnSpice(
            cinnamonUI,
            "Cinnamon travels through long trade routes, making it one of the most precious goods in the market.",
            cinnamonClip
        );


        yield return ShowSubtitle(
            "Remember these goods well. Knowing their worth may decide the success of your trade.",
            endingClip
        );


        yield return FadeCanvas(subtitleCanvas, 0f, 1f);


        // go to next scene
        GameManager.Instance.LoadNextScene();
    }



    IEnumerator FocusOnSpice(
        CanvasGroup spiceUI,
        string narration,
        AudioClip clip)
    {
        if (spiceUI != null)
            yield return FadeCanvas(spiceUI, 1f, 0.5f);

        yield return ShowSubtitle(
            narration,
            clip
        );

        yield return new WaitForSeconds(1f);

        if (spiceUI != null)
            yield return FadeCanvas(spiceUI, 0f, 0.5f);
    }



    IEnumerator ShowSubtitle(string message, AudioClip clip)
    {
        subtitleText.text = message;


        if (clip != null && narratorAudioSource != null)
        {
            narratorAudioSource.clip = clip;
            narratorAudioSource.Play();

            yield return new WaitWhile(
                () => narratorAudioSource.isPlaying
            );
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }
    }



    IEnumerator FadeCanvas(
        CanvasGroup canvasGroup,
        float targetAlpha,
        float duration)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float timer = 0f;


        while (timer < duration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    timer / duration
                );

            yield return null;
        }


        canvasGroup.alpha = targetAlpha;
    }
}