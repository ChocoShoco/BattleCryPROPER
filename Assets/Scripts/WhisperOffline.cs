using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;

public class WhisperOffline : MonoBehaviour
{
    [Header("Whisper Settings")]
    [SerializeField] private string whisperExePath = "Assets/WhisperIntegration/whisper-cli.exe";
    [SerializeField] private string whisperModelPath = "Assets/WhisperIntegration/ggml-base.en.bin";
    [SerializeField] private bool logOutput = false;

    /// <summary>
    /// Synchronously transcribes an audio file (blocks the main thread — use for testing only).
    /// </summary>
    public string Transcribe(string wavFilePath)
    {
        string exeFullPath = Path.GetFullPath(whisperExePath);
        string modelFullPath = Path.GetFullPath(whisperModelPath);

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = exeFullPath,
            Arguments = $"--model \"{modelFullPath}\" --language en \"{wavFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                    Debug.LogError($"[WhisperOffline] Whisper error: {error}");
                if (logOutput)
                    Debug.Log($"[WhisperOffline] Output: {output}");

                return CleanWhisperOutput(output);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[WhisperOffline] Failed to run Whisper: {e.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Coroutine wrapper — runs transcription on a background thread.
    /// </summary>
    public IEnumerator TranscribeRoutine(string wavFilePath, System.Action<string> onComplete)
    {
        string result = string.Empty;

        // Run Whisper in background thread
        Task task = Task.Run(() =>
        {
            result = Transcribe(wavFilePath);
        });

        // Wait until task is done
        yield return new WaitUntil(() => task.IsCompleted);

        onComplete?.Invoke(result);
    }

    /// <summary>
    /// Clean up Whisper output (removes timestamps and formatting)
    /// </summary>
    private string CleanWhisperOutput(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        // Whisper CLI output often includes lines like "[00:00.000 --> 00:02.000] Hello there"
        string[] lines = raw.Split('\n');
        System.Text.StringBuilder cleaned = new System.Text.StringBuilder();

        foreach (string line in lines)
        {
            if (line.Contains("]"))
            {
                int idx = line.IndexOf("]") + 1;
                string text = line.Substring(idx).Trim();
                if (!string.IsNullOrWhiteSpace(text))
                    cleaned.AppendLine(text);
            }
            else if (!string.IsNullOrWhiteSpace(line))
            {
                cleaned.AppendLine(line.Trim());
            }
        }

        return cleaned.ToString().Trim();
    }
}
