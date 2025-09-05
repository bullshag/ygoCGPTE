using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityClient;

/// <summary>
/// Handles arena battle flow against the database.
/// </summary>
public class ArenaManager : MonoBehaviour
{
    /// <summary>
    /// Initiates a battle and returns the result payload.
    /// </summary>
    public async Task<string> ChallengeAsync(int accountId)
    {
        Debug.Log($"Starting arena challenge for account {accountId}.");

        var opponentSqlPath = Path.Combine(Application.dataPath, "sql", "unity_arena_select_opponent.sql");
        var opponentRows = await DatabaseClientUnity.QueryAsync(
            File.ReadAllText(opponentSqlPath),
            new Dictionary<string, object?> { ["@id"] = accountId });

        int opponentId = opponentRows.Count > 0
            ? Convert.ToInt32(opponentRows[0]["account_id"])
            : 0;

        Debug.Log($"Selected opponent {opponentId}.");

        var resultPayload = JsonUtility.ToJson(new
        {
            battleReady = opponentId > 0,
            summary = opponentId > 0 ? $"Opponent {opponentId} found." : "No opponents available."
        });

        var sqlPath = Path.Combine(Application.dataPath, "sql", "unity_arena_challenge.sql");
        string sql = File.ReadAllText(sqlPath);
        foreach (var statement in sql.Split(';'))
        {
            var trimmed = statement.Trim();
            if (trimmed.Length == 0) continue;
            await DatabaseClientUnity.ExecuteAsync(trimmed, new Dictionary<string, object?>
            {
                ["@accountId"] = accountId,
                ["@opponentId"] = opponentId,
                ["@log"] = resultPayload
            });
        }

        Debug.Log($"Result payload: {resultPayload}");
        return resultPayload;
    }
}
