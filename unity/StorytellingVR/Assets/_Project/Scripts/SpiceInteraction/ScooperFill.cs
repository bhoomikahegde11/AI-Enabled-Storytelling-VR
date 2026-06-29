using UnityEngine;
using System.Collections;
public class ScooperFill : MonoBehaviour
{
    public SpiceVisualSet[] spiceVisuals;

    private bool insideSack = false;
    private bool filled = false;
    public SpiceType currentSpice = SpiceType.None;
    private SpiceZone currentZone;
    public static ScooperFill Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ShowSpiceVisual(SpiceType.None);
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

        ShowSpiceVisual(currentSpice);

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyScooperFilled(currentSpice);


        OVRInput.SetControllerVibration(
        0.8f,
        0.8f,
        OVRInput.Controller.RTouch
    );
        StartCoroutine(StopHaptics());

    }

    public void ResetScooper()
    {
        filled = false;

        ShowSpiceVisual(SpiceType.None);

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

            if (SpiceTutorialManager.Instance != null)
                SpiceTutorialManager.Instance.NotifyScooperEnteredSack(zone.spiceType);
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
    public void EmptyScooper()
    {
        filled = false;

        currentSpice = SpiceType.None;

        ShowSpiceVisual(currentSpice);

        Debug.Log("Scooper Emptied");
    }
    void ShowSpiceVisual(SpiceType spice)
    {
        foreach (SpiceVisualSet item in spiceVisuals)
        {
            item.visual.SetActive(item.spiceType == spice);
        }
    }
}
