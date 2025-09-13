using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Plays directional sprite animations based on a simple frame list per state.
/// </summary>
public class FrameAnimator : MonoBehaviour
{
    /// <summary>
    /// Identifies available animation states.
    /// </summary>
    public enum AnimationState
    {
        Idle,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Fight
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> idle = new List<Sprite>();
    [SerializeField] private List<Sprite> moveLeft = new List<Sprite>();
    [SerializeField] private List<Sprite> moveRight = new List<Sprite>();
    [SerializeField] private List<Sprite> moveUp = new List<Sprite>();
    [SerializeField] private List<Sprite> moveDown = new List<Sprite>();
    [SerializeField] private List<Sprite> fight = new List<Sprite>();

    [SerializeField] private float frameRate = 12f;
    [SerializeField] private bool loop = true;

    private float timer;
    private int frameIndex;
    private AnimationState currentState = AnimationState.Idle;

    /// <summary>
    /// Exposes idle animation frames for modification.
    /// </summary>
    public List<Sprite> Idle => idle;

    /// <summary>
    /// Exposes move-left animation frames for modification.
    /// </summary>
    public List<Sprite> MoveLeft => moveLeft;

    /// <summary>
    /// Exposes move-right animation frames for modification.
    /// </summary>
    public List<Sprite> MoveRight => moveRight;

    /// <summary>
    /// Exposes move-up animation frames for modification.
    /// </summary>
    public List<Sprite> MoveUp => moveUp;

    /// <summary>
    /// Exposes move-down animation frames for modification.
    /// </summary>
    public List<Sprite> MoveDown => moveDown;

    /// <summary>
    /// Exposes fight animation frames for modification.
    /// </summary>
    public List<Sprite> Fight => fight;

    /// <summary>
    /// Frames per second for the active animation.
    /// </summary>
    public float FrameRate { get => frameRate; set => frameRate = value; }

    /// <summary>
    /// Determines whether the animation loops when reaching the end.
    /// </summary>
    public bool Loop { get => loop; set => loop = value; }

    private void Awake()
    {
        if (!spriteRenderer)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        Advance(Time.deltaTime);
    }

    /// <summary>
    /// Advances the animation by a time delta.
    /// </summary>
    /// <param name="deltaTime">Elapsed time in seconds.</param>
    public void Advance(float deltaTime)
    {
        var frames = GetCurrentFrames();
        if (frames == null || frames.Count == 0)
        {
            return;
        }

        timer += deltaTime;
        var frameDuration = 1f / Mathf.Max(frameRate, 0.0001f);
        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            frameIndex++;
            if (frameIndex >= frames.Count)
            {
                if (loop)
                {
                    frameIndex = 0;
                }
                else
                {
                    frameIndex = frames.Count - 1;
                }
            }
            spriteRenderer.sprite = frames[frameIndex];
        }
    }

    /// <summary>
    /// Sets the current animation state and resets frame playback.
    /// </summary>
    /// <param name="state">State to activate.</param>
    public void SetState(AnimationState state)
    {
        if (currentState == state)
        {
            return;
        }

        currentState = state;
        frameIndex = 0;
        timer = 0f;

        var frames = GetCurrentFrames();
        if (frames != null && frames.Count > 0)
        {
            spriteRenderer.sprite = frames[0];
        }
    }

    private List<Sprite> GetCurrentFrames()
    {
        return currentState switch
        {
            AnimationState.Idle => idle,
            AnimationState.MoveLeft => moveLeft,
            AnimationState.MoveRight => moveRight,
            AnimationState.MoveUp => moveUp,
            AnimationState.MoveDown => moveDown,
            AnimationState.Fight => fight,
            _ => null,
        };
    }

#if UNITY_EDITOR
    [ContextMenu("Append Selected Frame to Idle")]
    private void AppendSelectedFrameToIdle() => AppendSelectedSprite(idle);

    [ContextMenu("Append Selected Frame to MoveLeft")]
    private void AppendSelectedFrameToMoveLeft() => AppendSelectedSprite(moveLeft);

    [ContextMenu("Append Selected Frame to MoveRight")]
    private void AppendSelectedFrameToMoveRight() => AppendSelectedSprite(moveRight);

    [ContextMenu("Append Selected Frame to MoveUp")]
    private void AppendSelectedFrameToMoveUp() => AppendSelectedSprite(moveUp);

    [ContextMenu("Append Selected Frame to MoveDown")]
    private void AppendSelectedFrameToMoveDown() => AppendSelectedSprite(moveDown);

    [ContextMenu("Append Selected Frame to Fight")]
    private void AppendSelectedFrameToFight() => AppendSelectedSprite(fight);

    private void AppendSelectedSprite(List<Sprite> list)
    {
        var sprite = Selection.activeObject as Sprite;
        if (sprite != null)
        {
            list.Add(sprite);
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
