using UnityEngine;
using UnityEngine.SceneManagement;

public class XRCameraDebugLogger : MonoBehaviour
{
    private void Awake()
    {
        LogData("Awake");
    }

    private void Start()
    {
        LogData("Start");
        Invoke("LogDelayed", 2f);
    }

    private void LogDelayed()
    {
        LogData("2 Seconds Delayed");
    }

    private void LogData(string phase)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string mainCamName = Camera.main != null ? Camera.main.name : "None";
        string mainCamWorld = Camera.main != null ? Camera.main.transform.position.ToString("F3") : "N/A";
        string mainCamLocal = Camera.main != null ? Camera.main.transform.localPosition.ToString("F3") : "N/A";

        string rigWorld = transform.position.ToString("F3");
        string rigLocal = transform.localPosition.ToString("F3");

        Transform trackingSpace = transform.Find("TrackingSpace");
        string tsWorld = trackingSpace != null ? trackingSpace.position.ToString("F3") : "None";
        string tsLocal = trackingSpace != null ? trackingSpace.localPosition.ToString("F3") : "None";

        Transform centerEye = trackingSpace != null ? trackingSpace.Find("CenterEyeAnchor") : null;
        string ceWorld = centerEye != null ? centerEye.position.ToString("F3") : "None";
        string ceLocal = centerEye != null ? centerEye.localPosition.ToString("F3") : "None";

        string trackingOrigin = "None";
        OVRManager ovrManager = GetComponent<OVRManager>();
        if (ovrManager != null)
        {
            trackingOrigin = ovrManager.trackingOriginType.ToString();
        }
        else
        {
            // OVRManager might be on a child or parent
            ovrManager = GetComponentInChildren<OVRManager>();
            if (ovrManager != null)
            {
                trackingOrigin = ovrManager.trackingOriginType.ToString() + " (In Children)";
            }
        }

        Debug.Log($"[XR_DEBUG] [{sceneName}] [{phase}]\n" +
                  $"  Camera.main: {mainCamName} | World: {mainCamWorld} | Local: {mainCamLocal}\n" +
                  $"  Rig: World: {rigWorld} | Local: {rigLocal}\n" +
                  $"  TrackingSpace: World: {tsWorld} | Local: {tsLocal}\n" +
                  $"  CenterEyeAnchor: World: {ceWorld} | Local: {ceLocal}\n" +
                  $"  OVRManager Tracking Origin: {trackingOrigin}");
    }
}
