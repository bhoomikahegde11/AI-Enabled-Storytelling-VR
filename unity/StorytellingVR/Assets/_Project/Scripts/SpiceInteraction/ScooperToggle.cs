using UnityEngine;

public class ScooperToggle : MonoBehaviour
{
    public GameObject scooper;
    public GameObject controllerModel;

    private ScooperFill scooperFill;
    private Renderer[] controllerRenderers;
    private bool scooperWasVisible = false;

    void Start()
    {
        if (scooper == null) return;
        scooperFill = scooper.GetComponent<ScooperFill>();
        controllerRenderers = controllerModel != null
            ? controllerModel.GetComponentsInChildren<Renderer>(true) : new Renderer[0];
        ReleaseScooper();
    }

    void Update()
    {
        bool tracked = OVRInput.IsControllerConnected(OVRInput.Controller.RTouch) &&
            OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) &&
            OVRInput.GetControllerOrientationTracked(OVRInput.Controller.RTouch);
        UpdateHeldState(OVRInput.Get(OVRInput.RawButton.RIndexTrigger),
            PauseMenuController.GameplayInputBlocked, tracked);
    }

    private void UpdateHeldState(bool held, bool inputBlocked, bool tracked)
    {
        if (scooper == null) return;
        // Reconcile the held state: a release edge may have happened while paused or untracked.
        if (!held || !tracked)
        {
            if (scooper.activeSelf || scooperWasVisible) ReleaseScooper();
            return;
        }
        if (inputBlocked) return;
        // Trigger held
        if (held)
        {
            scooper.SetActive(true);

            SetControllerVisible(false);

            if (!scooperWasVisible)
            {
                scooperWasVisible = true;

                if (SpiceTutorialManager.Instance != null)
                    SpiceTutorialManager.Instance.NotifyScooperAppeared();
            }
        }

    }

    private void ReleaseScooper()
    {
        if (scooperFill == null && scooper != null) scooperFill = scooper.GetComponent<ScooperFill>();
        if (scooperFill != null) scooperFill.ResetScooper();
        if (scooper != null) scooper.SetActive(false);
        SetControllerVisible(true);
        scooperWasVisible = false;
    }

    private void OnDisable() => ReleaseScooper();
    private void OnApplicationFocus(bool focused) { if (!focused) ReleaseScooper(); }
    void SetControllerVisible(bool visible)
    {
        if (controllerRenderers == null) return;
        foreach (Renderer r in controllerRenderers)
        {
            if (r != null) r.enabled = visible;
        }
    }
}
