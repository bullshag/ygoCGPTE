using System.Collections.Generic;
using UnityEngine;
using Networking;

namespace Player
{
    /// <summary>
    /// Spawns and updates <see cref="RemotePlayerMarker"/> instances for other online players.
    /// </summary>
    public class RemotePlayerManager : MonoBehaviour
    {
        [SerializeField] private RemotePlayerMarker markerPrefab;
        [SerializeField] private PlayerStateDownloader downloader;
        [SerializeField] private float syncInterval = 1f;

        private readonly Dictionary<int, RemotePlayerMarker> markers = new();
        private float nextSyncTime;

        private void Awake()
        {
            if (!downloader)
            {
                downloader = FindObjectOfType<PlayerStateDownloader>();
            }
        }

        private void Update()
        {
            if (Time.time >= nextSyncTime)
            {
                nextSyncTime = Time.time + syncInterval;
                SyncMarkers();
            }

            foreach (var marker in markers.Values)
            {
                var agent = marker.Agent;
                marker.transform.position = agent.transform.position;
            }
        }

        private void SyncMarkers()
        {
            var onlineIds = new HashSet<int>(downloader.OtherPlayers.Keys);

            foreach (var kvp in downloader.OtherPlayers)
            {
                int id = kvp.Key;
                var state = kvp.Value;

                if (!markers.TryGetValue(id, out var marker))
                {
                    marker = Instantiate(markerPrefab, state.Position, Quaternion.identity, transform);
                    marker.Initialize(id);
                    markers.Add(id, marker);
                }

                marker.IsOnline = true;

                if (state.IsTraveling && state.NextWaypoint.HasValue)
                {
                    marker.Agent.SetDestination(state.NextWaypoint.Value);
                }
            }

            foreach (var pair in markers)
            {
                if (!onlineIds.Contains(pair.Key))
                {
                    pair.Value.IsOnline = false;
                }
            }
        }
    }
}
