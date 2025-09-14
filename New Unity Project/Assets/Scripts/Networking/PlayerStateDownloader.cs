using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Networking
{
    /// <summary>
    /// Periodically retrieves other players' states from the database.
    /// </summary>
    public class PlayerStateDownloader : MonoBehaviour
    {
        [SerializeField] private int playerId;
        [SerializeField] private float pollInterval = 1f;

        public class PlayerState
        {
            public Vector3 Position;
            public bool IsTraveling;
            public Vector3? NextWaypoint;
            public DateTime Timestamp;
        }

        public Dictionary<int, PlayerState> OtherPlayers { get; } = new();

        private void Start()
        {
            StartCoroutine(PollLoop());
        }

        private IEnumerator PollLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(pollInterval);
                _ = DownloadAsync();
            }
        }

        private async Task DownloadAsync()
        {
            const string sql = @"SELECT player_id, current_pos, is_traveling, next_waypoint, timestamp
                                 FROM player_position WHERE player_id <> @player_id";
            var parameters = new Dictionary<string, object?> { ["@player_id"] = playerId };
            var rows = await DatabaseClientUnity.QueryAsync(sql, parameters);
            foreach (var row in rows)
            {
                int id = Convert.ToInt32(row["player_id"]);
                OtherPlayers[id] = Parse(row);
            }
        }

        private static PlayerState Parse(Dictionary<string, object?> row)
        {
            var state = new PlayerState
            {
                Position = ParseVector(row["current_pos"] as string),
                IsTraveling = Convert.ToInt32(row["is_traveling"]) != 0,
                NextWaypoint = row["next_waypoint"] is string s ? ParseVector(s) : (Vector3?)null,
                Timestamp = row["timestamp"] is DateTime dt ? dt : DateTime.UtcNow
            };
            return state;
        }

        private static Vector3 ParseVector(string? value)
        {
            if (string.IsNullOrEmpty(value)) return Vector3.zero;
            var parts = value.Split(',');
            if (parts.Length != 3) return Vector3.zero;
            return new Vector3(
                float.Parse(parts[0]),
                float.Parse(parts[1]),
                float.Parse(parts[2])
            );
        }
    }
}
