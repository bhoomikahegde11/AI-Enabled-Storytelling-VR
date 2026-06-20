using UnityEngine;

public class ScooperToggle : MonoBehaviour
{
    public GameObject scooper;
    public GameObject controllerModel;

    private Renderer[] controllerRenderers;
    private bool scooperActive = false;

    void Start()
    {
        scooper.SetActive(false);

        controllerRenderers =
            controllerModel.GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            scooperActive = !scooperActive;

            scooper.SetActive(scooperActive);

            foreach (Renderer r in controllerRenderers)
            {
                r.enabled = !scooperActive;
            }
        }
    }
}