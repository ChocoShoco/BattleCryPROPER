using UnityEngine;
using System.Collections;

public class VoiceCommandSystem : MonoBehaviour
{
    public static event System.Action<string> OnCommandRecognized;

    [Header("Components")]
    [SerializeField] private VoiceRecorder recorder;
    [SerializeField] private WhisperOffline whisper;

    [Header("Settings")]
    [Tooltip("Length of each recording cycle in seconds.")]
    [SerializeField] private float listenInterval = 1f;

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
        Debug.Log($"[{name}] Continuous listening started.");
    }

    public void StopListening()
    {
        isListening = false;
        Microphone.End(null);
        Debug.Log($"[{name}] Listening stopped.");
    }

    private IEnumerator ContinuousListeningLoop()
    {
        while (isListening)
        {
            recorder.StartRecording();
            yield return new WaitForSeconds(listenInterval);

            recorder.StopRecording();
            string path = recorder.GetLastSavedFilePath();

            Debug.Log($"[{name}] Processing segment: {path}");
            yield return StartCoroutine(whisper.TranscribeRoutine(path, OnWhisperComplete));

            // Minimal pause to keep the loop stable
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnWhisperComplete(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            Debug.LogWarning($"[{name}] Whisper returned no text.");
            return;
        }

        string cleaned = result.Trim().ToLower();
        Debug.Log($"[{name}] Whisper recognized: \"{cleaned}\"");
        OnCommandRecognized?.Invoke(cleaned);
    }
}
