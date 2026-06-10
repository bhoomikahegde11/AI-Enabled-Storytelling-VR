using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;

    public float speed = 1;


    public IEnumerator FadeOut()
    {
        float a = 0;


        while (a < 1)
        {
            a += Time.deltaTime * speed;


            fadeImage.color =
                new Color(
                    0,
                    0,
                    0,
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
            a -= Time.deltaTime * speed;


            fadeImage.color =
                new Color(
                    0,
                    0,
                    0,
                    a
                );


            yield return null;
        }
    }
}