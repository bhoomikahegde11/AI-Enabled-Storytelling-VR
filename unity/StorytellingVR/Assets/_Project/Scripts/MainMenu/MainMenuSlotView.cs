using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only: receives the existing save-flow result and never reads or writes saves.
public sealed class MainMenuSlotView : MonoBehaviour
{
    public TMP_Text Title { get; private set; }
    public TMP_Text Location { get; private set; }
    public TMP_Text SavedDate { get; private set; }
    public RectTransform Content { get; private set; }

    public void Initialize(TMP_Text title, TMP_Text metadataStyle)
    {
        Title = title;
        var content = new GameObject("Slot text", typeof(RectTransform), typeof(RectMask2D));
        Content = (RectTransform)content.transform;
        Content.SetParent(transform, false);
        Content.anchorMin = Content.anchorMax = Content.pivot = new Vector2(0.5f, 0.5f);
        Content.anchoredPosition = new Vector2(0f, -32f);
        Content.sizeDelta = new Vector2(664f, 130f);
        // Clip text only: preserve the ornamental frame and the original button/raycast target.
        Title.rectTransform.SetParent(Content, false);
        Location = Instantiate(metadataStyle, Content);
        Location.name = "Checkpoint";
        SavedDate = Instantiate(metadataStyle, Content);
        SavedDate.name = "Saved date";
        Layout(Title, 37f, 44f, 36f);
        Layout(Location, -7f, 32f, 27f);
        Layout(SavedDate, -43f, 28f, 23f);
    }

    private static void Layout(TMP_Text text, float y, float height, float size)
    {
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(640f, height);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        Vector3 position = rect.localPosition;
        position.z = 0f;
        rect.localPosition = position;
        text.fontSize = size;
        text.enableAutoSizing = false;
        text.fontSizeMin = size;
        text.fontSizeMax = size;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.maxVisibleLines = 1;
        text.lineSpacing = text.paragraphSpacing = text.wordSpacing = 0f;
        text.margin = Vector4.zero;
        text.richText = false;
        text.raycastTarget = false;
        text.maskable = true;
        text.characterSpacing = 0f;
        text.gameObject.SetActive(true);
    }

    public void SetContent(int slot, string journeyName, string chapter, string savedDate,
        bool hasData, bool valid, bool resumable, bool interactable)
    {
        // NoWrap still permits explicit newlines. Normalize only the display copy, never the save.
        string name = string.IsNullOrWhiteSpace(journeyName) ? "Slot " + slot :
            journeyName.Trim().Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ')
                .Replace('\u2028', ' ').Replace('\u2029', ' ');
        Title.text = !hasData ? "Slot " + slot + " - Empty Slot" : !valid ? "Slot " + slot + " - Save needs repair" : name;
        Location.text = !hasData ? "Available for a new journey" : !valid ? "Existing data has been preserved" :
            "Slot " + slot + "  /  " + (resumable ? chapter : "Checkpoint unavailable");
        SavedDate.text = valid && hasData ? "Saved " + savedDate : string.Empty;
        Title.color = new Color(0.96f, 0.91f, 0.79f, interactable ? 1f : 0.72f);
        Location.color = new Color(0.88f, 0.80f, 0.63f, interactable ? 1f : 0.72f);
        SavedDate.color = new Color(0.80f, 0.75f, 0.65f, interactable ? 1f : 0.72f);
    }
}
