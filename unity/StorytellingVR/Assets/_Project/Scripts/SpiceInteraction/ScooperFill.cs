using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScooperFill : MonoBehaviour
{
    public SpiceVisualSet[] spiceVisuals;
    public float wrongSpiceClearDelay = 0.7f;

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
        // Not inside any sack
        if (!insideSack)
            return;

        // Already holding spice
        if (filled)
            return;

        // Somehow no current zone
        if (currentZone == null)
            return;

        // Player isn't holding the trigger
        if (!OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            return;

        currentSpice = currentZone.spiceType;

        FillScooper();

        if (currentSpice != OrderManager.Instance.requestedSpice)
        {
            StartCoroutine(ClearWrongSpiceAfterDelay());
        }
    }

    void FillScooper()
    {
        if (filled) return;

        filled = true;

        ShowSpiceVisual(currentSpice);

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyScooperFilled(currentSpice);

        if (OrderManager.Instance != null &&
            currentSpice != OrderManager.Instance.requestedSpice)
        {
            StartCoroutine(ClearWrongSpiceAfterDelay());
        }

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
        SpiceZone zone = other.GetComponent<SpiceZone>();

        if (zone == null)
            return;

        currentZone = zone;
        insideSack = true;
    }
    private void OnTriggerExit(Collider other)
    {
        SpiceZone zone = FindSpiceZone(other);

        if (zone == currentZone)
        {
            currentZone = null;
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

    IEnumerator ClearWrongSpiceAfterDelay()
    {
        yield return new WaitForSeconds(wrongSpiceClearDelay);

        if (filled &&
            OrderManager.Instance != null &&
            currentSpice != OrderManager.Instance.requestedSpice)
        {
            EmptyScooper();
        }
    }

    

    SpiceZone FindSpiceZone(Collider other)
    {
        SpiceZone zone = other.GetComponent<SpiceZone>();
        if (zone != null)
            return zone;

        zone = other.GetComponentInParent<SpiceZone>();
        if (zone != null)
            return zone;

        return other.GetComponentInChildren<SpiceZone>();
    }

    void ShowSpiceVisual(SpiceType spice)
    {
        foreach (SpiceVisualSet item in spiceVisuals)
        {
            item.visual.SetActive(item.spiceType == spice);
        }
    }
}
