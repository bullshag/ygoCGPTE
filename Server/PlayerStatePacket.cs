/// <summary>
/// Network packet describing a player's position and animation state.
/// </summary>
public record PlayerStatePacket(int PlayerId, float X, float Y, float Z, string AnimationState);
