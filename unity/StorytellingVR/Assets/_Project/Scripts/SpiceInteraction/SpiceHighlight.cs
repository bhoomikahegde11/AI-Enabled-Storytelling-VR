using UnityEngine;

public class SpiceHighlight : MonoBehaviour
{
    public SpiceType spiceType;

    public GameObject glowObject;

    void Update()
    {
        if (OrderManager.Instance == null)
            return;

        glowObject.SetActive(
            spiceType == OrderManager.Instance.requestedSpice
        );
    }
}