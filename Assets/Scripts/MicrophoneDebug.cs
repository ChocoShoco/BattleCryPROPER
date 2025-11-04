using UnityEngine;

public class MicrophoneDebug : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== Available Microphone Devices ===");

        var devices = Microphone.devices;
        if (devices == null || devices.Length == 0)
        {
            Debug.LogWarning("No microphone devices detected!");
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"[{i}] {devices[i]}");
        }

        Debug.Log("====================================");
    }
}
