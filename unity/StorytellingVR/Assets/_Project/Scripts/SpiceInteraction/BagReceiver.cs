using UnityEngine;

public class BagReceiver : MonoBehaviour
{
    public HandBagAnimation customer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bag Trigger Hit: " + other.name);
        ScooperFill scooper =
            other.GetComponent<ScooperFill>();

        if (scooper == null)
            return;

        if (!scooper.IsFilled())
            return;

        if (
            scooper.currentSpice ==
            OrderManager.Instance.requestedSpice
        )
        {
            customer.ReceiveBag();
        }
        else
        {
            Debug.Log("Wrong Spice");
        }
    }
}