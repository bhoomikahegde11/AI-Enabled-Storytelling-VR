using UnityEngine;
using TMPro;

public class CoinInfoManager : MonoBehaviour
{
    public GameObject panel;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI descriptionText;


    void Awake()
    {
        panel.SetActive(false);
    }


    public void ShowInfo(
        string title,
        string type,
        string description
    )
    {
        panel.SetActive(true);

        titleText.text = title;
        typeText.text = type;
        descriptionText.text = description;
    }


    public void Hide()
    {
        panel.SetActive(false);
    }
}