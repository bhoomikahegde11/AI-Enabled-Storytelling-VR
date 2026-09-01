using UnityEngine;
using Oculus.Interaction.Locomotion;

public class Level1PlayerBodyController : MonoBehaviour
{
    private const string GenderPlayerPrefsKey = "MainMenu.SelectedGender";

    private enum BodySelectionMode
    {
        UseSavedGender,
        ForceMale,
        ForceFemale
    }

    [Header("Tracking")]
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, -0.95f, 0.08f);
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Body Visuals")]
    [SerializeField] private BodySelectionMode bodySelectionMode = BodySelectionMode.UseSavedGender;
    [SerializeField] private GameObject maleBodyRoot;
    [SerializeField] private GameObject femaleBodyRoot;
    [SerializeField] private Vector3 maleBodyLocalPosition = new Vector3(0f, -0.18f, 0.16f);
    [SerializeField] private Vector3 maleBodyLocalEulerAngles = Vector3.zero;
    [SerializeField] private Vector3 maleBodyLocalScale = new Vector3(0.9f, 0.9f, 0.9f);
    [SerializeField] private Vector3 femaleBodyLocalPosition = Vector3.zero;
    [SerializeField] private Vector3 femaleBodyLocalEulerAngles = Vector3.zero;
    [SerializeField] private Vector3 femaleBodyLocalScale = Vector3.one;
    [SerializeField] private string[] maleRenderersToDisable =
    {
        "Wolf3D_Head",
        "Wolf3D_Teeth",
        "Wolf3D_Hair",
        "Wolf3D_Outfit_Top"
    };
    [SerializeField] private string[] femaleRenderersToDisable = { };

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
        DisableLevel1PositionalLocomotion();
        UpdateBodyPose();
    }

    private void LateUpdate()
    {
        UpdateBodyPose();
    }

    private void ApplySelectedBody()
    {
        bool useFemale = ResolveUseFemale();

        if (maleBodyRoot != null)
            maleBodyRoot.SetActive(!useFemale);

        if (femaleBodyRoot != null)
            femaleBodyRoot.SetActive(useFemale);
    }

    private void ConfigureBodyVisuals()
    {
        ConfigureBodyRoot(maleBodyRoot, maleBodyLocalPosition, maleBodyLocalEulerAngles, maleBodyLocalScale, maleRenderersToDisable);
        ConfigureBodyRoot(femaleBodyRoot, femaleBodyLocalPosition, femaleBodyLocalEulerAngles, femaleBodyLocalScale, femaleRenderersToDisable);
    }

    private void DisableLevel1PositionalLocomotion()
    {
        // Level 1 is stall-bound: retain locomotion rotation handling while blocking velocity and jump translation.
        FirstPersonLocomotor locomotor = transform.root.GetComponentInChildren<FirstPersonLocomotor>(true);
        if (locomotor == null)
            return;

        locomotor.DisableMovement();
        locomotor.Velocity = Vector3.zero;
        locomotor.JumpForce = 0f;
    }

    private bool ResolveUseFemale()
    {
        switch (bodySelectionMode)
        {
            case BodySelectionMode.ForceMale:
                return false;
            case BodySelectionMode.ForceFemale:
                return true;
            default:
                string selectedGender = PlayerPrefs.GetString(GenderPlayerPrefsKey, "Male");
                return string.Equals(selectedGender, "Female", System.StringComparison.OrdinalIgnoreCase);
        }
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
