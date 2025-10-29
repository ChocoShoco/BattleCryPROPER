using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Player2_Movement : MonoBehaviour, IPlayerVoiceControlled
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    public Transform opponent;
    public bool can_input = true;
    public bool CanInput => can_input;

    public enum PlayerState
    {
        Idle,
        Attacking,
        Blocking,
        Kicking
    }

    public PlayerState state = PlayerState.Idle;

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
        if (!agent)
            agent = GetComponent<NavMeshAgent>();

        can_input = true;
        state = PlayerState.Idle;
    }

    private void Update()
    {
        if (opponent)
            agent.SetDestination(opponent.position);

        HandleManualInput();
    }

    private void HandleManualInput()
    {
        if (!can_input) return;

        if (Input.GetKeyDown(KeyCode.RightShift)) ExecuteCommand("dodge");
        if (Input.GetKeyDown(KeyCode.U)) ExecuteCommand("attack");
        if (Input.GetKeyDown(KeyCode.I)) ExecuteCommand("block");
        if (Input.GetKeyDown(KeyCode.O)) ExecuteCommand("kick");
        if (Input.GetKeyDown(KeyCode.P)) ExecuteCommand("cancel");
    }

    private void OnVoiceCommand(string command)
    {
        // Filter so only Player 2 commands get through
        if (command.Contains("player two") || command.Contains("p2"))
        {
            command = command.Replace("player two", "").Replace("p2", "").Trim();
            ExecuteCommand(command);
        }
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
                animator.SetTrigger("Attack");
                can_input = false;
                break;

            case "block":
                state = PlayerState.Blocking;
                animator.SetTrigger("Block");
                can_input = false;
                break;

            case "kick":
                state = PlayerState.Kicking;
                animator.SetTrigger("Kick");
                can_input = false;
                break;

            case "dodge":
                animator.SetTrigger("Dodge");
                can_input = false;
                break;

            case "cancel":
                animator.SetTrigger("Cancel");
                can_input = true;
                break;

            default:
                Debug.Log($"[Player 2] Unrecognized command: {command}");
                break;
        }
    }

    public void input_enable()
    {
        can_input = true;
        Debug.Log($"{name} input re-enabled");
    }

    private void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(param.name);
        }
    }
}
