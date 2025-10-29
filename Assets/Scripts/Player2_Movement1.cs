using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

public class Player2_Movement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    public GameObject enemy;
    [SerializeField] private Animator animator;
    public bool can_input;

    public enum PlayerState
    {
        Idle,
        Attacking,
        Blocking,
        Kicking
    }

    public PlayerState state;
    private void Start()
    {
        can_input = true;
        state = PlayerState.Idle;
        //agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (state)
        {
            case PlayerState.Idle:
                if (agent.velocity == Vector3.zero)
                {
                    //return;
                }
                else
                {
                    //return;
                }
                break;
            case PlayerState.Attacking:
                break;
            case PlayerState.Blocking:
                break;
            case PlayerState.Kicking:
                break;
        }
        agent.SetDestination(enemy.transform.position);

        if (Input.GetKeyDown(KeyCode.RightShift) && can_input)
        {
            ResetAllTriggers();
            can_input = false;
            animator.SetTrigger("Dodge");
        }

        if (Input.GetKeyDown(KeyCode.U) && can_input)
        {
            ResetAllTriggers();
            state = PlayerState.Attacking;
            can_input = false;
            animator.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.I) && can_input)
        {
            ResetAllTriggers();
            state = PlayerState.Blocking;
            can_input = false;
            animator.SetTrigger("Block");
        }

        if (Input.GetKeyDown(KeyCode.O) && can_input)
        {
            ResetAllTriggers();
            state = PlayerState.Kicking;
            can_input = false;
            animator.SetTrigger("Kick");
        }

        if (Input.GetKeyDown(KeyCode.P) && can_input == false)
        {
            can_input = true;
            //ResetAllTriggers();
            //can_input = true;
            animator.SetTrigger("Cancel");
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