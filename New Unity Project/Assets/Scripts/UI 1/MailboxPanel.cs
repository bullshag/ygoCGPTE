using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Unity UI wrapper for viewing mail messages.
/// </summary>
public class MailboxPanel : MonoBehaviour
{
    [SerializeField] private MailboxManager mailboxManager = null!;
    [SerializeField] private Transform messageContainer = null!;
    [SerializeField] private GameObject messageEntryPrefab = null!;

    private List<MailboxManager.MailMessage> _mail = new();

    private async void Start()
    {
        await LoadMailAsync();
    }

    public async void Refresh() => await LoadMailAsync();

    private async Task LoadMailAsync()
    {
        // Placeholder uses account id 0; actual game would supply the logged in id.
        _mail = await mailboxManager.GetMailAsync(0);
        PopulateMessages();
    }

    private void PopulateMessages()
    {
        foreach (Transform child in messageContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var msg in _mail)
        {
            var entry = Instantiate(messageEntryPrefab, messageContainer);
            var label = entry.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = msg.subject;
            var button = entry.GetComponentInChildren<Button>();
            if (button != null)
            {
                int id = msg.id;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnMarkReadClicked(id));
            }
        }
    }

    private async void OnMarkReadClicked(int messageId)
    {
        if (await mailboxManager.MarkReadAsync(messageId))
        {
            await LoadMailAsync();
        }
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
