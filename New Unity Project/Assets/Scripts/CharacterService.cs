using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityClient;
using WinFormsApp2;

public static class CharacterService
{
    private static readonly List<CharacterData> PartyMembersCache = new();

    public static IReadOnlyList<CharacterData> CachedPartyMembers => PartyMembersCache;

    public static async Task<List<CharacterData>> GetPartyMembersAsync()
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_get_main_party_members.sql");
            Debug.Log("Executing party members query");
            var rows = await DatabaseClientUnity.QueryAsync(
                File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@id"] = InventoryServiceUnity.AccountId }
            );
            Debug.Log($"Party members returned: {rows.Count}");

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

            PartyMembersCache.Clear();
            PartyMembersCache.AddRange(members);

            return new List<CharacterData>(PartyMembersCache);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch party members: {ex.Message}");
            return new List<CharacterData>();
        }
    }

    public static async Task<int> GetGoldAsync()
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_get_gold_users.sql");
            Debug.Log("Executing gold query");
            var rows = await DatabaseClientUnity.QueryAsync(
                File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@id"] = InventoryServiceUnity.AccountId }
            );
            Debug.Log($"Gold rows returned: {rows.Count}");

            return rows.Count > 0 && rows[0].TryGetValue("gold", out var g)
                ? Convert.ToInt32(g)
                : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch gold: {ex.Message}");
            return 0;
        }
    }

    public static async Task<List<CharacterData>> GetHiredCompanionsAsync()
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_get_hired_companions.sql");
            Debug.Log("Executing hired companions query");
            var rows = await DatabaseClientUnity.QueryAsync(
                File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@id"] = InventoryServiceUnity.AccountId }
            );
            Debug.Log($"Hired companions returned: {rows.Count}");

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
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch hired companions: {ex.Message}");
            return new List<CharacterData>();
        }
    }

    public static Task AddPartyMemberAsync(CharacterData data)
    {
        if (data != null)
        {
            PartyMembersCache.Add(data);
        }

        return Task.CompletedTask;
    }
}
