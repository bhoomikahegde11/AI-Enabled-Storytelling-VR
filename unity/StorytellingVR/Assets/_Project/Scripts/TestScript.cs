using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text responseText;

    public void WhatAreYouSelling()
    {
        responseText.text =
            "I sell fine silk fabrics woven by skilled artisans throughout the Vijayanagara Empire.";
    }

    public void SilkTypes()
    {
        responseText.text =
            "We offer plain silk, embroidered silk, and luxurious patterned silk favored by nobles.";
    }

    public void SilkCost()
    {
        responseText.text =
            "The cost depends on quality, color, and craftsmanship.";
    }

    public void TradeInfo()
    {
        responseText.text =
            "Silk was one of the important trade goods sold in the markets of Hampi.";
    }
}