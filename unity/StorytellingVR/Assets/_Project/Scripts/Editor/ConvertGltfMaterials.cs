using UnityEditor;
using UnityEngine;

public static class ConvertGltfMaterials
{
    private const string SourceShaderName = "Shader Graphs/glTF-pbrMetallicRoughness";
    private const string TargetShaderName = "Universal Render Pipeline/Simple Lit";
    private const string UrpLitShaderName = "Universal Render Pipeline/Lit";

    private static readonly string[] BaseTextureProperties =
    {
        "_BaseMap",
        "_BaseColorMap",
        "_BaseColorTexture",
        "_MainTex",
        "_BaseColorTex"
    };

    private static readonly string[] NormalTextureProperties =
    {
        "_BumpMap",
        "_NormalMap",
        "_NormalTexture",
        "_NormalTex"
    };

    private static readonly string[] BaseColorProperties =
    {
        "_BaseColor",
        "_BaseColorFactor",
        "_Color"
    };

    [MenuItem("Tools/Optimization/Convert GLTF Materials")]
    public static void ConvertMaterials()
    {
        Shader targetShader = Shader.Find(TargetShaderName);
        if (targetShader == null)
        {
            Debug.LogError($"[ConvertGltfMaterials] Could not find shader '{TargetShaderName}'.");
            return;
        }

        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        int convertedCount = 0;

        foreach (string guid in materialGuids)
        {
            string materialPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null || material.shader == null)
            {
                continue;
            }

            string shaderName = material.shader.name;
            if (shaderName == TargetShaderName || shaderName == UrpLitShaderName)
            {
                continue;
            }

            if (shaderName != SourceShaderName)
            {
                continue;
            }

            Texture baseTexture = GetFirstTexture(material, BaseTextureProperties, out string baseTextureProperty);
            Texture normalTexture = GetFirstTexture(material, NormalTextureProperties, out string normalTextureProperty);
            Color baseColor = GetFirstColor(material, BaseColorProperties, Color.white);
            Vector2 baseTextureScale = baseTexture != null ? material.GetTextureScale(baseTextureProperty) : Vector2.one;
            Vector2 baseTextureOffset = baseTexture != null ? material.GetTextureOffset(baseTextureProperty) : Vector2.zero;
            Vector2 normalTextureScale = normalTexture != null ? material.GetTextureScale(normalTextureProperty) : Vector2.one;
            Vector2 normalTextureOffset = normalTexture != null ? material.GetTextureOffset(normalTextureProperty) : Vector2.zero;

            material.shader = targetShader;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", baseColor);
            }

            if (baseTexture != null && material.HasProperty("_BaseMap"))
            {
                material.SetTexture("_BaseMap", baseTexture);
                material.SetTextureScale("_BaseMap", baseTextureScale);
                material.SetTextureOffset("_BaseMap", baseTextureOffset);
            }

            if (normalTexture != null && material.HasProperty("_BumpMap"))
            {
                material.SetTexture("_BumpMap", normalTexture);
                material.SetTextureScale("_BumpMap", normalTextureScale);
                material.SetTextureOffset("_BumpMap", normalTextureOffset);
                material.EnableKeyword("_NORMALMAP");
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", 0f);
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", 0.2f);
            }

            EditorUtility.SetDirty(material);
            Debug.Log($"[ConvertGltfMaterials] Converted material: {materialPath}");
            convertedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[ConvertGltfMaterials] Conversion complete. Converted {convertedCount} material(s).");
    }

    private static Texture GetFirstTexture(Material material, string[] propertyNames, out string propertyName)
    {
        foreach (string candidate in propertyNames)
        {
            if (!material.HasProperty(candidate))
            {
                continue;
            }

            Texture texture = material.GetTexture(candidate);
            if (texture != null)
            {
                propertyName = candidate;
                return texture;
            }
        }

        propertyName = null;
        return null;
    }

    private static Color GetFirstColor(Material material, string[] propertyNames, Color fallback)
    {
        foreach (string candidate in propertyNames)
        {
            if (material.HasProperty(candidate))
            {
                return material.GetColor(candidate);
            }
        }

        return fallback;
    }
}
