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
    public bool enableKeyboardDebugShortcuts = false;
    public bool enableKeyboardVoiceShortcut = false;
    public bool enableKeyboardResetShortcut = false;
    public bool enableVrTradePanelShortcut = true;

    [Header("Logging")]
    public bool enableVerboseLogs = false;
    public bool enableTradeLogs = true;
    public bool enableVoiceLogs = false;
    public bool enableParserLogs = true;

    public ChatManager chatManager;
    public DebugButtonOption debugButton = DebugButtonOption.SecondaryButtonX;
    public DebugButtonOption vrTradePanelButton = DebugButtonOption.SecondaryButtonY;

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
        if (enableVerboseLogs && Time.unscaledTime >= nextHeartbeatLogTime)
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

        LogVerbose("[TEMP DEBUG] Force accept pressed. X/debug button detected.");

        if (chatManager == null)
        {
            chatManager = FindFirstObjectByType<ChatManager>();
        }

        LogVerbose($"[TEMP DEBUG] ChatManager assigned after lookup: {chatManager != null}");

        if (chatManager == null)
        {
            Debug.LogWarning("[TEMP DEBUG] Force accept ignored because ChatManager was not found.");
            return;
        }

        LogVerbose("[TEMP DEBUG] TryForceDebugAcceptCurrentTrade called: true");
        bool accepted = chatManager.TryForceDebugAcceptCurrentTrade();
        if (accepted)
        {
            LogTrade("[TEMP DEBUG] Force accept triggered pending fulfillment.");
        }
        else
        {
            LogVerbose("[TEMP DEBUG] Force accept ignored because no active negotiable trade/customer was available.");
        }
    }

    private bool GetDebugButtonDown()
    {
        return GetButtonDown(debugButton);
    }

    private bool GetTradePanelButtonDown()
    {
        return GetButtonDown(vrTradePanelButton);
    }

    private bool GetButtonDown(DebugButtonOption buttonOption)
    {
        return buttonOption switch
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

    public static bool IsKeyboardVoiceShortcutEnabled()
    {
        return Instance != null &&
               Instance.enableKeyboardDebugShortcuts &&
               Instance.enableKeyboardVoiceShortcut;
    }

    public static bool IsKeyboardResetShortcutEnabled()
    {
        return Instance != null &&
               Instance.enableKeyboardDebugShortcuts &&
               Instance.enableKeyboardResetShortcut;
    }

    public static bool IsVrTradePanelShortcutPressed()
    {
        return Instance != null &&
               Instance.enableVrTradePanelShortcut &&
               Instance.GetTradePanelButtonDown();
    }

    public static bool VerboseLogsEnabled()
    {
        return Instance != null ? Instance.enableVerboseLogs : false;
    }

    public static bool TradeLogsEnabled()
    {
        return Instance != null ? Instance.enableTradeLogs : true;
    }

    public static bool VoiceLogsEnabled()
    {
        return Instance != null ? Instance.enableVoiceLogs : false;
    }

    public static bool ParserLogsEnabled()
    {
        return Instance != null ? Instance.enableParserLogs : true;
    }

    public static void LogVerbose(string message)
    {
        if (VerboseLogsEnabled())
        {
            Debug.Log(message);
        }
    }

    public static void LogTrade(string message)
    {
        if (TradeLogsEnabled())
        {
            Debug.Log(message);
        }
    }

    public static void LogVoice(string message)
    {
        if (VoiceLogsEnabled())
        {
            Debug.Log(message);
        }
    }

    public static void LogParser(string message)
    {
        if (ParserLogsEnabled())
        {
            Debug.Log(message);
        }
    }
}
