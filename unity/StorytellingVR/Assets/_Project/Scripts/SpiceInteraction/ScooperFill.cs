using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScooperFill : MonoBehaviour
{
    public SpiceVisualSet[] spiceVisuals;
    public float wrongSpiceClearDelay = 0.7f;
    public float zoneExitGracePeriod = 0.2f;

    private bool insideSack = false;
    private bool filled = false;
    public SpiceType currentSpice = SpiceType.None;

    private SpiceZone currentZone;
    private readonly HashSet<SpiceZone> overlappingZones = new HashSet<SpiceZone>();
    private Coroutine clearZoneCoroutine;
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
        insideSack = false;
        currentZone = null;
        currentSpice = SpiceType.None;
        overlappingZones.Clear();

        if (clearZoneCoroutine != null)
        {
            StopCoroutine(clearZoneCoroutine);
            clearZoneCoroutine = null;
        }

        ShowSpiceVisual(SpiceType.None);

        Debug.Log("Reset");
    }

    private void OnTriggerEnter(Collider other)
    {
        RefreshZoneState(FindSpiceZone(other));
    }

    private void OnTriggerStay(Collider other)
    {
        RefreshZoneState(FindSpiceZone(other));
    }

    private void OnTriggerExit(Collider other)
    {
        SpiceZone zone = FindSpiceZone(other);

        if (zone == null)
            return;

        overlappingZones.Remove(zone);

        if (overlappingZones.Count > 0)
        {
            currentZone = GetAnyOverlappingZone();
            insideSack = currentZone != null;
            return;
        }

        if (clearZoneCoroutine != null)
            StopCoroutine(clearZoneCoroutine);

        clearZoneCoroutine = StartCoroutine(ClearZoneAfterGracePeriod(zone));
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

    private void RefreshZoneState(SpiceZone zone)
    {
        if (zone == null)
            return;

        overlappingZones.Add(zone);
        currentZone = zone;
        insideSack = true;

        if (clearZoneCoroutine != null)
        {
            StopCoroutine(clearZoneCoroutine);
            clearZoneCoroutine = null;
        }
    }

    private IEnumerator ClearZoneAfterGracePeriod(SpiceZone exitedZone)
    {
        yield return new WaitForSeconds(zoneExitGracePeriod);

        if (overlappingZones.Count > 0)
        {
            currentZone = GetAnyOverlappingZone();
            insideSack = currentZone != null;
        }
        else if (currentZone == exitedZone || currentZone == null)
        {
            currentZone = null;
            insideSack = false;
        }

        clearZoneCoroutine = null;
    }

    private SpiceZone GetAnyOverlappingZone()
    {
        foreach (SpiceZone zone in overlappingZones)
        {
            if (zone != null)
                return zone;
        }

        return null;
    }

    void ShowSpiceVisual(SpiceType spice)
    {
        foreach (SpiceVisualSet item in spiceVisuals)
        {
            item.visual.SetActive(item.spiceType == spice);
        }
    }
}
