using System.Collections.Generic;
using UnityEngine;

public class PlayerStallSpiceGuide : MonoBehaviour
{
    [Header("Scope")]
    public Transform playerStallRoot;

    [Header("Collider Alignment")]
    public bool alignCollidersOnStart = true;
    public Vector3 colliderPadding = new Vector3(0.08f, 0.08f, 0.08f);
    public float minimumColliderSize = 0.12f;

    [Header("Selected Spice Glow")]
    public bool glowRequestedSpice = true;
    public Color glowColor = new Color(1f, 0.82f, 0.24f, 1f);
    public float glowIntensity = 1.8f;
    public float glowPulseSpeed = 2.5f;
    public float glowLightRange = 1.2f;
    public float glowLightIntensity = 1.4f;

    private readonly List<Renderer> glowingRenderers = new List<Renderer>();
    private readonly List<Material> glowingMaterials = new List<Material>();
    private Light glowLight;
    private SpiceType currentGlowingSpice = SpiceType.None;

    void Start()
    {
        if (alignCollidersOnStart)
            AlignAllSpiceColliders();

        UpdateRequestedSpiceGlow();
    }

    void Update()
    {
        if (!glowRequestedSpice)
            return;

        SpiceType requestedSpice = GetRequestedSpice();
        if (requestedSpice != currentGlowingSpice)
            UpdateRequestedSpiceGlow();

        PulseGlow();
    }

    public void AlignAllSpiceColliders()
    {
        foreach (SpiceZone zone in GetSpiceZones())
        {
            AlignCollider(zone);
        }
    }

    public void UpdateRequestedSpiceGlow()
    {
        ClearGlow();

        if (!glowRequestedSpice)
            return;

        currentGlowingSpice = GetRequestedSpice();
        if (currentGlowingSpice == SpiceType.None)
            return;

        Bounds combinedBounds = new Bounds();
        bool hasBounds = false;

        foreach (SpiceZone zone in GetSpiceZones())
        {
            if (zone == null || zone.spiceType != currentGlowingSpice)
                continue;

            foreach (Renderer renderer in zone.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null)
                    continue;

                glowingRenderers.Add(renderer);
                hasBounds = ExpandBounds(renderer.bounds, ref combinedBounds, hasBounds);
            }
        }

        ApplyGlowMaterials();

        if (hasBounds)
            CreateGlowLight(combinedBounds);
    }

    void AlignCollider(SpiceZone zone)
    {
        if (zone == null)
            return;

        BoxCollider boxCollider = zone.GetComponent<BoxCollider>();
        if (boxCollider == null)
            boxCollider = zone.gameObject.AddComponent<BoxCollider>();

        Bounds worldBounds = new Bounds();
        bool hasBounds = false;

        foreach (Renderer renderer in zone.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null)
                continue;

            hasBounds = ExpandBounds(renderer.bounds, ref worldBounds, hasBounds);
        }

        if (!hasBounds)
            return;

        ApplyWorldBoundsToLocalCollider(boxCollider, worldBounds);
        boxCollider.isTrigger = true;
    }

    void ApplyWorldBoundsToLocalCollider(BoxCollider boxCollider, Bounds worldBounds)
    {
        Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        Vector3 center = worldBounds.center;
        Vector3 extents = worldBounds.extents;

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    Vector3 corner = center + Vector3.Scale(extents, new Vector3(x, y, z));
                    Vector3 localCorner = boxCollider.transform.InverseTransformPoint(corner);
                    min = Vector3.Min(min, localCorner);
                    max = Vector3.Max(max, localCorner);
                }
            }
        }

        Vector3 size = max - min + colliderPadding;
        size.x = Mathf.Max(size.x, minimumColliderSize);
        size.y = Mathf.Max(size.y, minimumColliderSize);
        size.z = Mathf.Max(size.z, minimumColliderSize);

        boxCollider.center = (min + max) * 0.5f;
        boxCollider.size = size;
    }

    void ApplyGlowMaterials()
    {
        foreach (Renderer renderer in glowingRenderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                if (material == null)
                    continue;

                material.EnableKeyword("_EMISSION");
                glowingMaterials.Add(material);
            }
        }
    }

    void PulseGlow()
    {
        if (glowingMaterials.Count == 0 && glowLight == null)
            return;

        float pulse = 0.65f + Mathf.Sin(Time.time * glowPulseSpeed) * 0.35f;
        Color emissionColor = glowColor * (glowIntensity * pulse);

        foreach (Material material in glowingMaterials)
        {
            if (material != null)
                material.SetColor("_EmissionColor", emissionColor);
        }

        if (glowLight != null)
            glowLight.intensity = glowLightIntensity * pulse;
    }

    void CreateGlowLight(Bounds bounds)
    {
        GameObject lightObject = new GameObject("RequestedSpiceGlowLight");
        lightObject.transform.SetParent(transform);
        lightObject.transform.position = bounds.center + Vector3.up * Mathf.Max(bounds.extents.y, 0.15f);

        glowLight = lightObject.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = glowColor;
        glowLight.range = Mathf.Max(glowLightRange, bounds.size.magnitude);
        glowLight.intensity = glowLightIntensity;
    }

    void ClearGlow()
    {
        foreach (Material material in glowingMaterials)
        {
            if (material != null)
                material.SetColor("_EmissionColor", Color.black);
        }

        glowingRenderers.Clear();
        glowingMaterials.Clear();

        if (glowLight != null)
        {
            Destroy(glowLight.gameObject);
            glowLight = null;
        }

        currentGlowingSpice = SpiceType.None;
    }

    IEnumerable<SpiceZone> GetSpiceZones()
    {
        Transform root = playerStallRoot != null ? playerStallRoot : transform;
        return root.GetComponentsInChildren<SpiceZone>(true);
    }

    SpiceType GetRequestedSpice()
    {
        if (OrderManager.Instance != null)
            return OrderManager.Instance.requestedSpice;

        return SpiceType.None;
    }

    bool ExpandBounds(Bounds source, ref Bounds target, bool hasBounds)
    {
        if (!hasBounds)
        {
            target = source;
            return true;
        }

        target.Encapsulate(source);
        return true;
    }
}
