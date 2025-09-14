using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles point-and-click navigation with queued waypoints and directional animations.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerNavigator : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private FrameAnimator frameAnimator;
    [SerializeField] private GameObject cityInteractionPanel;
    [SerializeField] private GameObject waypointPrefab;
    [SerializeField] private LineRenderer pathLinePrefab;

    private readonly Queue<Vector3> waypoints = new Queue<Vector3>();
    private readonly List<GameObject> waypointMarkers = new List<GameObject>();
    private LineRenderer pathLine;
    private FrameAnimator.AnimationState currentAnimState = FrameAnimator.AnimationState.Idle;

    /// <summary>
    /// Raised when all queued waypoints are exhausted.
    /// </summary>
    public event Action QueueEmptied;

    private void Awake()
    {
        if (!agent)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        HandleInput();
        AdvanceQueue();
        UpdatePathLine();
        UpdateAnimation();
    }

    private void HandleInput()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (TryGetMousePoint(out var point))
            {
                SetDestination(point);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (TryGetMousePoint(out var point))
            {
                AddWaypoint(point);
            }
        }
    }

    private bool TryGetMousePoint(out Vector3 point)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            point = hit.point;
            return true;
        }
        point = Vector3.zero;
        return false;
    }

    private void SetDestination(Vector3 destination)
    {
        waypoints.Clear();
        ClearMarkers();
        agent.SetDestination(destination);
        SpawnMarker(destination);
        ClearPathLine();
    }

    private void AddWaypoint(Vector3 waypoint)
    {
        waypoints.Enqueue(waypoint);
        SpawnMarker(waypoint);
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypoints.Dequeue());
        }
    }

    private void AdvanceQueue()
    {
        if (agent.pathPending)
        {
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            DestroyFirstMarker();
            if (waypoints.Count > 0)
            {
                agent.SetDestination(waypoints.Dequeue());
            }
            else
            {
                QueueEmptied?.Invoke();
                ClearPathLine();
            }
        }
    }

    private void ClearMarkers()
    {
        foreach (var marker in waypointMarkers)
        {
            if (marker)
            {
                Destroy(marker);
            }
        }
        waypointMarkers.Clear();
    }

    private void SpawnMarker(Vector3 position)
    {
        if (!waypointPrefab)
        {
            return;
        }
        var marker = Instantiate(waypointPrefab, position, Quaternion.identity);
        waypointMarkers.Add(marker);
    }

    private void DestroyFirstMarker()
    {
        if (waypointMarkers.Count == 0)
        {
            return;
        }
        var marker = waypointMarkers[0];
        waypointMarkers.RemoveAt(0);
        if (marker)
        {
            Destroy(marker);
        }
    }

    private void UpdatePathLine()
    {
        if (!pathLinePrefab)
        {
            return;
        }

        if (!pathLine)
        {
            pathLine = Instantiate(pathLinePrefab);
        }

        var positions = new List<Vector3> { transform.position };
        if (agent.hasPath)
        {
            positions.Add(agent.destination);
        }
        positions.AddRange(waypoints);
        pathLine.positionCount = positions.Count;
        pathLine.SetPositions(positions.ToArray());
    }

    private void ClearPathLine()
    {
        if (pathLine)
        {
            Destroy(pathLine.gameObject);
            pathLine = null;
        }
    }

    private void UpdateAnimation()
    {
        var velocity = agent.velocity;
        if (velocity.sqrMagnitude <= 0.0001f)
        {
            if (currentAnimState != FrameAnimator.AnimationState.Idle)
            {
                frameAnimator?.SetState(FrameAnimator.AnimationState.Idle);
                currentAnimState = FrameAnimator.AnimationState.Idle;
            }
            return;
        }

        FrameAnimator.AnimationState state;
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.z))
        {
            state = velocity.x > 0f ? FrameAnimator.AnimationState.MoveRight : FrameAnimator.AnimationState.MoveLeft;
        }
        else
        {
            state = velocity.z > 0f ? FrameAnimator.AnimationState.MoveUp : FrameAnimator.AnimationState.MoveDown;
        }

        if (state != currentAnimState)
        {
            frameAnimator?.SetState(state);
            currentAnimState = state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CityNode"))
        {
            cityInteractionPanel?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CityNode"))
        {
            cityInteractionPanel?.SetActive(false);
        }
    }
}

