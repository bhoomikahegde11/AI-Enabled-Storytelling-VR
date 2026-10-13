using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 1;
    public Color fadeColor = Color.black;


    public IEnumerator FadeOut()
    {
        float a = 0;


        while (a < 1)
        {
            if (fadeImage == null) yield break;
            a += FadeDeltaTime * speed;


            fadeImage.color = new Color(
                fadeColor.r,
                fadeColor.g,
                fadeColor.b,
                a
            );


            yield return null;
        }
    }



    public IEnumerator FadeIn()
    {
        float a = 1;


        while (a > 0)
        {
            if (fadeImage == null) yield break;
            a -= FadeDeltaTime * speed;


            fadeImage.color = new Color(
                fadeColor.r,
                fadeColor.g,
                fadeColor.b,
                a
            );


            yield return null;
        }
    }

    private static float FadeDeltaTime => GameManager.ExistingInstance != null && GameManager.ExistingInstance.IsLoading && Time.timeScale <= 0f
        ? Time.unscaledDeltaTime : Time.deltaTime;
}
