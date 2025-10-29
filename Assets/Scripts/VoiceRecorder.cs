using System;
using System.IO;
using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip recording;
    private string filePath;

    public int sampleRate = 16000;
    public int recordTime = 5; // seconds

    public void StartRecording()
    {
        recording = Microphone.Start(null, false, recordTime, sampleRate);
        filePath = Path.Combine(Application.persistentDataPath, "recorded.wav");
        Debug.Log("Recording started...");
    }

    public void StopRecording()
    {
        Microphone.End(null);
        SaveWav(recording, filePath);
        Debug.Log("Recording saved to: " + filePath);
    }

    private void SaveWav(AudioClip clip, string filePath)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        using (var fileStream = CreateEmpty(filePath))
        {
            ConvertAndWrite(fileStream, samples);
            WriteHeader(fileStream, clip);
        }
    }

    private FileStream CreateEmpty(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Create);
        byte emptyByte = new byte();
        for (int i = 0; i < 44; i++) fileStream.WriteByte(emptyByte);
        return fileStream;
    }

    private void ConvertAndWrite(FileStream fileStream, float[] samples)
    {
        Int16[] intData = new Int16[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        int rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = System.BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    private void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes(16), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes((ushort)1), 0, 2);
        fileStream.Write(System.BitConverter.GetBytes((ushort)channels), 0, 2);
        fileStream.Write(System.BitConverter.GetBytes(hz), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes(hz * channels * 2), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
        fileStream.Write(System.BitConverter.GetBytes((ushort)16), 0, 2);
        fileStream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        fileStream.Write(System.BitConverter.GetBytes(samples * channels * 2), 0, 4);
    }

    public string GetFilePath() => filePath;

    public string GetLastSavedFilePath()
    {
        return filePath;
    }

}
