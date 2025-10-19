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

    private readonly PartyMemberGenerator _generator = new();
    private readonly List<PartyMemberGenerator.GeneratedRecruit> _availableRecruits = new();
    private readonly List<Button> _spawnedButtons = new();
    private PartyMemberGenerator.GeneratedRecruit? _selectedRecruit;

    private Transform? CandidateContent => candidateScrollRect != null ? candidateScrollRect.content : null;

    private static readonly (int count, float weight)[] RecruitCountWeights =
    {
        (1, 0.28f),
        (2, 0.24f),
        (3, 0.2f),
        (4, 0.14f),
        (5, 0.09f),
        (6, 0.05f)
    };

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

        int accountId = InventoryServiceUnity.AccountId;
        List<TavernManager.Recruit> baseRecruits = await tavernManager.GetCandidatesAsync(accountId);
        int recruitCount = RollRecruitCount();
        _availableRecruits.Clear();
        _availableRecruits.AddRange(_generator.BuildCandidates(baseRecruits, recruitCount));

        RebuildCandidateButtons();
    }

    private static int RollRecruitCount()
    {
        float totalWeight = 0f;
        foreach (var option in RecruitCountWeights)
        {
            totalWeight += option.weight;
        }

        if (totalWeight <= 0f)
        {
            return 1;
        }

        float roll = Random.value * totalWeight;
        foreach (var option in RecruitCountWeights)
        {
            if (roll <= option.weight)
            {
                return option.count;
            }

            roll -= option.weight;
        }

        return RecruitCountWeights[^1].count;
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

        foreach (var recruit in _availableRecruits)
        {
            var candidateGO = Instantiate(candidateButtonPrefab, content);
            var button = candidateGO.GetComponent<Button>();
            if (button == null)
            {
                button = candidateGO.AddComponent<Button>();
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
        bool success = await tavernManager.HireAsync(accountId, _selectedRecruit.Source.id);
        if (!success)
        {
            Debug.LogWarning($"Failed to hire recruit {_selectedRecruit.Source.name}.");
            CloseDetailPanel();
            return;
        }

        await CharacterService.AddPartyMemberAsync(_selectedRecruit.ToCharacterData());

        _availableRecruits.Remove(_selectedRecruit);
        _selectedRecruit = null;

        CloseDetailPanel();
        RebuildCandidateButtons();
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
