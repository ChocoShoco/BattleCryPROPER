using UnityEngine;
using System.Collections;

public class VoiceCommandSystem : MonoBehaviour
{
    public static event System.Action<string> OnCommandRecognized;

    [SerializeField] private VoiceRecorder recorder;
    [SerializeField] private WhisperOffline whisper;
    [SerializeField] private float listenInterval = 3f; // every few seconds, process audio

    private bool isListening = false;

    private void Start()
    {
        StartListening();
    }

    public void StartListening()
    {
        if (isListening) return;
        isListening = true;
        StartCoroutine(ContinuousListeningLoop());
        Debug.Log("[VoiceCommandSystem] Continuous listening started.");
    }

    private IEnumerator ContinuousListeningLoop()
    {
        while (isListening)
        {
            recorder.StartRecording();
            yield return new WaitForSeconds(listenInterval);

            recorder.StopRecording();
            string path = recorder.GetLastSavedFilePath();
            Debug.Log($"[VoiceCommandSystem] Processing segment: {path}");

            yield return StartCoroutine(whisper.TranscribeRoutine(path, OnWhisperComplete));

            // small delay before next capture
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnWhisperComplete(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            Debug.LogWarning("Whisper returned no text.");
            return;
        }

        string cleaned = result.Trim().ToLower();
        Debug.Log($"Whisper recognized: \"{cleaned}\"");

        OnCommandRecognized?.Invoke(cleaned);
    }

    public void StopListening()
    {
        isListening = false;
        Microphone.End(null);
        Debug.Log("[VoiceCommandSystem] Listening stopped.");
    }
}
