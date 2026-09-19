using UnityEngine;

/// <summary>
/// Compatibility wrapper for older Free Roam scripts.
///
/// Existing scripts can continue using:
/// TeleportLockController.Instance.SetAllTeleportEnabled(...)
///
/// Internally, the calls are forwarded to the new TeleportManager.
/// This component can be removed later after all old references
/// have been migrated.
/// </summary>
public class TeleportLockController : MonoBehaviour
{
    public static TeleportLockController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "[TELEPORT COMPATIBILITY] Duplicate " +
                "TeleportLockController found. Destroying duplicate."
            );

            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Enables or disables the tutorial teleport group.
    /// </summary>
    public void SetTutorialHotspotsEnabled(bool enabled)
    {
        if (!TryGetTeleportManager(out TeleportManager manager))
            return;

        if (enabled)
            manager.EnableGroup("Tutorial");
        else
            manager.DisableGroup("Tutorial");
    }

    /// <summary>
    /// Enables or disables the general Free Roam teleport group.
    /// </summary>
    public void SetGeneralHotspotsEnabled(bool enabled)
    {
        if (!TryGetTeleportManager(out TeleportManager manager))
            return;

        if (enabled)
            manager.EnableGroup("General");
        else
            manager.DisableGroup("General");
    }

    /// <summary>
    /// Enables or disables every configured teleport group.
    /// </summary>
    public void SetAllTeleportEnabled(bool enabled)
    {
        if (!TryGetTeleportManager(out TeleportManager manager))
            return;

        if (enabled)
            manager.EnableAll();
        else
            manager.DisableAll();
    }

    private bool TryGetTeleportManager(
        out TeleportManager manager)
    {
        manager = TeleportManager.Instance;

        if (manager != null)
            return true;

        manager = FindFirstObjectByType<TeleportManager>();

        if (manager != null)
            return true;

        Debug.LogError(
            "[TELEPORT COMPATIBILITY] TeleportManager is missing " +
            "from the scene."
        );

        return false;
    }
}