using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Modal overlay for showing recruit details and hire actions.
/// </summary>
public class TavernRecruitDetailPanel : MonoBehaviour
{
    [SerializeField] private GameObject root = null!;
    [SerializeField] private TMP_Text nameText = null!;
    [SerializeField] private TMP_Text statsText = null!;
    [SerializeField] private TMP_Text costText = null!;
    [SerializeField] private Button hireButton = null!;
    [SerializeField] private Button cancelButton = null!;

    private void Awake()
    {
        if (root == null)
        {
            root = gameObject;
        }

        HideInstant();
    }

    public void Show(
        PartyMemberGenerator.GeneratedRecruit recruit,
        UnityAction onHire,
        UnityAction onCancel)
    {
        if (root == null)
        {
            Debug.LogWarning("Tavern recruit detail root not assigned.");
            return;
        }

        root.SetActive(true);

        if (nameText != null)
        {
            nameText.text = recruit.Name;
        }

        if (statsText != null)
        {
            statsText.text = recruit.BuildStatsDescription();
        }

        if (costText != null)
        {
            costText.text = $"Cost: {recruit.Cost} gold";
        }

        if (hireButton != null)
        {
            hireButton.onClick.RemoveAllListeners();
            hireButton.onClick.AddListener(onHire);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(onCancel);
        }
    }

    public void Hide()
    {
        if (hireButton != null)
        {
            hireButton.onClick.RemoveAllListeners();
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
        }

        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public void HideInstant()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }
}
