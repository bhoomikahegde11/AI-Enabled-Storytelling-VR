using UnityEngine;
using System.Collections;
public class ScooperFill : MonoBehaviour
{
    public GameObject cardamomVisual;

    private bool insideSack = false;
    private bool filled = false;

    void Start()
    {
        cardamomVisual.SetActive(false);
    }

    void Update()
    {
        // If holding trigger and inside sack
        if (!filled &&
            insideSack &&
            OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            FillScooper();
        }
    }

    void FillScooper()
    {
        filled = true;

        cardamomVisual.SetActive(true);
        OVRInput.SetControllerVibration(
            1f,
            1f,
            OVRInput.Controller.RTouch
        );
        StartCoroutine(StopHaptics());
        
    }

    public void ResetScooper()
    {
        filled = false;

        cardamomVisual.SetActive(false);

        Debug.Log("Reset");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered " + other.name);

        if (other.CompareTag("CardamomZone"))
        {
            insideSack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited " + other.name);

        if (other.CompareTag("CardamomZone"))
        {
            insideSack = false;
        }
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