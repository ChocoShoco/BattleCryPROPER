using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    public static void Save(string filePath, AudioClip clip)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            byte[] wav = ConvertAudioClipToWav(clip);
            fileStream.Write(wav, 0, wav.Length);
        }
    }

    private static byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] bytes = new byte[samples.Length * 2];
        int offset = 0;

        foreach (float sample in samples)
        {
            short intSample = (short)(Mathf.Clamp(sample, -1f, 1f) * short.MaxValue);
            bytes[offset++] = (byte)(intSample & 0xff);
            bytes[offset++] = (byte)((intSample >> 8) & 0xff);
        }

        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            int sampleRate = clip.frequency;
            int channels = clip.channels;
            int byteRate = sampleRate * channels * 2;
            int subChunk2Size = bytes.Length;
            int chunkSize = 36 + subChunk2Size;

            // RIFF header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(chunkSize);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));

            // fmt subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1); // PCM
            writer.Write((short)channels);
            writer.Write(sampleRate);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);

            // data subchunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(subChunk2Size);
            writer.Write(bytes);

            return stream.ToArray();
        }
    }
}
