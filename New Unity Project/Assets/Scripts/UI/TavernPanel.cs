using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WinFormsApp2;

/// <summary>
/// Structured controller for Tavern activities. Manages recruit discovery, detail overlays,
/// and wiring to tavern services.
/// </summary>
public class TavernPanel : MonoBehaviour
{
    [Header("Top-Level Actions")]
    [SerializeField] private Button searchPartyMembersButton = null!;
    [SerializeField] private Button hireMercenariesButton = null!;
    [SerializeField] private Button lookForWorkButton = null!;

    [Header("Candidate List")]
    [SerializeField] private ScrollRect candidateScrollRect = null!;
    [SerializeField] private GameObject candidateButtonPrefab = null!;

    [Header("Detail Overlay")]
    [SerializeField] private TavernRecruitDetailPanel recruitDetailPanel = null!;

    [Header("Services")]
    [SerializeField] private TavernManager tavernManager = null!;
    [SerializeField] private RPGManager rpgManager = null!;

    private readonly List<PartyMemberGenerator.GeneratedRecruit> _availableRecruits = new();
    private readonly List<Button> _spawnedButtons = new();
    private PartyMemberGenerator.GeneratedRecruit? _selectedRecruit;
    private string activeNodeId = string.Empty;

    private Transform? CandidateContent => candidateScrollRect != null ? candidateScrollRect.content : null;

    private void Awake()
    {
        if (recruitDetailPanel != null)
        {
            recruitDetailPanel.HideInstant();
        }

        if (candidateScrollRect != null)
        {
            candidateScrollRect.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        if (tavernManager == null)
        {
            tavernManager = FindObjectOfType<TavernManager>();
        }

        if (rpgManager == null)
        {
            rpgManager = FindObjectOfType<RPGManager>();
        }

        WireTopLevelButtons();
        ConfigureTodoButton(hireMercenariesButton);
        ConfigureTodoButton(lookForWorkButton);
    }

    public void SetActiveNode(string nodeId)
    {
        activeNodeId = nodeId ?? string.Empty;
    }

    private void WireTopLevelButtons()
    {
        if (searchPartyMembersButton != null)
        {
            searchPartyMembersButton.onClick.RemoveAllListeners();
            searchPartyMembersButton.onClick.AddListener(() => _ = OnSearchForPartyMembersAsync());
        }
    }

    private void ConfigureTodoButton(Button? button)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.interactable = false;

        var label = button.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.text = $"{label.text} (Coming Soon)";
            label.alpha = 0.5f;
        }
    }

    private async Task OnSearchForPartyMembersAsync()
    {
        if (searchPartyMembersButton != null)
        {
            searchPartyMembersButton.interactable = false;
        }

        try
        {
            await PopulateRecruitListAsync();
        }
        finally
        {
            if (searchPartyMembersButton != null)
            {
                searchPartyMembersButton.interactable = true;
            }
        }
    }

    private async Task PopulateRecruitListAsync()
    {
        if (tavernManager == null || candidateScrollRect == null || candidateButtonPrefab == null)
        {
            Debug.LogWarning("TavernPanel missing required references for candidate generation.");
            return;
        }

        CloseDetailPanel();

        if (string.IsNullOrWhiteSpace(activeNodeId))
        {
            Debug.LogWarning("TavernPanel cannot populate recruits without an active node identifier.");
            return;
        }

        int accountId = InventoryServiceUnity.AccountId;
        List<PartyMemberGenerator.GeneratedRecruit> recruits = await tavernManager.GetCandidatesAsync(accountId, activeNodeId);
        _availableRecruits.Clear();
        _availableRecruits.AddRange(recruits);

        RebuildCandidateButtons();
    }

    private void RebuildCandidateButtons()
    {
        foreach (var button in _spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        _spawnedButtons.Clear();

        var content = CandidateContent;
        if (content == null)
        {
            Debug.LogWarning("TavernPanel candidate content is not assigned.");
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        var layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null && layoutGroup.enabled)
        {
            layoutGroup.enabled = false;
        }

        var sizeFitter = content.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null && sizeFitter.enabled)
        {
            sizeFitter.enabled = false;
        }

        var contentRect = content as RectTransform;
        if (contentRect != null)
        {
            contentRect.anchorMin = new Vector2(0.5f, 1f);
            contentRect.anchorMax = new Vector2(0.5f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
        }

        for (int index = 0; index < _availableRecruits.Count; index++)
        {
            var recruit = _availableRecruits[index];
            var candidateGO = Instantiate(candidateButtonPrefab, content);
            var button = candidateGO.GetComponent<Button>();
            if (button == null)
            {
                button = candidateGO.AddComponent<Button>();
            }

            var rectTransform = candidateGO.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                float yOffset = 113f - (74f * index);
                rectTransform.anchoredPosition = new Vector2(0f, yOffset);
                rectTransform.anchoredPosition3D = new Vector3(0f, yOffset, 0f);
            }

            var label = candidateGO.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = recruit.DisplayLabel;
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnRecruitSelected(recruit));
            _spawnedButtons.Add(button);
        }

        candidateScrollRect.gameObject.SetActive(_availableRecruits.Count > 0);
    }

    private void OnRecruitSelected(PartyMemberGenerator.GeneratedRecruit recruit)
    {
        _selectedRecruit = recruit;
        if (candidateScrollRect != null)
        {
            candidateScrollRect.gameObject.SetActive(false);
        }

        if (recruitDetailPanel == null)
        {
            Debug.LogWarning("Recruit detail panel not assigned.");
            if (candidateScrollRect != null)
            {
                candidateScrollRect.gameObject.SetActive(true);
            }
            return;
        }

        recruitDetailPanel.Show(
            recruit,
            () => _ = HireSelectedRecruitAsync(),
            CloseDetailPanel);
    }

    private async Task HireSelectedRecruitAsync()
    {
        if (_selectedRecruit == null || tavernManager == null)
        {
            return;
        }

        int accountId = InventoryServiceUnity.AccountId;
        bool success = await tavernManager.HireAsync(accountId, _selectedRecruit.Source.id, activeNodeId);
        if (!success)
        {
            Debug.LogWarning($"Failed to hire recruit {_selectedRecruit.Source.name}.");
            CloseDetailPanel();
            return;
        }

        await CharacterService.AddPartyMemberAsync(_selectedRecruit.ToCharacterData());

        _selectedRecruit = null;

        CloseDetailPanel();
        await PopulateRecruitListAsync();
        await RefreshPartyDisplayAsync();
    }

    private void CloseDetailPanel()
    {
        recruitDetailPanel?.Hide();
        _selectedRecruit = null;
        if (candidateScrollRect != null)
        {
            candidateScrollRect.gameObject.SetActive(true);
        }
    }

    private async Task RefreshPartyDisplayAsync()
    {
        if (rpgManager == null)
        {
            rpgManager = FindObjectOfType<RPGManager>();
        }

        if (rpgManager != null)
        {
            await rpgManager.RefreshPartyUIAsync();
        }
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
