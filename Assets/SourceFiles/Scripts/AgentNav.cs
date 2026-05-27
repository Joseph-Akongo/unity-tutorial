using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentNav : MonoBehaviour
{
    public Transform[] points;   // waypoints, set in Inspector
    int index = 0;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log($"AgentNav points: {points.Length}, on navmesh: {agent.isOnNavMesh}");
        if (points.Length > 0) agent.SetDestination(points[0].position);
    }

    void Update()
    {
        if (points.Length == 0) return;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % points.Length;
            agent.SetDestination(points[index].position);
        }
    }
}