using UnityEngine;

public class MarketDayLightingController : MonoBehaviour
{
    private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private static readonly int TintPropertyId = Shader.PropertyToID("_Tint");
    private static readonly int ExposurePropertyId = Shader.PropertyToID("_Exposure");
    private static readonly int RotationPropertyId = Shader.PropertyToID("_Rotation");

    [Header("References")]
    [SerializeField] private Material daySkyboxMaterial;

    [Header("Progress")]
    [SerializeField] private AnimationCurve atmosphereProgressCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Directional Light")]
    [SerializeField] private Color dayLightColor = new Color(1f, 0.95686275f, 0.8392157f, 1f);
    [SerializeField] private Color eveningLightColor = new Color(1f, 0.72f, 0.5f, 1f);
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float eveningLightIntensity = 0.68f;
    [SerializeField] private Vector3 dayLightEuler = new Vector3(45f, 90f, 0f);
    [SerializeField] private Vector3 eveningLightEuler = new Vector3(18f, 110f, 0f);
    [SerializeField] private float dayShadowStrength = 1f;
    [SerializeField] private float eveningShadowStrength = 0.78f;

    [Header("Skybox")]
    [SerializeField] private Color daySkyTint = new Color(0.8840241f, 0.7150535f, 0.57680476f, 0.88402414f);
    [SerializeField] private Color eveningSkyTint = new Color(1f, 0.86f, 0.76f, 0.92f);
    [SerializeField] private float daySkyExposure = 0.72495896f;
    [SerializeField] private float eveningSkyExposure = 0.58f;
    [SerializeField] private float daySkyRotation = 0f;
    [SerializeField] private float eveningSkyRotation = 0f;

    [Header("Ambient Lighting")]
    [SerializeField] private Color dayAmbientSkyColor = new Color(0.212f, 0.227f, 0.259f, 1f);
    [SerializeField] private Color eveningAmbientSkyColor = new Color(0.33f, 0.26f, 0.22f, 1f);
    [SerializeField] private Color dayAmbientEquatorColor = new Color(0.114f, 0.125f, 0.133f, 1f);
    [SerializeField] private Color eveningAmbientEquatorColor = new Color(0.24f, 0.19f, 0.16f, 1f);
    [SerializeField] private Color dayAmbientGroundColor = new Color(0.047f, 0.043f, 0.035f, 1f);
    [SerializeField] private Color eveningAmbientGroundColor = new Color(0.11f, 0.085f, 0.065f, 1f);
    [SerializeField] private float dayAmbientIntensity = 1f;
    [SerializeField] private float eveningAmbientIntensity = 0.72f;

    [Header("Fog")]
    [SerializeField] private bool enableAtmosphericFog = false;
    [SerializeField] private Color dayFogColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    [SerializeField] private Color eveningFogColor = new Color(0.78f, 0.62f, 0.52f, 1f);
    [SerializeField] private float dayFogDensity = 0.01f;
    [SerializeField] private float eveningFogDensity = 0.014f;

    [Header("Background Hill")]
    [SerializeField] private Color dayBackgroundTint = Color.white;
    [SerializeField] private Color eveningBackgroundTint = new Color(1f, 0.78f, 0.6f, 1f);

    [Header("Runtime Debug")]
    public bool usePreviewProgress;
    [Range(0f, 1f)] public float previewNormalizedProgress;

    private Light directionalLight;
    private Renderer backgroundRenderer;
    private MaterialPropertyBlock backgroundPropertyBlock;
    private int backgroundColorPropertyId;
    private Material runtimeSkyboxMaterial;
    private Material originalSkyboxMaterial;
    private bool hasLoggedMissingLight;
    private bool hasLoggedMissingBackground;
    private bool hasLoggedMissingSkybox;
    private bool atmosphereWasActiveLastFrame;
    private float previousNormalizedProgress = -1f;
    private bool giUpdatedAtStart;
    private bool giUpdatedAtMidpoint;
    private bool giUpdatedAtEnd;
    private bool originalFogEnabled;
    private FogMode originalFogMode;
    private Color originalFogColor;
    private float originalFogDensity;
    private float originalFogStartDistance;
    private float originalFogEndDistance;

    private void Awake()
    {
        CacheDirectionalLight();
        CacheBackgroundRenderer();
        CacheOriginalFogSettings();
        InitializeSkybox();
        ResetAtmosphereState();
    }

    private void Update()
    {
        bool shouldUsePreview = usePreviewProgress;
        Level1GameState gameState = Level1GameState.ExistingInstance;
        bool marketDayActive = gameState != null && gameState.MarketDayStarted;
        bool shouldDriveAtmosphere = shouldUsePreview || marketDayActive;

        if (!shouldDriveAtmosphere)
        {
            ResetAtmosphereState();
            atmosphereWasActiveLastFrame = false;
            previousNormalizedProgress = -1f;
            return;
        }

        float normalizedProgress = shouldUsePreview
            ? Mathf.Clamp01(previewNormalizedProgress)
            : (gameState.MarketDayEnded ? 1f : gameState.MarketDayNormalizedProgress);

        if (!atmosphereWasActiveLastFrame || normalizedProgress < previousNormalizedProgress)
        {
            ResetAtmosphereState();
        }

        ApplyAtmosphere(normalizedProgress);
        previousNormalizedProgress = normalizedProgress;
        atmosphereWasActiveLastFrame = true;
    }

    private void OnDestroy()
    {
        if (runtimeSkyboxMaterial != null && RenderSettings.skybox == runtimeSkyboxMaterial)
        {
            RenderSettings.skybox = originalSkyboxMaterial;
        }

        RestoreOriginalFogSettings();

        if (runtimeSkyboxMaterial != null)
        {
            Destroy(runtimeSkyboxMaterial);
        }
    }

    private void CacheDirectionalLight()
    {
        if (directionalLight != null)
        {
            return;
        }

        directionalLight = GetComponentInChildren<Light>(true);
        if (directionalLight == null || directionalLight.type != LightType.Directional)
        {
            Light[] lights = GetComponentsInChildren<Light>(true);
            foreach (Light candidate in lights)
            {
                if (candidate != null && candidate.type == LightType.Directional)
                {
                    directionalLight = candidate;
                    break;
                }
            }
        }

        if (directionalLight != null)
        {
            RenderSettings.sun = directionalLight;
        }
        else if (!hasLoggedMissingLight)
        {
            hasLoggedMissingLight = true;
            Debug.LogWarning("[MarketDayLightingController] No directional light was found under ENVIROMENT.");
        }
    }

    private void CacheBackgroundRenderer()
    {
        if (backgroundRenderer != null)
        {
            return;
        }

        Transform backgroundTransform = transform.Find("BG_Hampi_Hill_Image");
        if (backgroundTransform == null)
        {
            backgroundTransform = transform.Find("ENVIROMENT/BG_Hampi_Hill_Image");
        }

        if (backgroundTransform != null)
        {
            backgroundRenderer = backgroundTransform.GetComponent<Renderer>();
        }

        if (backgroundRenderer == null)
        {
            backgroundRenderer = GetBackgroundRendererFromChildren();
        }

        if (backgroundRenderer != null)
        {
            Material sharedMaterial = backgroundRenderer.sharedMaterial;
            if (sharedMaterial != null)
            {
                if (sharedMaterial.HasProperty(BaseColorPropertyId))
                {
                    backgroundColorPropertyId = BaseColorPropertyId;
                }
                else if (sharedMaterial.HasProperty(ColorPropertyId))
                {
                    backgroundColorPropertyId = ColorPropertyId;
                }
            }

            backgroundPropertyBlock ??= new MaterialPropertyBlock();
        }
        else if (!hasLoggedMissingBackground)
        {
            hasLoggedMissingBackground = true;
            Debug.LogWarning("[MarketDayLightingController] BG_Hampi_Hill_Image was not found under ENVIROMENT.");
        }
    }

    private Renderer GetBackgroundRendererFromChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer candidate in renderers)
        {
            if (candidate != null && candidate.name == "BG_Hampi_Hill_Image")
            {
                return candidate;
            }
        }

        return null;
    }

    private void CacheOriginalFogSettings()
    {
        originalFogEnabled = RenderSettings.fog;
        originalFogMode = RenderSettings.fogMode;
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogStartDistance = RenderSettings.fogStartDistance;
        originalFogEndDistance = RenderSettings.fogEndDistance;
    }

    private void RestoreOriginalFogSettings()
    {
        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogMode = originalFogMode;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;
        RenderSettings.fogStartDistance = originalFogStartDistance;
        RenderSettings.fogEndDistance = originalFogEndDistance;
    }

    private void InitializeSkybox()
    {
        originalSkyboxMaterial = RenderSettings.skybox;

        Material sourceSkybox = daySkyboxMaterial != null ? daySkyboxMaterial : RenderSettings.skybox;
        if (sourceSkybox == null)
        {
            if (!hasLoggedMissingSkybox)
            {
                hasLoggedMissingSkybox = true;
                Debug.LogWarning("[MarketDayLightingController] No day skybox material is assigned. Atmospheric skybox controls are disabled.");
            }

            return;
        }

        runtimeSkyboxMaterial = new Material(sourceSkybox);
        runtimeSkyboxMaterial.name = $"{sourceSkybox.name} (Runtime)";
        RenderSettings.skybox = runtimeSkyboxMaterial;
    }

    private void ResetAtmosphereState()
    {
        giUpdatedAtStart = false;
        giUpdatedAtMidpoint = false;
        giUpdatedAtEnd = false;
        ApplyAtmosphereImmediate(0f);
    }

    private void ApplyAtmosphere(float normalizedProgress)
    {
        float curvedProgress = EvaluateProgress(normalizedProgress);
        ApplyDirectionalLight(curvedProgress);
        ApplyAmbientLighting(curvedProgress);
        ApplyFog(curvedProgress);
        ApplyBackgroundTint(curvedProgress);
        ApplySkybox(curvedProgress);
        UpdateGiMilestones(curvedProgress);
    }

    private void ApplyAtmosphereImmediate(float normalizedProgress)
    {
        float curvedProgress = EvaluateProgress(normalizedProgress);
        ApplyDirectionalLight(curvedProgress);
        ApplyAmbientLighting(curvedProgress);
        ApplyFog(curvedProgress);
        ApplyBackgroundTint(curvedProgress);
        ApplySkybox(curvedProgress);
    }

    private float EvaluateProgress(float normalizedProgress)
    {
        float clampedProgress = Mathf.Clamp01(normalizedProgress);
        if (atmosphereProgressCurve == null || atmosphereProgressCurve.length == 0)
        {
            return clampedProgress;
        }

        return Mathf.Clamp01(atmosphereProgressCurve.Evaluate(clampedProgress));
    }

    private void ApplyDirectionalLight(float t)
    {
        if (directionalLight == null)
        {
            return;
        }

        directionalLight.color = Color.Lerp(dayLightColor, eveningLightColor, t);
        directionalLight.intensity = Mathf.Lerp(dayLightIntensity, eveningLightIntensity, t);
        directionalLight.transform.rotation = Quaternion.Euler(Vector3.Lerp(dayLightEuler, eveningLightEuler, t));

        if (directionalLight.shadows != LightShadows.None)
        {
            directionalLight.shadowStrength = Mathf.Lerp(dayShadowStrength, eveningShadowStrength, t);
        }
    }

    private void ApplyAmbientLighting(float t)
    {
        RenderSettings.ambientSkyColor = Color.Lerp(dayAmbientSkyColor, eveningAmbientSkyColor, t);
        RenderSettings.ambientEquatorColor = Color.Lerp(dayAmbientEquatorColor, eveningAmbientEquatorColor, t);
        RenderSettings.ambientGroundColor = Color.Lerp(dayAmbientGroundColor, eveningAmbientGroundColor, t);
        RenderSettings.ambientIntensity = Mathf.Lerp(dayAmbientIntensity, eveningAmbientIntensity, t);
    }

    private void ApplyFog(float t)
    {
        if (!enableAtmosphericFog)
        {
            RenderSettings.fog = false;
            return;
        }

        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.Lerp(dayFogColor, eveningFogColor, t);
        RenderSettings.fogDensity = Mathf.Lerp(dayFogDensity, eveningFogDensity, t);
    }

    private void ApplyBackgroundTint(float t)
    {
        if (backgroundRenderer == null || backgroundColorPropertyId == 0 || backgroundPropertyBlock == null)
        {
            return;
        }

        backgroundRenderer.GetPropertyBlock(backgroundPropertyBlock);
        backgroundPropertyBlock.SetColor(
            backgroundColorPropertyId,
            Color.Lerp(dayBackgroundTint, eveningBackgroundTint, t));
        backgroundRenderer.SetPropertyBlock(backgroundPropertyBlock);
    }

    private void ApplySkybox(float t)
    {
        if (runtimeSkyboxMaterial == null)
        {
            return;
        }

        RenderSettings.skybox = runtimeSkyboxMaterial;

        if (runtimeSkyboxMaterial.HasProperty(TintPropertyId))
        {
            runtimeSkyboxMaterial.SetColor(TintPropertyId, Color.Lerp(daySkyTint, eveningSkyTint, t));
        }

        if (runtimeSkyboxMaterial.HasProperty(ExposurePropertyId))
        {
            runtimeSkyboxMaterial.SetFloat(ExposurePropertyId, Mathf.Lerp(daySkyExposure, eveningSkyExposure, t));
        }

        if (runtimeSkyboxMaterial.HasProperty(RotationPropertyId))
        {
            runtimeSkyboxMaterial.SetFloat(RotationPropertyId, Mathf.Lerp(daySkyRotation, eveningSkyRotation, t));
        }
    }

    private void UpdateGiMilestones(float t)
    {
        if (t >= 0.1f && !giUpdatedAtStart)
        {
            giUpdatedAtStart = true;
            DynamicGI.UpdateEnvironment();
        }

        if (t >= 0.5f && !giUpdatedAtMidpoint)
        {
            giUpdatedAtMidpoint = true;
            DynamicGI.UpdateEnvironment();
        }

        if (t >= 0.95f && !giUpdatedAtEnd)
        {
            giUpdatedAtEnd = true;
            DynamicGI.UpdateEnvironment();
        }
    }
}
