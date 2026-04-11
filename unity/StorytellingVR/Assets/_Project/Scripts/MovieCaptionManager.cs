using UnityEngine;
using TMPro;

public class MovieCaptionManager : MonoBehaviour
{
    public TextMeshProUGUI bottomCaptionText;

    // Call this from your Tutorial Sequence
    public void SetCaption(string text)
    {
        bottomCaptionText.text = text;
        // Optional: Trigger a subtle "ping" sound for immersion
    }

    public void ClearCaption()
    {
        bottomCaptionText.text = "";
    }
}