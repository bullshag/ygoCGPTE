using UnityEngine;
using UnityEngine.AI;

namespace Player
{
    /// <summary>
    /// Simple component used to identify and control remote player markers.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class RemotePlayerMarker : MonoBehaviour
    {
        private NavMeshAgent agent;

        /// <summary>
        /// Identifier provided by the server for this remote player.
        /// </summary>
        public int PlayerId { get; private set; }

        /// <summary>
        /// True when the player has a current row in the download results.
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// Exposes the NavMeshAgent for movement control.
        /// </summary>
        public NavMeshAgent Agent => agent;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Initializes the marker with its player identifier.
        /// </summary>
        public void Initialize(int playerId)
        {
            PlayerId = playerId;
        }
    }
}
