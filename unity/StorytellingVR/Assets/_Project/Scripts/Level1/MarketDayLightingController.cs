using UnityEngine;

public class MarketDayLightingController : MonoBehaviour
{
    private static readonly int BaseColorPropertyId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
    private static readonly int TintPropertyId = Shader.PropertyToID("_Tint");
    private static readonly int ExposurePropertyId = Shader.PropertyToID("_Exposure");

    [Header("Directional Light (Day to Evening)")]
    [SerializeField] private Color dayLightColor = new Color(1f, 0.95686275f, 0.8392157f, 1f);
    [SerializeField] private Color nightLightColor = new Color(1f, 0.62f, 0.34f, 1f);
    [SerializeField] private float dayLightIntensity = 1f;
    [SerializeField] private float nightLightIntensity = 0.45f;
    [SerializeField] private Vector3 dayRotation = new Vector3(50f, -30f, 0f);
    [SerializeField] private Vector3 nightRotation = new Vector3(18f, -95f, 0f);

    [Header("Ambient Lighting (Day to Evening)")]
    [SerializeField] private Color dayAmbientColor = new Color(0.212f, 0.227f, 0.259f, 1f);
    [SerializeField] private Color nightAmbientColor = new Color(0.42f, 0.24f, 0.16f, 1f);
    [SerializeField] private float dayAmbientIntensity = 1f;
    [SerializeField] private float nightAmbientIntensity = 0.55f;

    [Header("Backdrop (Day to Evening)")]
    [SerializeField] private Color dayBackgroundTint = Color.white;
    [SerializeField] private Color eveningBackgroundTint = new Color(1f, 0.78f, 0.6f, 1f);

    private Light directionalLight;
    private Renderer backgroundRenderer;
    private MaterialPropertyBlock backgroundPropertyBlock;
    private int backgroundColorPropertyId;
    private Material skyboxMaterial;
    private bool skyboxSupportsTint;
    private bool skyboxSupportsExposure;
    private Color initialSkyboxTint = Color.white;
    private float initialSkyboxExposure = 1f;
    private bool hasLoggedMissingLight;
    private bool hasLoggedMissingBackground;

    private void Awake()
    {
        CacheDirectionalLight();
        CacheBackgroundRenderer();
        CacheSkybox();
    }

    private void Update()
    {
        Level1GameState gameState = Level1GameState.ExistingInstance;
        if (gameState == null || !gameState.MarketDayStarted)
        {
            return;
        }

        float progress = gameState.MarketDayEnded
            ? 1f
            : gameState.MarketDayNormalizedProgress;

        ApplyLighting(progress);
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

    private void CacheSkybox()
    {
        if (skyboxMaterial != null)
        {
            return;
        }

        skyboxMaterial = RenderSettings.skybox;
        if (skyboxMaterial == null)
        {
            return;
        }

        skyboxSupportsTint = skyboxMaterial.HasProperty(TintPropertyId);
        skyboxSupportsExposure = skyboxMaterial.HasProperty(ExposurePropertyId);

        if (skyboxSupportsTint)
        {
            initialSkyboxTint = skyboxMaterial.GetColor(TintPropertyId);
        }

        if (skyboxSupportsExposure)
        {
            initialSkyboxExposure = skyboxMaterial.GetFloat(ExposurePropertyId);
        }
    }

    private void ApplyLighting(float progress)
    {
        CacheDirectionalLight();
        CacheBackgroundRenderer();
        CacheSkybox();

        float t = Mathf.Clamp01(progress);

        if (directionalLight != null)
        {
            directionalLight.color = Color.Lerp(dayLightColor, nightLightColor, t);
            directionalLight.intensity = Mathf.Lerp(dayLightIntensity, nightLightIntensity, t);
            directionalLight.transform.rotation = Quaternion.Euler(Vector3.Lerp(dayRotation, nightRotation, t));
        }

        Color ambient = Color.Lerp(dayAmbientColor, nightAmbientColor, t);
        RenderSettings.ambientSkyColor = ambient;
        RenderSettings.ambientEquatorColor = ambient * 0.7f;
        RenderSettings.ambientGroundColor = ambient * 0.35f;
        RenderSettings.ambientIntensity = Mathf.Lerp(dayAmbientIntensity, nightAmbientIntensity, t);
        RenderSettings.fog = false;

        ApplyBackgroundTint(t);
        ApplySkybox(t);
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
        if (skyboxMaterial == null)
        {
            return;
        }

        if (skyboxSupportsTint)
        {
            skyboxMaterial.SetColor(
                TintPropertyId,
                Color.Lerp(initialSkyboxTint, eveningBackgroundTint, t));
        }

        if (skyboxSupportsExposure)
        {
            skyboxMaterial.SetFloat(
                ExposurePropertyId,
                Mathf.Lerp(initialSkyboxExposure, initialSkyboxExposure * 0.75f, t));
        }
    }
}
