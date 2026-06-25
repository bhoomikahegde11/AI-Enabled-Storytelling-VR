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

        ScooperFill scooper = ScooperFill.Instance;

        if (scooper == null)
        {
            Debug.Log("Scooper doesn't exist!");
            return;
        }

        Debug.Log("Scooper Instance Found");
        Debug.Log("Scooper Filled: " + scooper.IsFilled());
        Debug.Log("Current Spice: " + scooper.currentSpice);

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

        if (scooper.currentSpice != OrderManager.Instance.requestedSpice)
        {
            Debug.Log("Wrong Spice!");

            return;
        }

        Debug.Log("Correct Spice!");

        customer.FillBag();

        scooper.EmptyScooper();

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