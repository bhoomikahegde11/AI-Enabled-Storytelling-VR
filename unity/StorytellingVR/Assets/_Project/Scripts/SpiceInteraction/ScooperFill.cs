using UnityEngine;
using System.Collections;
public class ScooperFill : MonoBehaviour
{
    public SpiceVisualSet[] spiceVisuals;
    public float wrongSpiceClearDelay = 0.7f;

    private bool filled;
    public SpiceType currentSpice = SpiceType.None;

    private Coroutine wrongSpiceClearCoroutine;
    public static ScooperFill Instance;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ShowSpiceVisual(SpiceType.None);
    }

    void FillScooper()
    {
        if (filled) return;

        filled = true;

        ShowSpiceVisual(currentSpice);

        if (SpiceTutorialManager.Instance != null)
            SpiceTutorialManager.Instance.NotifyScooperFilled(currentSpice);

        if (OrderManager.Instance != null &&
            currentSpice != OrderManager.Instance.ExpectedSpice)
        {
            if (wrongSpiceClearCoroutine != null) StopCoroutine(wrongSpiceClearCoroutine);
            wrongSpiceClearCoroutine = StartCoroutine(ClearWrongSpiceAfterDelay());
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
        if (wrongSpiceClearCoroutine != null)
        {
            StopCoroutine(wrongSpiceClearCoroutine);
            wrongSpiceClearCoroutine = null;
        }
        EmptyScooper();
    }

    private void OnTriggerEnter(Collider other) => TryFillFromOpening(other, IsScoopTriggerHeld());
    private void OnTriggerStay(Collider other) => TryFillFromOpening(other, IsScoopTriggerHeld());

    private static bool IsScoopTriggerHeld() => OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

    private void TryFillFromOpening(Collider other, bool triggerHeld)
    {
        if (filled || !triggerHeld || PauseMenuController.GameplayInputBlocked)
            return;

        if (!other.TryGetComponent(out SpiceZone zone) || other != zone.fillOpening)
            return;

        currentSpice = zone.spiceType;
        FillScooper();
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
        wrongSpiceClearCoroutine = null;

        if (filled &&
            OrderManager.Instance != null &&
            currentSpice != OrderManager.Instance.ExpectedSpice)
        {
            EmptyScooper();
        }
    }

    void ShowSpiceVisual(SpiceType spice)
    {
        if (spiceVisuals == null) return;
        foreach (SpiceVisualSet item in spiceVisuals)
        {
            if (item != null && item.visual != null) item.visual.SetActive(item.spiceType == spice);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
