using UnityEngine;

public class VoiceCommandListener : MonoBehaviour
{
    public MonoBehaviour playerScript; // assign either Player_Movement or Player2_Movement
    private IPlayerVoiceControlled player;

    private void Awake()
    {
        player = playerScript as IPlayerVoiceControlled;
        if (player == null)
            Debug.LogError("Assigned script does not implement IPlayerVoiceControlled!");
    }

    private void OnEnable()
    {
        VoiceCommandSystem.OnCommandRecognized += HandleVoiceCommand;
    }

    private void OnDisable()
    {
        VoiceCommandSystem.OnCommandRecognized -= HandleVoiceCommand;
    }

    private void HandleVoiceCommand(string command)
    {
        Debug.Log($"Voice command received: {command}");

        if (!player.CanInput) return;
        player.ExecuteCommand(command.ToLower());
    }
}
