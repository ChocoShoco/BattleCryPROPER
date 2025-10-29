using UnityEngine;

public interface IPlayerVoiceControlled
{
    bool CanInput { get; }
    void ExecuteCommand(string command);
}
