using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityClient;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WinFormsApp2;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

public class RPGManager : MonoBehaviour
{
    [Header("UI References")]
    public List<GameObject> partyMemberEntries = new();
    public List<GameObject> mercBacks = new();
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI chatText;
    public GameObject scrollviewObj;
    [SerializeField] private ScrollRect chatScrollView;
    [SerializeField] private Image worldMapImage;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_InputField friendInput;
    [SerializeField] private TMP_Text friendListText;
    [SerializeField] private List<GameObject> mercenaryUIContainers = new();

    private List<CharacterData> partyMembers = new List<CharacterData>();
    private List<CharacterData> hiredCompanions = new List<CharacterData>();
    private GameObject _selectedBlock;

    private async void Start()
    {
        chatScrollView = scrollviewObj.GetComponent<ScrollRect>();
        await LoadPartyMembersAsync();
        PopulatePartyList();
        if (chatInput != null)
        {
            chatInput.onSubmit.AddListener(_ => SendChatMessage());
        }
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendChatMessage);
        }
        InitializeCharacterBlocks();
        StartCoroutine(ChatLoop());
        StartCoroutine(RegenLoop());
        StartCoroutine(FriendLoop());
        if (friendInput != null)
        {
            friendInput.onSubmit.AddListener(OnFriendInputSubmit);
        }
    }

    private async Task LoadPartyMembersAsync()
    {
        partyMembers = await CharacterService.GetPartyMembersAsync();
        hiredCompanions = await CharacterService.GetHiredCompanionsAsync();
        if (goldText != null)
        {
            int gold = await CharacterService.GetGoldAsync();
            goldText.text = $"Gold: {gold}";
        }
    }

    private void PopulatePartyList()
    {
        if ((partyMemberEntries == null || partyMemberEntries.Count == 0) &&
            (mercBacks == null || mercBacks.Count == 0))
        {
            return;
        }

        for (int i = 0; i < partyMemberEntries.Count; i++)
        {
            var go = partyMemberEntries[i];
            if (i < partyMembers.Count)
            {
                go.SetActive(true);
                var member = partyMembers[i];
                go.name = member.Name;

                var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var t in texts)
                {
                    if (t.gameObject.name == "NameText")
                    {
                        t.text = member.Name;
                    }
                }

                var bar = go.GetComponentInChildren<ColoredProgressBar>();
                if (bar != null)
                {
                    bar.SetValue(member.HP / (float)member.MaxHP, member.Mana / (float)member.MaxMana);
                }

                var img = go.GetComponent<Image>();
                if (img != null && go != _selectedBlock)
                {
                    img.color = Color.yellow;
                }
            }
            else
            {
                go.SetActive(false);
            }
        }

        for (int i = 0; i < mercBacks.Count; i++)
        {
            var go = mercBacks[i];
            if (i < hiredCompanions.Count)
            {
                go.SetActive(true);
                var member = hiredCompanions[i];
                go.name = member.Name;

                var texts = go.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var t in texts)
                {
                    if (t.gameObject.name == "NameText")
                    {
                        t.text = member.Name;
                    }
                }

                var bar = go.GetComponentInChildren<ColoredProgressBar>();
                if (bar != null)
                {
                    bar.SetValue(member.HP / (float)member.MaxHP, member.Mana / (float)member.MaxMana);
                }
            }
            else
            {
                go.SetActive(false);
            }
        }
    }

    private void InitializeCharacterBlocks()
    {
        foreach (var characterBlock in partyMemberEntries)
        {
            if (characterBlock == null)
            {
                continue;
            }

            var image = characterBlock.GetComponent<Image>();
            if (image != null)
            {
                image.color = Color.yellow;
            }

            var button = characterBlock.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCharacterBlockClicked(characterBlock));
            }
        }
    }

    private void OnCharacterBlockClicked(GameObject block)
    {
        if (_selectedBlock != null)
        {
            var previousImage = _selectedBlock.GetComponent<Image>();
            if (previousImage != null)
            {
                previousImage.color = Color.yellow;
            }
        }

        _selectedBlock = block;

        var currentImage = block.GetComponent<Image>();
        if (currentImage != null)
        {
            currentImage.color = Color.red;
        }
    }

    private IEnumerator ChatLoop()
    {
        while (true)
        {
            var task = ChatService.GetMessagesAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception == null)
            {
                if (chatText != null)
                {
                    chatText.text = string.Empty;
                    foreach (var msg in task.Result)
                    {
                        chatText.text += $"\n{msg.Sender}: {msg.Message}";
                    }
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch chat messages: {task.Exception}");
            }

            Canvas.ForceUpdateCanvases();
            chatScrollView.verticalNormalizedPosition = 0f;
            yield return new WaitForSeconds(2f);
        }
    }

    public async void SendChatMessage()
    {
        if (chatInput == null) return;
        string message = chatInput.text.Trim();
        if (string.IsNullOrEmpty(message)) return;

        await ChatService.SendMessageAsync(InventoryServiceUnity.AccountId, null, message);
        chatInput.text = string.Empty;

        var messages = await ChatService.GetMessagesAsync();
        if (chatText != null)
        {
            chatText.text = string.Empty;
            foreach (var msg in messages)
            {
                chatText.text += $"\n{msg.Sender}: {msg.Message}";
            }
            Canvas.ForceUpdateCanvases();
            chatScrollView.verticalNormalizedPosition = 0f;
        }

    }

    private IEnumerator RegenLoop()
    {
        while (true)
        {
            foreach (var member in partyMembers)
            {
                member.RegenTick();
            }
            foreach (var merc in hiredCompanions)
            {
                merc.RegenTick();
            }
            PopulatePartyList();
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator FriendLoop()
    {
        while (true)
        {
            var friendsTask = FriendServiceUnity.GetFriendsAsync(InventoryServiceUnity.AccountId);
            var requestsTask = FriendServiceUnity.GetFriendRequestsAsync(InventoryServiceUnity.AccountId);
            yield return new WaitUntil(() => friendsTask.IsCompleted && requestsTask.IsCompleted);
            if (friendListText != null)
            {
                string friends = string.Join("\n", friendsTask.Result);
                string requests = string.Join("\n", requestsTask.Result);
                friendListText.text = $"Friends:\n{friends}\nRequests:\n{requests}";
            }
            yield return new WaitForSeconds(5f);
        }
    }

    private async void OnFriendInputSubmit(string text)
    {
        string nick = text.Trim();
        if (nick.Length == 0) return;
        await FriendServiceUnity.SendFriendRequestAsync(InventoryServiceUnity.AccountId, nick);
        if (friendInput != null)
            friendInput.text = string.Empty;
    }
}

[System.Serializable]
public class CharacterData
{
    public string Name;
    public int HP;
    public int MaxHP;
    public int Mana;
    public int MaxMana;

    public void RegenTick()
    {
        HP = Mathf.Min(MaxHP, HP + 1);
        Mana = Mathf.Min(MaxMana, Mana + 1);
    }
}

public static class CharacterDatabase
{
    public static List<CharacterData> GetPartyMembers()
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "get_party_members.sql");
        var rows = DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath)).GetAwaiter().GetResult();
        var members = new List<CharacterData>();
        foreach (var row in rows)
        {
            members.Add(new CharacterData
            {
                Name = Convert.ToString(row["name"]) ?? string.Empty,
                HP = Convert.ToInt32(row["hp"]),
                MaxHP = Convert.ToInt32(row["max_hp"]),
                Mana = Convert.ToInt32(row["mana"]),
                MaxMana = Convert.ToInt32(row["max_mana"])
            });
        }
        return members;
    }

    public static int GetGold()
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "get_gold.sql");
        var rows = DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath), new Dictionary<string, object?> { ["@id"] = 1 }).GetAwaiter().GetResult();
        return rows.Count > 0 && rows[0].TryGetValue("gold", out var g) ? Convert.ToInt32(g) : 0;
    }
}
