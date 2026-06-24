using UnityEngine;
using System.Collections;
public class ScooperFill : MonoBehaviour
{
    public GameObject cardamomVisual;

    private bool insideSack = false;
    private bool filled = false;
    public SpiceType currentSpice = SpiceType.None;
    private SpiceZone currentZone;
    void Start()
    {
        cardamomVisual.SetActive(false);
    }

    void Update()
    {
        if (!filled && insideSack && currentZone != null && OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            currentSpice = currentZone.spiceType;

            FillScooper();
        }
    }

    void FillScooper()
    {
        if (filled) return;

        filled = true;

        cardamomVisual.SetActive(true);

        Debug.Log(
            "Filled with " +
            currentSpice
        );
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

        SpiceZone zone = other.GetComponent<SpiceZone>();

        if (zone != null)
        {
            insideSack = true;
            currentZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited " + other.name);

        SpiceZone zone = other.GetComponent<SpiceZone>();

        if (zone != null)
        {
            insideSack = false;
            currentZone = null;
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
    public bool IsFilled()
    {
        return filled;
    }
}