using UnityEngine;
using Oculus.Interaction.Locomotion;

public class TeleportLockController : MonoBehaviour
{
    public static TeleportLockController Instance { get; private set; }

    [Header("Hotspot Groups")]
    [Tooltip("Hotspots used for the teleport tutorial.")]
    public TeleportInteractable[] tutorialHotspots;

    [Tooltip("All general free-roam teleport hotspots.")]
    public TeleportInteractable[] generalHotspots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Sets teleportation availability for tutorial hotspots.
    /// </summary>
    public void SetTutorialHotspotsEnabled(bool enabled)
    {
        SetHotspotsEnabled(tutorialHotspots, enabled);
    }

    /// <summary>
    /// Sets teleportation availability for general free-roam hotspots.
    /// </summary>
    public void SetGeneralHotspotsEnabled(bool enabled)
    {
        SetHotspotsEnabled(generalHotspots, enabled);
    }

    /// <summary>
    /// Sets teleportation availability for all hotspots.
    /// </summary>
    public void SetAllTeleportEnabled(bool enabled)
    {
        SetTutorialHotspotsEnabled(enabled);
        SetGeneralHotspotsEnabled(enabled);
    }

    private void SetHotspotsEnabled(TeleportInteractable[] hotspots, bool enabled)
    {
        if (hotspots == null) return;

        foreach (var hotspot in hotspots)
        {
            if (hotspot == null) continue;

            // 1. Set the Meta SDK teleport interactable allowTeleport state
            hotspot.AllowTeleport = enabled;

            // 2. Enable/disable the component itself to stop interaction update loops
            hotspot.enabled = enabled;

            // 3. Enable/disable collider components on the hotspot object
            var colliders = hotspot.GetComponents<Collider>();
            foreach (var col in colliders)
            {
                if (col != null)
                {
                    col.enabled = enabled;
                }
            }

            // 4. Enable/disable all mesh renderers (on this object and children) to hide visuals
            var renderers = hotspot.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var ren in renderers)
            {
                if (ren != null)
                {
                    ren.enabled = enabled;
                }
            }
        }
    }
}
