using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RandomMazePatrol : MonoBehaviour
{
    public float roamRadius = 20f;
    public float waitTime = 0.5f;

    private NavMeshAgent agent;
    private bool waiting;
    private Vector3 patrolCenter;

    [Header("Animation")]
    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomPoint();
        patrolCenter = transform.position;
    }

    void Update()
    {
        // ✅ Animation control
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        if (waiting)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAndMove());
        }
    }

    IEnumerator WaitAndMove()
    {
        waiting = true;

        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        agent.isStopped = false;

        SetRandomPoint();

        waiting = false;
    }

    void SetRandomPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection =
                patrolCenter +
                Random.insideUnitSphere * roamRadius;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomDirection,
                out hit,
                roamRadius,
                NavMesh.AllAreas))
            {
                if (Vector3.Distance(transform.position, hit.position) > 5f)
                {
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }
    void FixedUpdate()
{
    if (agent != null)
    {
        transform.position = agent.nextPosition;
    }
}
}