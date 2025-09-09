using UnityEngine;

/// <summary>
/// Toggles a popup panel using Unity's Animator.
/// Wire <see cref="Toggle"/> to your Raw Image or Button.
/// </summary>
public class UIAnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;    // Animator controlling the popup
    [SerializeField] private string showTrigger = "Show";
    [SerializeField] private string hideTrigger = "Hide";

    private bool isOpen = true;

    /// <summary>
    /// Plays the appropriate animation to show or hide the popup.
    /// </summary>
    public void Toggle()
    {
        if (animator == null) return;

        animator.ResetTrigger(isOpen ? hideTrigger : showTrigger);
        animator.SetTrigger(isOpen ? hideTrigger : showTrigger);
        isOpen = !isOpen;
    }
}