using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ChatService
{
    public static async Task<List<string>> FetchMessagesAsync()
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_chat_messages.sql");
        try
        {
            var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath));
            var messages = new List<string>();
            foreach (var row in rows)
            {
                if (row.TryGetValue("message", out var value))
                {
                    var msg = Convert.ToString(value);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        messages.Add(msg);
                    }
                }
            }
            Debug.Log($"Fetched {messages.Count} chat messages");
            return messages;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch chat messages: {ex.Message}");
            return new List<string>();
        }
    }
}
