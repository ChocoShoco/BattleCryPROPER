using UnityEngine;
using TMPro;

public class MicrophoneChecker : MonoBehaviour
{
    public TextMeshProUGUI micStatusText; // optional UI display
    private AudioClip micClip;
    private string micName;
    private bool micWorking;

    void Start()
    {
        CheckMicrophone();
    }

    void Update()
    {
        if (!micWorking) return;

        // Check mic volume level (debug + visual)
        float level = GetMicLoudness();
        if (micStatusText)
            micStatusText.text = $" {micName} | Level: {level:F2}";
    }

    public void CheckMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError(" No microphones detected! Please plug one in.");
            if (micStatusText)
                micStatusText.text = " No mic detected!";
            micWorking = false;
            return;
        }

        micName = Microphone.devices[0];
        Debug.Log($" Microphone detected: {micName}");

        micClip = Microphone.Start(micName, true, 1, 44100);

        if (micClip == null)
        {
            Debug.LogError(" Failed to start microphone.");
            micWorking = false;
            if (micStatusText)
                micStatusText.text = " Mic not working!";
            return;
        }

        micWorking = true;
        if (micStatusText)
            micStatusText.text = $" Active Mic: {micName}";
    }

    private float GetMicLoudness()
    {
        if (micClip == null) return 0f;

        const int sampleWindow = 128;
        float[] data = new float[sampleWindow];
        int position = Microphone.GetPosition(micName) - sampleWindow + 1;
        if (position < 0) return 0f;

        micClip.GetData(data, position);
        float levelMax = 0;
        foreach (var s in data)
        {
            float level = Mathf.Abs(s);
            if (level > levelMax) levelMax = level;
        }

        return levelMax * 10f; // scaled for readability
    }

    private void OnDestroy()
    {
        if (micClip != null && micWorking)
            Microphone.End(micName);
    }
}
