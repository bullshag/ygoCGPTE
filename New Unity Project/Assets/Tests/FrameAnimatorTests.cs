using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tests for <see cref="FrameAnimator"/>.
/// </summary>
public class FrameAnimatorTests
{
    [Test]
    public void SetState_UsesFirstFrameImmediately()
    {
        var go = new GameObject();
        var renderer = go.AddComponent<SpriteRenderer>();
        var animator = go.AddComponent<FrameAnimator>();

        var sprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        animator.Idle.Add(sprite);

        animator.SetState(FrameAnimator.AnimationState.Idle);

        Assert.AreEqual(sprite, renderer.sprite);
    }

    [Test]
    public void Advance_CyclesFramesWithLoop()
    {
        var go = new GameObject();
        var renderer = go.AddComponent<SpriteRenderer>();
        var animator = go.AddComponent<FrameAnimator>();

        var spriteA = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        var spriteB = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        animator.Idle.AddRange(new List<Sprite> { spriteA, spriteB });
        animator.FrameRate = 1f;
        animator.Loop = true;
        animator.SetState(FrameAnimator.AnimationState.Idle);

        animator.Advance(1f);
        Assert.AreEqual(spriteB, renderer.sprite);

        animator.Advance(1f);
        Assert.AreEqual(spriteA, renderer.sprite);
    }
}
