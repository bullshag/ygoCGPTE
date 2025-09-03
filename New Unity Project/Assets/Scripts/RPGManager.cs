using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        LoadPartyMembers();
        PopulatePartyList();
        StartCoroutine(ChatLoop());
        StartCoroutine(RegenLoop());
    }

    private void LoadPartyMembers()
    {
        partyMembers = CharacterDatabase.GetPartyMembers();
        if (goldText != null)
        {
            goldText.text = $"Gold: {CharacterDatabase.GetGold()}";
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
            string msg = ChatService.FetchNewMessage();
            if (!string.IsNullOrEmpty(msg) && chatText != null)
            {
                chatText.text += "\n" + msg;
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
        return new List<CharacterData>
        {
            new CharacterData { Name = "Hero", HP = 50, MaxHP = 100, Mana = 30, MaxMana = 50 },
            new CharacterData { Name = "Mage", HP = 40, MaxHP = 60, Mana = 80, MaxMana = 100 }
        };
    }

    public static int GetGold()
    {
        return 123;
    }
}

public static class ChatService
{
    private static Queue<string> messages = new Queue<string>(new[] { "Welcome to the world!" });

    public static string FetchNewMessage()
    {
        if (messages.Count > 0)
        {
            return messages.Dequeue();
        }
        return null;
    }
}
