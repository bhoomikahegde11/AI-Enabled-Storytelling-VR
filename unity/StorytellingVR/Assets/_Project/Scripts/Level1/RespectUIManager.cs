using UnityEngine;
using UnityEngine.UI;

public class RespectUIManager : MonoBehaviour
{
    [Header("Respect Visuals")]
    public Image fillImage;

    private float currentRespect = 100f;

    public void SetRespect(float newRespect)
    {
        currentRespect = Mathf.Clamp(
            newRespect,
            0f,
            100f
        );

        UpdateColor();
    }

    void UpdateColor()
    {
        if (fillImage == null)
            return;

        if (currentRespect > 70f)
        {
            fillImage.color =
                new Color(0.2f, 0.8f, 0.2f);
        }
        else if (currentRespect > 40f)
        {
            fillImage.color =
                new Color(1f, 0.75f, 0.1f);
        }
        else
        {
            fillImage.color =
                new Color(0.9f, 0.15f, 0.15f);
        }
    }
}