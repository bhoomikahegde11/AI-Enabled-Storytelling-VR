using UnityEngine;

public class Customer : MonoBehaviour
{
    public string itemName;
    public float offerPrice;
    public float acceptMin;
    public float acceptMax;

    public bool CheckDeal(float playerPrice)
    {
        if (playerPrice >= acceptMin && playerPrice <= acceptMax)
        {
            return true;
        }
        return false;
    }
}