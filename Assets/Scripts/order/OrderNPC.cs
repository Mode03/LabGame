using UnityEngine;
using UnityEngine.AI;

public class OrderNPC : MonoBehaviour
{
    public Transform entryExitPoint; // pradžia ir pabaiga
    public Transform standPoint;     // kur laukia

    public float moveSpeed = 2f;
    private Animator animator;
    private bool hasOrder = false;
    private bool isAtStandPoint = false;

    private NavMeshAgent agent;

    private enum State
    {
        ToStandPoint,
        WaitingForOrder,
        ToExit,
        WaitingAtExit
    }
    private State currentState = State.ToStandPoint;

    public OrderReceiver orderReceiver;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        transform.position = entryExitPoint.position;

        GoTo(standPoint.position);
    }

    private void Update()
    {
        animator.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            switch (currentState)
            {
                case State.ToStandPoint:
                    currentState = State.WaitingForOrder;
                    isAtStandPoint = true;
                    break;

                case State.ToExit:
                    currentState = State.WaitingAtExit;
                    break;

                case State.WaitingAtExit:
                    // Laukia cia, nieko nedaro, kol negauna signalo grizti
                    break;
            }
        }
    }

    private void GoTo(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void GiveOrder()
    {
        if (currentState == State.WaitingForOrder)
        {
            hasOrder = true;
            isAtStandPoint = false;
            currentState = State.ToExit;
            GoTo(entryExitPoint.position);
        }
    }

    public void OrderCompleted()
    {
        if (currentState == State.WaitingAtExit)
        {
            hasOrder = false;
            currentState = State.ToStandPoint;
            GoTo(standPoint.position);
        }
    }

    public bool IsReadyForOrder()
    {
        return currentState == State.WaitingForOrder;
    }

    public void ResetNPC()
    {
        hasOrder = false;
        isAtStandPoint = false;
        currentState = State.ToStandPoint;
        GoTo(standPoint.position);
    }
}
