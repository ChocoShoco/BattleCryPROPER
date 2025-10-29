using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Player_Movement : MonoBehaviour, IPlayerVoiceControlled

{
    [SerializeField] private NavMeshAgent agent;
    public GameObject enemy;
    [SerializeField] private Animator animator;
    public bool can_input;
    public Transform opponent;
    public bool CanInput => can_input;

    public enum PlayerState
    {
        Idle,
        Attacking,
        Blocking,
        Kicking
    }

    public PlayerState state;

    private void OnEnable()
    {
        VoiceCommandSystem.OnCommandRecognized += OnVoiceCommand;
    }

    private void OnDisable()
    {
        VoiceCommandSystem.OnCommandRecognized -= OnVoiceCommand;
    }

    private void Start()
    {
        can_input = true;
        state = PlayerState.Idle;
    }

    private void Update()
    {
        agent.SetDestination(enemy.transform.position);

        // Keep manual key inputs for fallback testing
        if (Input.GetKeyDown(KeyCode.LeftShift)) ExecuteCommand("dodge");
        if (Input.GetKeyDown(KeyCode.Alpha1)) ExecuteCommand("attack");
        if (Input.GetKeyDown(KeyCode.Alpha2)) ExecuteCommand("block");
        if (Input.GetKeyDown(KeyCode.Alpha3)) ExecuteCommand("kick");
        if (Input.GetKeyDown(KeyCode.Alpha4)) ExecuteCommand("cancel");
    }

    private void OnVoiceCommand(string command)
    {
        // Optional: only react to voice commands for this player
        // Example: Player 1 listens for "Player One attack"
        if (command.Contains("player one") || command.Contains("p1"))
        {
            command = command.Replace("player one", "").Replace("p1", "").Trim();
        }

        ExecuteCommand(command);
    }

    public void ExecuteCommand(string command)
    {
        if (!can_input) return;
        command = command.ToLower();

        ResetAllTriggers();

        switch (command)
        {
            case "attack":
                state = PlayerState.Attacking;
                can_input = false;
                animator.SetTrigger("Attack");
                break;

            case "block":
                state = PlayerState.Blocking;
                can_input = false;
                animator.SetTrigger("Block");
                break;

            case "kick":
                state = PlayerState.Kicking;
                can_input = false;
                animator.SetTrigger("Kick");
                break;

            case "dodge":
                can_input = false;
                animator.SetTrigger("Dodge");
                break;

            case "cancel":
                can_input = true;
                animator.SetTrigger("Cancel");
                break;

            default:
                // For debugging unknown phrases
                Debug.Log($"Unrecognized command: {command}");
                break;
        }
    }

    public void input_enable()
    {
        can_input = true;
        Debug.Log("enabled input");
    }

    private void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
}
