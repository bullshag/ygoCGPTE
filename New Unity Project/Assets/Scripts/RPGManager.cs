using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityClient;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class RPGManager : MonoBehaviour
{
    [Header("UI References")]
    public List<GameObject> partyMemberEntries = new();
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI chatText;

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
        if (partyMemberEntries == null || partyMemberEntries.Count == 0)
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
            }
            else
            {
                go.SetActive(false);
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

public static class CharacterDatabase
{
    public static List<CharacterData> GetPartyMembers()
    {
        string sqlPath = Path.Combine(AppContext.BaseDirectory, "get_party_members.sql");
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
        string sqlPath = Path.Combine(AppContext.BaseDirectory, "get_gold.sql");
        var rows = DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath), new Dictionary<string, object?> { ["@id"] = 1 }).GetAwaiter().GetResult();
        return rows.Count > 0 && rows[0].TryGetValue("gold", out var g) ? Convert.ToInt32(g) : 0;
    }
}
