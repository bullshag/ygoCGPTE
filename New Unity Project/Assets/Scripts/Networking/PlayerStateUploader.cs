using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Networking
{
    /// <summary>
    /// Periodically pushes the local player's navigation state to the database.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerStateUploader : MonoBehaviour
    {
        [SerializeField] private int playerId;
        [SerializeField] private float uploadInterval = 1f;

        private NavMeshAgent navAgent;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            StartCoroutine(UploadLoop());
        }

        private IEnumerator UploadLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(uploadInterval);
                _ = UploadAsync();
            }
        }

        private async Task UploadAsync()
        {
            if (navAgent == null) return;

            string currentPos = FormatVector(transform.position);
            string? nextWaypoint = navAgent.hasPath ? FormatVector(navAgent.destination) : null;
            bool isTraveling = navAgent.hasPath;

            const string sql = @"INSERT INTO player_position (player_id, current_pos, is_traveling, next_waypoint, timestamp)
                                 VALUES (@player_id, @current_pos, @is_traveling, @next_waypoint, CURRENT_TIMESTAMP)
                                 ON DUPLICATE KEY UPDATE
                                   current_pos = VALUES(current_pos),
                                   is_traveling = VALUES(is_traveling),
                                   next_waypoint = VALUES(next_waypoint),
                                   timestamp = VALUES(timestamp);";

            var parameters = new Dictionary<string, object?>
            {
                ["@player_id"] = playerId,
                ["@current_pos"] = currentPos,
                ["@is_traveling"] = isTraveling ? 1 : 0,
                ["@next_waypoint"] = nextWaypoint
            };

            await DatabaseClientUnity.ExecuteAsync(sql, parameters);
        }

        private static string FormatVector(Vector3 v)
        {
            return $"{v.x:F2},{v.y:F2},{v.z:F2}";
        }
    }
}
