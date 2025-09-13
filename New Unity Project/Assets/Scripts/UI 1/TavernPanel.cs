using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WinFormsApp2;
using TMPro;

/// <summary>
/// Unity UI wrapper for TavernManager interactions.
/// Displays recruit candidates and allows hiring or joining parties.
/// </summary>
public class TavernPanel : MonoBehaviour
{
    [SerializeField] private RectTransform candidateListParent = null!;
    [SerializeField] private TavernManager tavernManager = null!;

    private readonly List<TavernManager.Recruit> _candidates = new();

    private async void Start()
    {
        if (tavernManager == null)
            tavernManager = FindObjectOfType<TavernManager>();
        await RefreshAsync();
    }

    /// <summary>
    /// Reload candidate list from the server and rebuild the UI.
    /// </summary>
    private async Task RefreshAsync()
    {
        int accountId = InventoryServiceUnity.AccountId;
        _candidates.Clear();
        _candidates.AddRange(await tavernManager.GetCandidatesAsync(accountId));

        if (candidateListParent == null)
        {
            var canvas = FindObjectOfType<Canvas>() ?? new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
            candidateListParent = canvas.transform as RectTransform;
        }

        foreach (Transform child in candidateListParent)
            Destroy(child.gameObject);

        foreach (var c in _candidates)
        {
            var entry = new GameObject($"Candidate_{c.id}", typeof(RectTransform));
            entry.transform.SetParent(candidateListParent, false);

            var nameGO = new GameObject("Name", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var nameRT = nameGO.GetComponent<RectTransform>();
            nameRT.SetParent(entry.transform);
            nameRT.sizeDelta = new Vector2(200, 30);
            var label = nameGO.GetComponent<TextMeshProUGUI>();
            label.text = $"{c.name} (Lv {c.level})";
            label.color = Color.black;

            var hireBtn = CreateButton(entry.transform, "Hire", new Vector2(110, 0));
            int recruitId = c.id;
            hireBtn.onClick.AddListener(async () =>
            {
                if (await tavernManager.HireAsync(accountId, recruitId))
                    await RefreshAsync();
            });

            var joinBtn = CreateButton(entry.transform, "Join Party", new Vector2(220, 0));
            joinBtn.onClick.AddListener(OnJoinParty);
        }
    }

    /// <summary>
    /// Helper for dynamically creating UI buttons.
    /// </summary>
    private Button CreateButton(Transform parent, string label, Vector2 position)
    {
        var go = new GameObject(label + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(90, 30);
        rt.anchoredPosition = position;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        var text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;

        return go.GetComponent<Button>();
    }

    /// <summary>
    /// Placeholder for opening the multiplayer party join flow.
    /// Refreshes candidate list afterward.
    /// </summary>
    private async void OnJoinParty()
    {
        // Placeholder: actual implementation would open a join-party window.
        await RefreshAsync();
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}

