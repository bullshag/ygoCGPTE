using UnityEngine;

/// <summary>
/// Represents a lightweight snapshot of a player's world map state.
/// </summary>
[System.Serializable]
public struct PlayerStatePacket
{
    public int playerId;
    public Vector3 position;
    public FrameAnimator.AnimationState animationState;
}
