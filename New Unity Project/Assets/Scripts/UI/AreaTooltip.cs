using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the visibility and interaction logic for an area tooltip prefab.
/// </summary>
[RequireComponent(typeof(Collider))]
public class AreaTooltip : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private string areaName = "TBD";

    [SerializeField]
    private string playerTag = "Player";

    [Header("UI References")]
    [SerializeField]
    private TMP_Text areaNameLabel = null!;

    [SerializeField]
    private Button infoButton = null!;

    [SerializeField]
    private Button enterButton = null!;

    [SerializeField]
    private Animator tooltipAnimator = null!;

    [Header("Animator States")]
    [Tooltip("State name for the show animation.")]
    [SerializeField]
    private string showStateName = "Show";

    [Tooltip("Trigger name that plays the show animation.")]
    [SerializeField]
    private string showTriggerName = "Show";

    [Tooltip("State name for the idle animation.")]
    [SerializeField]
    private string idleStateName = "Idle";

    [Tooltip("Animator cross-fade duration when switching to idle.")]
    [SerializeField]
    private float idleCrossFadeDuration = 0.1f;

    [Tooltip("Trigger name that plays the hide animation.")]
    [SerializeField]
    private string hideTriggerName = "Hide";

    [Header("Events")]
    public UnityEvent InfoClicked = new();
    public UnityEvent EnterClicked = new();

    private readonly WaitForEndOfFrame waitForEndOfFrame = new();
    private int showStateHash;
    private int idleStateHash;
    private int showTriggerHash;
    private int hideTriggerHash;
    private Coroutine idleCoroutine;
    private bool isPlayerInside;

    private void Awake()
    {
        ValidateSerializedFields();
        CacheAnimatorHashes();
        BindButtons();
        UpdateAreaNameLabel();

        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void OnValidate()
    {
        CacheAnimatorHashes();
        UpdateAreaNameLabel();
    }

    private void OnEnable()
    {
        UpdateAreaNameLabel();
    }

    private void OnDisable()
    {
        StopIdleCoroutine();
        isPlayerInside = false;
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        isPlayerInside = true;
        PlayShowAnimation();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag))
        {
            return;
        }

        isPlayerInside = false;
        StopIdleCoroutine();
        if (tooltipAnimator != null && hideTriggerHash != 0)
        {
            tooltipAnimator.ResetTrigger(showTriggerHash);
            tooltipAnimator.SetTrigger(hideTriggerHash);
        }
    }

    /// <summary>
    /// Updates the tooltip text with the configured area name.
    /// </summary>
    public void RefreshAreaName(string newAreaName)
    {
        areaName = newAreaName;
        UpdateAreaNameLabel();
    }

    private void UpdateAreaNameLabel()
    {
        if (areaNameLabel != null)
        {
            areaNameLabel.text = areaName;
        }
    }

    private void PlayShowAnimation()
    {
        if (tooltipAnimator == null)
        {
            return;
        }

        if (hideTriggerHash != 0)
        {
            tooltipAnimator.ResetTrigger(hideTriggerHash);
        }

        if (showTriggerHash == 0)
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Show trigger name is empty; cannot play show animation.", this);
            return;
        }

        tooltipAnimator.SetTrigger(showTriggerHash);

        RestartIdleCoroutine();
    }

    private void RestartIdleCoroutine()
    {
        if (string.IsNullOrEmpty(idleStateName) || string.IsNullOrEmpty(showStateName) || tooltipAnimator == null)
        {
            return;
        }

        StopIdleCoroutine();
        idleCoroutine = StartCoroutine(WaitForShowThenIdle());
    }

    private IEnumerator WaitForShowThenIdle()
    {
        // Ensure we are evaluating after Animator updates this frame.
        yield return waitForEndOfFrame;

        if (tooltipAnimator == null)
        {
            yield break;
        }

        var showInfo = tooltipAnimator.GetCurrentAnimatorStateInfo(0);
        var startHash = showInfo.shortNameHash;

        if (startHash != showStateHash)
        {
            // Wait for the show state to start playing.
            var watchdog = 0;
            while (watchdog++ < 60)
            {
                yield return null;
                showInfo = tooltipAnimator.GetCurrentAnimatorStateInfo(0);
                if (showInfo.shortNameHash == showStateHash)
                {
                    break;
                }
            }
        }

        showInfo = tooltipAnimator.GetCurrentAnimatorStateInfo(0);
        if (showInfo.shortNameHash != showStateHash)
        {
            yield break;
        }

        var normalizedTime = showInfo.normalizedTime;
        var remainingTime = (1f - (normalizedTime % 1f)) * showInfo.length / Mathf.Max(showInfo.speed, 0.001f);
        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        if (tooltipAnimator != null && idleStateHash != 0 && isPlayerInside)
        {
            tooltipAnimator.CrossFade(idleStateHash, idleCrossFadeDuration);
        }
    }

    private void StopIdleCoroutine()
    {
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }
    }

    private void CacheAnimatorHashes()
    {
        showStateHash = Animator.StringToHash(showStateName);
        idleStateHash = Animator.StringToHash(idleStateName);
        showTriggerHash = Animator.StringToHash(showTriggerName);
        hideTriggerHash = Animator.StringToHash(hideTriggerName);
    }

    private void BindButtons()
    {
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(HandleInfoClicked);
        }

        if (enterButton != null)
        {
            enterButton.onClick.AddListener(HandleEnterClicked);
        }
    }

    private void UnbindButtons()
    {
        if (infoButton != null)
        {
            infoButton.onClick.RemoveListener(HandleInfoClicked);
        }

        if (enterButton != null)
        {
            enterButton.onClick.RemoveListener(HandleEnterClicked);
        }
    }

    private void HandleInfoClicked()
    {
        InfoClicked.Invoke();
    }

    private void HandleEnterClicked()
    {
        EnterClicked.Invoke();
    }

    private void ValidateSerializedFields()
    {
        if (areaNameLabel == null)
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Area name label is not assigned.", this);
        }

        if (infoButton == null)
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Info button is not assigned.", this);
        }

        if (enterButton == null)
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Enter button is not assigned.", this);
        }

        if (tooltipAnimator == null)
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Animator is not assigned.", this);
        }

        if (string.IsNullOrWhiteSpace(showStateName))
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Show state name is empty; idle transition will be skipped.", this);
        }

        if (string.IsNullOrWhiteSpace(idleStateName))
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Idle state name is empty; idle transition will be skipped.", this);
        }

        if (string.IsNullOrWhiteSpace(showTriggerName))
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Show trigger name is empty; tooltip cannot animate in.", this);
        }

        if (string.IsNullOrWhiteSpace(hideTriggerName))
        {
            Debug.LogWarning($"[{nameof(AreaTooltip)}] Hide trigger name is empty; tooltip cannot animate out.", this);
        }
    }
}
