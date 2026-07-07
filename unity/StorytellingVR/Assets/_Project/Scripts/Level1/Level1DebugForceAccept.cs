using UnityEngine;

// TEMP DEBUG: Inspector-toggle Quest debug accept button for marketplace fulfillment testing.
public class Level1DebugForceAccept : MonoBehaviour
{
    public static Level1DebugForceAccept Instance { get; private set; }

    public enum DebugButtonOption
    {
        SecondaryButtonY = 0,
        SecondaryButtonX = 1,
        Start = 2
    }

    [Header("TEMP DEBUG")]
    public bool debugModeEnabled = false;
    public bool bypassScoopFulfillmentForTesting = false;
    public ChatManager chatManager;
    public DebugButtonOption debugButton = DebugButtonOption.SecondaryButtonX;

    // TEMP DEBUG: Throttles heartbeat logs so Update visibility is readable in-device.
    private float nextHeartbeatLogTime;

    private void Awake()
    {
        Instance = this;

        // TEMP DEBUG: Auto-find ChatManager if the scene ref is missing.
        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // TEMP DEBUG: Heartbeat log every 2 seconds so we can confirm Update is executing.
        if (Time.unscaledTime >= nextHeartbeatLogTime)
        {
            bool heartbeatButtonDetected = GetDebugButtonDown();
            Debug.Log($"[TEMP DEBUG] ForceAccept heartbeat. debugModeEnabled={debugModeEnabled}, buttonDetected={heartbeatButtonDetected}, chatManagerAssigned={(chatManager != null)}");
            nextHeartbeatLogTime = Time.unscaledTime + 2f;
        }

        if (!debugModeEnabled)
        {
            return;
        }

        bool buttonDetected = GetDebugButtonDown();
        if (!buttonDetected)
        {
            return;
        }

        Debug.Log("[TEMP DEBUG] Force accept pressed. X/debug button detected.");

        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }

        Debug.Log($"[TEMP DEBUG] ChatManager assigned after lookup: {chatManager != null}");

        if (chatManager == null)
        {
            Debug.LogWarning("[TEMP DEBUG] Force accept ignored because ChatManager was not found.");
            return;
        }

        Debug.Log("[TEMP DEBUG] TryForceDebugAcceptCurrentTrade called: true");
        bool accepted = chatManager.TryForceDebugAcceptCurrentTrade();
        if (accepted)
        {
            Debug.Log("[TEMP DEBUG] Force accept triggered pending fulfillment.");
        }
        else
        {
            Debug.Log("[TEMP DEBUG] Force accept ignored because no active negotiable trade/customer was available.");
        }
    }

    private bool GetDebugButtonDown()
    {
        return debugButton switch
        {
            DebugButtonOption.SecondaryButtonY => OVRInput.GetDown(OVRInput.Button.Four),
            DebugButtonOption.SecondaryButtonX => OVRInput.GetDown(OVRInput.Button.Three),
            DebugButtonOption.Start => OVRInput.GetDown(OVRInput.Button.Start),
            _ => false
        };
    }

    public static bool ShouldBypassScoopFulfillment()
    {
        return Instance != null && Instance.bypassScoopFulfillmentForTesting;
    }
}
