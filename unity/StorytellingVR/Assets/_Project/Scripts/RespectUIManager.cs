using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RespectUIManager : MonoBehaviour
{
    public Slider respectSlider;
    public Image fillImage;
    public TMP_Text respectValueText;

    private float displayedRespect;
    private float targetRespect;

    void Start()
    {
        displayedRespect = respectSlider.value;
        targetRespect = respectSlider.value;
        UpdateColor();
    }

    void Update()
    {
        displayedRespect = Mathf.Lerp(displayedRespect, targetRespect, Time.deltaTime * 3f);
        respectSlider.value = displayedRespect;
        respectValueText.text = Mathf.RoundToInt(displayedRespect).ToString();

        UpdateColor();
    }

    public void SetRespect(float newRespect)
    {
        targetRespect = Mathf.Clamp(newRespect, 0, 100);
    }

    void UpdateColor()
    {
        if (displayedRespect > 70)
        {
            fillImage.color = new Color(0.2f, 0.8f, 0.2f);
        }
        else if (displayedRespect > 40)
        {
            fillImage.color = new Color(1f, 0.75f, 0.1f);
        }
        else
        {
            fillImage.color = new Color(0.9f, 0.15f, 0.15f);
        }
    }
}