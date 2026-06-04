using UnityEngine;
using TMPro;

public class CoinInfoManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;

    public TextMeshProUGUI typeText;

    public TextMeshProUGUI descriptionText;



    void Awake()
    {
        gameObject.SetActive(false);
    }



    public void ShowInfo(
        string title,
        string type,
        string description
    )
    {
        gameObject.SetActive(true);


        titleText.text = title;

        typeText.text = type;

        descriptionText.text =
            description;
    }



    public void Hide()
    {
        gameObject.SetActive(false);
    }
}