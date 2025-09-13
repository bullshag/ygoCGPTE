using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and updates <see cref="RemotePlayer"/> instances using incoming packets.
/// </summary>
public class RemotePlayerManager : MonoBehaviour
{
    [SerializeField] private RemotePlayer remotePlayerPrefab;

    private readonly Dictionary<int, RemotePlayer> players = new();

    /// <summary>
    /// Applies a batch of state packets from the server.
    /// </summary>
    public void ApplyPackets(IEnumerable<PlayerStatePacket> packets)
    {
        foreach (var packet in packets)
        {
            if (!players.TryGetValue(packet.playerId, out var player))
            {
                player = Instantiate(remotePlayerPrefab, packet.position, Quaternion.identity, transform);
                player.Initialize(packet.playerId);
                players.Add(packet.playerId, player);
            }
            player.ApplyState(packet);
        }
    }
}
