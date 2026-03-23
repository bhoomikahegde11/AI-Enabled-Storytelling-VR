using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CostBoardRow : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Image background;

    Color normalColor = Color.clear;
    Color highlightColor = new Color(1f, 0.95f, 0.6f, 0.5f);

    public void Set(string displayName, float cost)
    {
        if (nameText != null) nameText.text = displayName;
        if (costText != null) costText.text = "Cost: " + cost.ToString("F0");
        if (background != null) background.color = normalColor;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (background != null)
            background.color = highlighted ? highlightColor : normalColor;
    }
}
