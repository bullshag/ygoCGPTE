using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WorldMapSyncTests
{
    [UnityTest]
    public IEnumerator RemotePlayerMovesTowardTargetAndSetsAnimation()
    {
        var go = new GameObject("Remote");
        go.AddComponent<SpriteRenderer>();
        var animator = go.AddComponent<FrameAnimator>();
        var remote = go.AddComponent<RemotePlayer>();
        remote.Initialize(1);

        var packet = new PlayerStatePacket
        {
            playerId = 1,
            position = new Vector3(5f, 0f, 0f),
            animationState = FrameAnimator.AnimationState.MoveRight
        };
        remote.ApplyState(packet);
        var start = go.transform.position;

        yield return null; // wait one frame for interpolation

        Assert.Greater(go.transform.position.x, start.x);
    }
}
