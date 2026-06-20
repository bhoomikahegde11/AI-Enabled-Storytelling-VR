using UnityEngine;

public class ScooperToggle : MonoBehaviour
{
    public GameObject scooper;
    public GameObject controllerModel;

    private ScooperFill scooperFill;
    private Renderer[] controllerRenderers;

    void Start()
    {
        scooperFill = scooper.GetComponent<ScooperFill>();

        scooper.SetActive(false);

        controllerRenderers =
            controllerModel.GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        // Trigger held
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
        {
            scooper.SetActive(true);

            SetControllerVisible(false);
        }

        // Trigger released
        if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
        {
            scooper.SetActive(false);

            SetControllerVisible(true);

            scooperFill.ResetScooper();
        }
    }
    void SetControllerVisible(bool visible)
    {
        foreach (Renderer r in controllerRenderers)
        {
            r.enabled = visible;
        }
    }
}