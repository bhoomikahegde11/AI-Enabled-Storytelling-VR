using UnityEngine;

public class MicDebug : MonoBehaviour
{
    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Mic found: " + device);
        }
    }
}