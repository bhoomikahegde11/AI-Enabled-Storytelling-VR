using UnityEngine;
using System.Collections;
public class BagReceiver : MonoBehaviour
{
    public HandBagAnimation customer;

    private bool completed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
            return;

        Debug.Log(
            "BAG RECEIVED: " +
            other.name
        );

        ScooperFill scooper =
     other.GetComponentInParent<ScooperFill>();

        if (scooper == null)
        {
            Debug.Log("No ScooperFill found");
            return;
        }

        if (!scooper.IsFilled())
        {
            Debug.Log("Scooper Empty");
            return;
        }

        completed = true;

        Debug.Log("Correct Spice");
        OVRInput.SetControllerVibration(
        1f,
        1f,
        OVRInput.Controller.RTouch
    );

        customer.FillBag();

        scooper.EmptyScooper();

        StartCoroutine(StopHaptics());

        
    }
    IEnumerator StopHaptics()
    {
        yield return new WaitForSeconds(0.15f);

        OVRInput.SetControllerVibration(
            0,
            0,
            OVRInput.Controller.RTouch
        );
    }
}