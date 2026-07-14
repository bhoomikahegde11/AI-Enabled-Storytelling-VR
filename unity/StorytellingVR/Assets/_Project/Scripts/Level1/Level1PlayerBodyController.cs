using UnityEngine;

public class Level1PlayerBodyController : MonoBehaviour
{
    private const string GenderPlayerPrefsKey = "MainMenu.SelectedGender";

    [Header("Tracking")]
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, -0.95f, 0.08f);
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Body Visuals")]
    [SerializeField] private GameObject maleBodyRoot;
    [SerializeField] private GameObject femaleBodyRoot;
    [SerializeField] private Vector3 maleBodyLocalPosition = new Vector3(0f, -0.18f, 0.16f);
    [SerializeField] private Vector3 maleBodyLocalEulerAngles = Vector3.zero;
    [SerializeField] private Vector3 maleBodyLocalScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private string[] maleRenderersToDisable =
    {
        "Wolf3D_Head",
        "Wolf3D_Teeth",
        "Wolf3D_Hair",
        "Wolf3D_Outfit_Top"
    };

    private void Awake()
    {
        if (centerEyeAnchor == null)
        {
            centerEyeAnchor = transform.parent != null
                ? transform.parent.Find("CenterEyeAnchor")
                : null;
        }

        ApplySelectedBody();
        ConfigureBodyVisuals();
        UpdateBodyPose();
    }

    private void LateUpdate()
    {
        UpdateBodyPose();
    }

    private void ApplySelectedBody()
    {
        string selectedGender = PlayerPrefs.GetString(GenderPlayerPrefsKey, "Male");
        bool useFemale = string.Equals(selectedGender, "Female", System.StringComparison.OrdinalIgnoreCase);

        if (maleBodyRoot != null)
        {
            maleBodyRoot.SetActive(!useFemale);
        }

        if (femaleBodyRoot != null)
        {
            femaleBodyRoot.SetActive(useFemale);
        }
    }

    private void ConfigureBodyVisuals()
    {
        ConfigureBodyRoot(maleBodyRoot, maleBodyLocalPosition, maleBodyLocalEulerAngles, maleBodyLocalScale, maleRenderersToDisable);
    }

    private static void ConfigureBodyRoot(GameObject bodyRoot, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale, string[] renderersToDisable)
    {
        if (bodyRoot == null)
            return;

        Transform bodyTransform = bodyRoot.transform;
        bodyTransform.localPosition = localPosition;
        bodyTransform.localRotation = Quaternion.Euler(localEulerAngles);
        bodyTransform.localScale = localScale;

        if (renderersToDisable == null || renderersToDisable.Length == 0)
            return;

        var disabledNames = new System.Collections.Generic.HashSet<string>(renderersToDisable, System.StringComparer.Ordinal);
        foreach (Renderer renderer in bodyRoot.GetComponentsInChildren<Renderer>(true))
        {
            if (!disabledNames.Contains(renderer.gameObject.name))
                continue;

            renderer.enabled = false;
        }
    }

    private void UpdateBodyPose()
    {
        if (centerEyeAnchor == null)
            return;

        Vector3 localPosition = centerEyeAnchor.localPosition;
        transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z) + positionOffset;

        Vector3 euler = centerEyeAnchor.localEulerAngles;
        transform.localRotation = Quaternion.Euler(0f, euler.y, 0f) * Quaternion.Euler(rotationOffsetEuler);
    }
}
