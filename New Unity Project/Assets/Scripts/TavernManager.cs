using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles recruiting and tavern interactions via direct database calls.
/// Mirrors TavernForm operations like fetching candidates and hiring.
/// </summary>
public class TavernManager : MonoBehaviour
{
    /// <summary>
    /// Fetch recruit candidates for the account.
    /// </summary>
    public async Task<List<Recruit>> GetCandidatesAsync(int accountId)
    {
        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_tavern_candidates.sql");
        var rows = await DatabaseClientUnity.QueryAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?> { ["@accountId"] = accountId });

        var list = new List<Recruit>();
        foreach (var row in rows)
        {
            list.Add(new Recruit
            {
                id = Convert.ToInt32(row["id"]),
                name = Convert.ToString(row["name"]) ?? string.Empty,
                level = Convert.ToInt32(row["level"])
            });
        }

        Debug.Log($"Tavern candidates fetched: {list.Count}");
        return list;
    }

    /// <summary>
    /// Hire a specific recruit.
    /// </summary>
    public async Task<bool> HireAsync(int accountId, int recruitId)
    {
        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_tavern_hire.sql");
        int affected = await DatabaseClientUnity.ExecuteAsync(
            File.ReadAllText(sqlPath),
            new Dictionary<string, object?>
            {
                ["@userId"] = accountId,
                ["@recruitId"] = recruitId
            });

        bool success = affected > 0;
        Debug.Log($"Hire recruit {recruitId} success: {success}");
        return success;
    }

    [System.Serializable]
    public class Recruit
    {
        public int id;
        public string name;
        public int level;
    }

}
