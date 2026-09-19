using Oculus.Interaction.Locomotion;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    [Header("Named Hotspot Groups")]
    [SerializeField]
    private TeleportHotspotGroup[] hotspotGroups;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "[TELEPORT MANAGER] Duplicate manager found. " +
                "Destroying duplicate component."
            );

            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void EnableGroup(string groupName)
    {
        SetGroupEnabled(groupName, true);
    }

    public void DisableGroup(string groupName)
    {
        SetGroupEnabled(groupName, false);
    }

    public void DisableAll()
    {
        if (hotspotGroups == null)
            return;

        foreach (TeleportHotspotGroup group in hotspotGroups)
        {
            if (group == null)
                continue;

            SetHotspotsEnabled(group.hotspots, false);
        }
    }

    public void EnableAll()
    {
        if (hotspotGroups == null)
            return;

        foreach (TeleportHotspotGroup group in hotspotGroups)
        {
            if (group == null)
                continue;

            SetHotspotsEnabled(group.hotspots, true);
        }
    }

    public void SetGroupEnabled(
        string groupName,
        bool enabled)
    {
        TeleportHotspotGroup group =
            FindGroup(groupName);

        if (group == null)
        {
            Debug.LogWarning(
                $"[TELEPORT MANAGER] Group '{groupName}' was not found."
            );

            return;
        }

        SetHotspotsEnabled(
            group.hotspots,
            enabled
        );

        Debug.Log(
            $"[TELEPORT MANAGER] Group '{groupName}' enabled: {enabled}"
        );
    }

    private TeleportHotspotGroup FindGroup(
        string groupName)
    {
        if (hotspotGroups == null ||
            string.IsNullOrWhiteSpace(groupName))
        {
            return null;
        }

        foreach (TeleportHotspotGroup group in hotspotGroups)
        {
            if (group == null)
                continue;

            if (string.Equals(
                group.groupName,
                groupName,
                System.StringComparison.OrdinalIgnoreCase))
            {
                return group;
            }
        }

        return null;
    }

    private void SetHotspotsEnabled(
        TeleportInteractable[] hotspots,
        bool enabled)
    {
        if (hotspots == null)
            return;

        foreach (TeleportInteractable hotspot in hotspots)
        {
            if (hotspot == null)
                continue;

            hotspot.AllowTeleport = enabled;
            hotspot.enabled = enabled;

            Collider[] colliders =
                hotspot.GetComponents<Collider>();

            foreach (Collider collider in colliders)
            {
                if (collider != null)
                    collider.enabled = enabled;
            }

            Renderer[] renderers =
                hotspot.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                    renderer.enabled = enabled;
            }
        }
    }
}