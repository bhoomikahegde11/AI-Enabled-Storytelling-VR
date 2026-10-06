using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float speed = 1;
    public Color fadeColor = Color.black;

    private static ScreenFader instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Start transparent
        fadeImage.color = new Color(
            fadeColor.r,
            fadeColor.g,
            fadeColor.b,
            0f
        );
    }

    public void RepositionToCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null) canvas = GetComponentInParent<Canvas>();

            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                // Position in front of the camera
                transform.position = cam.transform.position + cam.transform.forward * 0.4f;
                // Match camera rotation so it stays face-on
                transform.rotation = cam.transform.rotation;
            }
        }
    }

    public IEnumerator WaitForCamera()
    {
        // Wait until Camera.main is available
        while (Camera.main == null)
        {
            yield return null;
        }

        // Wait one additional frame to ensure the XR rig has applied its initial tracking pose
        yield return null;

        RepositionToCamera();
    }

    public IEnumerator FadeOut()
    {
        float a = fadeImage.color.a;

        while (a < 1f)
        {
            a += Time.deltaTime * speed;

            fadeImage.color = new Color(
                fadeColor.r,
                fadeColor.g,
                fadeColor.b,
                Mathf.Clamp01(a)
            );

            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float a = fadeImage.color.a;

        while (a > 0f)
        {
            a -= Time.deltaTime * speed;

            fadeImage.color = new Color(
                fadeColor.r,
                fadeColor.g,
                fadeColor.b,
                Mathf.Clamp01(a)
            );

            yield return null;
        }
    }
}