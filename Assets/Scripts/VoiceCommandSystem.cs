using System;
using UnityEngine;

public class VoiceCommandSystem : MonoBehaviour
{
    public static event Action<string> OnCommandRecognized;

    public static void Recognized(string command)
    {
        Debug.Log($"Voice recognized: {command}");
        OnCommandRecognized?.Invoke(command.ToLower().Trim());
    }
}
