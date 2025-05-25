using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWander : MonoBehaviour
{
    public Transform centerPoint; // Assign in Inspector
    public float wanderRadius = 10f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;

    private NavMeshAgent agent;
    private int failedAttempts = 0;
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(30, 70);
        StartCoroutine(WanderLoop());
    }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            Vector3 destination;

            // Check if we've failed to move too many times
            if (failedAttempts >= 2)
            {
                Debug.Log($"{gameObject.name} moving toward center to recover.");
                destination = centerPoint.position;
                failedAttempts = 0; // Reset counter
            }
            else
            {
                wanderRadius = Random.Range(2, 25);
                destination = GetRandomNavmeshLocation(wanderRadius);
            }

            agent.SetDestination(destination);

            // Wait until path is ready
            while (agent.pathPending)
                yield return null;

            // Wait until arrival
            while (agent.remainingDistance > agent.stoppingDistance)
                yield return null;

            yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));

            // Check how much we actually moved
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved < 2f)
                failedAttempts++;
            else
                failedAttempts = 0;

            lastPosition = transform.position;
        }
    }

    Vector3 GetRandomNavmeshLocation(float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return transform.position; // fallback
    }
}
