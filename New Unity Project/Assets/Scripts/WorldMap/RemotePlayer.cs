using UnityEngine;

/// <summary>
/// Represents another player's avatar on the world map and interpolates
/// movement between received network updates.
/// </summary>
public class RemotePlayer : MonoBehaviour
{
    [SerializeField] private FrameAnimator frameAnimator;
    [SerializeField] private float interpolationSpeed = 10f;

    private Vector3 targetPosition;
    public int PlayerId { get; private set; }

    private void Awake()
    {
        if (!frameAnimator)
        {
            frameAnimator = GetComponent<FrameAnimator>();
        }
        targetPosition = transform.position;
    }

    /// <summary>
    /// Initializes this remote player with its unique identifier.
    /// </summary>
    /// <param name="playerId">Identifier supplied by the server.</param>
    public void Initialize(int playerId)
    {
        PlayerId = playerId;
        targetPosition = transform.position;
    }

    /// <summary>
    /// Applies an incoming state packet from the server.
    /// </summary>
    public void ApplyState(PlayerStatePacket packet)
    {
        targetPosition = packet.position;
        frameAnimator?.SetState(packet.animationState);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition,
            Time.deltaTime * interpolationSpeed);
    }
}
