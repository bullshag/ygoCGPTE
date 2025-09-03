using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RPGManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform partyListContent;
    [SerializeField] private GameObject partyMemberPrefab;
    [SerializeField] private Text goldText;
    [SerializeField] private Text chatText;

    private List<CharacterData> partyMembers = new List<CharacterData>();

    private async void Start()
    {
        await LoadPartyMembersAsync();
        PopulatePartyList();
        StartCoroutine(ChatLoop());
        StartCoroutine(RegenLoop());
    }

    private async Task LoadPartyMembersAsync()
    {
        partyMembers = await CharacterService.GetPartyMembersAsync();
        if (goldText != null)
        {
            int gold = await CharacterService.GetGoldAsync();
            goldText.text = $"Gold: {gold}";
        }
    }

    private void PopulatePartyList()
    {
        if (partyListContent == null || partyMemberPrefab == null)
        {
            return;
        }

        foreach (Transform child in partyListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var member in partyMembers)
        {
            var go = GameObject.Instantiate(partyMemberPrefab, partyListContent);
            go.name = member.Name;

            var texts = go.GetComponentsInChildren<Text>();
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
    }

    private IEnumerator ChatLoop()
    {
        while (true)
        {
            var task = ChatService.FetchMessagesAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (chatText != null)
            {
                foreach (var msg in task.Result)
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        chatText.text += "\n" + msg;
                    }
                }
            }
            yield return new WaitForSeconds(2f);
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
            PopulatePartyList();
            yield return new WaitForSeconds(1f);
        }
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
