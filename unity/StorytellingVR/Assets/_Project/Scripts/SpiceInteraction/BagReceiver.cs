using UnityEngine;

public class BagReceiver : MonoBehaviour
{
    public HandBagAnimation customer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "BAG RECEIVED: " +
            other.name
        );

        ScooperFill scooper =
            other.GetComponent<ScooperFill>();

        if (scooper == null)
        {
            Debug.Log("No ScooperFill found");
            return;
        }

        Debug.Log("Scooper Found");
    }
}