using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class TempleMaterialAutoAssigner : MonoBehaviour
{
    private enum TempleMaterialGroup
    {
        UpperGopura,
        BorderLedge,
        PillarBase,
        Top,
        FallbackMidStone
    }

    [Header("Application")]
    public bool applyOnStart = true;
    public bool applyOnValidate = false;

    [Header("Colors")]
    public Color upperGopuraColor = new Color32(0xC9, 0xC0, 0xB1, 0xFF);
    public Color borderLedgeColor = new Color32(0x8B, 0x82, 0x73, 0xFF);
    public Color pillarBaseColor = new Color32(0x7E, 0x76, 0x6A, 0xFF);
    public Color topColor = new Color32(0xD6, 0xCC, 0xBC, 0xFF);
    public Color fallbackMidStoneColor = new Color32(0xA9, 0x9B, 0x86, 0xFF);

    [Header("Smoothness")]
    [Range(0f, 1f)] public float upperGopuraSmoothness = 0.2f;
    [Range(0f, 1f)] public float borderLedgeSmoothness = 0.18f;
    [Range(0f, 1f)] public float pillarBaseSmoothness = 0.16f;
    [Range(0f, 1f)] public float topSmoothness = 0.2f;
    [Range(0f, 1f)] public float fallbackSmoothness = 0.18f;

    private readonly Dictionary<TempleMaterialGroup, Material> cachedMaterials = new Dictionary<TempleMaterialGroup, Material>();
    private bool isApplying;

    private void OnEnable()
    {
        if (applyOnStart && !isApplying)
        {
            ApplyMaterials();
        }
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled || !applyOnValidate || isApplying)
        {
            return;
        }

        ApplyMaterials();
    }

    private void OnDestroy()
    {
        foreach (Material material in cachedMaterials.Values)
        {
            if (material == null)
            {
                continue;
            }

            if (Application.isPlaying)
            {
                Destroy(material);
            }
            else
            {
                DestroyImmediate(material);
            }
        }

        cachedMaterials.Clear();
    }

    [ContextMenu("Apply Temple Materials")]
    public void ApplyMaterials()
    {
        if (isApplying)
        {
            return;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            Debug.LogWarning($"{nameof(TempleMaterialAutoAssigner)} found no child renderers under {name}.", this);
            return;
        }

        isApplying = true;

        try
        {
            Dictionary<TempleMaterialGroup, int> counts = new Dictionary<TempleMaterialGroup, int>();

            foreach (Renderer renderer in renderers)
            {
                TempleMaterialGroup group = ResolveGroup(renderer.gameObject.name);
                Material sharedMaterial = GetOrCreateMaterial(group);
                Material[] sharedMaterials = renderer.sharedMaterials;

                if (sharedMaterials == null || sharedMaterials.Length == 0)
                {
                    renderer.sharedMaterial = sharedMaterial;
                }
                else
                {
                    bool changed = false;

                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        if (sharedMaterials[i] == sharedMaterial)
                        {
                            continue;
                        }

                        sharedMaterials[i] = sharedMaterial;
                        changed = true;
                    }

                    if (changed)
                    {
                        renderer.sharedMaterials = sharedMaterials;
                    }
                }

                counts[group] = counts.TryGetValue(group, out int currentCount) ? currentCount + 1 : 1;
            }

            LogSummary(renderers.Length, counts);
        }
        finally
        {
            isApplying = false;
        }
    }

    private TempleMaterialGroup ResolveGroup(string objectName)
    {
        string normalizedName = objectName.ToLowerInvariant();

        if (normalizedName.Contains("gopura"))
        {
            return TempleMaterialGroup.UpperGopura;
        }

        if (normalizedName.Contains("border"))
        {
            return TempleMaterialGroup.BorderLedge;
        }

        if (normalizedName.Contains("half pillar") || normalizedName.Contains("pillar"))
        {
            return TempleMaterialGroup.PillarBase;
        }

        if (normalizedName.Contains("top"))
        {
            return TempleMaterialGroup.Top;
        }

        return TempleMaterialGroup.FallbackMidStone;
    }

    private Material GetOrCreateMaterial(TempleMaterialGroup group)
    {
        if (cachedMaterials.TryGetValue(group, out Material cachedMaterial) && cachedMaterial != null)
        {
            UpdateMaterialProperties(cachedMaterial, group);
            return cachedMaterial;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            shader = Shader.Find("Diffuse");
        }

        Material material = new Material(shader)
        {
            name = $"Temple_{group}",
            hideFlags = HideFlags.HideAndDontSave
        };

        UpdateMaterialProperties(material, group);
        cachedMaterials[group] = material;
        return material;
    }

    private void UpdateMaterialProperties(Material material, TempleMaterialGroup group)
    {
        Color color = GetColor(group);
        float smoothness = GetSmoothness(group);

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        if (material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0f);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", smoothness);
        }

        if (material.HasProperty("_Glossiness"))
        {
            material.SetFloat("_Glossiness", smoothness);
        }
    }

    private Color GetColor(TempleMaterialGroup group)
    {
        switch (group)
        {
            case TempleMaterialGroup.UpperGopura:
                return upperGopuraColor;
            case TempleMaterialGroup.BorderLedge:
                return borderLedgeColor;
            case TempleMaterialGroup.PillarBase:
                return pillarBaseColor;
            case TempleMaterialGroup.Top:
                return topColor;
            default:
                return fallbackMidStoneColor;
        }
    }

    private float GetSmoothness(TempleMaterialGroup group)
    {
        switch (group)
        {
            case TempleMaterialGroup.UpperGopura:
                return upperGopuraSmoothness;
            case TempleMaterialGroup.BorderLedge:
                return borderLedgeSmoothness;
            case TempleMaterialGroup.PillarBase:
                return pillarBaseSmoothness;
            case TempleMaterialGroup.Top:
                return topSmoothness;
            default:
                return fallbackSmoothness;
        }
    }

    private void LogSummary(int rendererCount, Dictionary<TempleMaterialGroup, int> counts)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"{nameof(TempleMaterialAutoAssigner)} assigned {rendererCount} renderers on {name}: ");
        builder.Append($"gopura={GetCount(counts, TempleMaterialGroup.UpperGopura)}, ");
        builder.Append($"borders={GetCount(counts, TempleMaterialGroup.BorderLedge)}, ");
        builder.Append($"pillars={GetCount(counts, TempleMaterialGroup.PillarBase)}, ");
        builder.Append($"top={GetCount(counts, TempleMaterialGroup.Top)}, ");
        builder.Append($"fallback={GetCount(counts, TempleMaterialGroup.FallbackMidStone)}");

        Debug.Log(builder.ToString(), this);
    }

    private static int GetCount(Dictionary<TempleMaterialGroup, int> counts, TempleMaterialGroup group)
    {
        return counts.TryGetValue(group, out int count) ? count : 0;
    }
}
