using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    /// <summary>
    /// Handles queued waypoint navigation using shift-click inputs.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class WaypointNavAgent : MonoBehaviour
    {
        private readonly Queue<Vector3> waypoints = new Queue<Vector3>();
        private NavMeshAgent agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            HandleInput();
            ProcessMovement();
        }

        private void HandleInput()
        {
            bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            if (!shiftHeld) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1f, NavMesh.AllAreas))
                    {
                        waypoints.Enqueue(navHit.position);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                waypoints.Clear();
                agent.ResetPath();
            }
        }

        private void ProcessMovement()
        {
            if (agent.pathPending) return;

            if (!agent.hasPath && waypoints.Count > 0)
            {
                agent.SetDestination(waypoints.Peek());
            }
            else if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (waypoints.Count > 0)
                {
                    waypoints.Dequeue();
                    if (waypoints.Count > 0)
                    {
                        agent.SetDestination(waypoints.Peek());
                    }
                }
            }
        }
    }
}
