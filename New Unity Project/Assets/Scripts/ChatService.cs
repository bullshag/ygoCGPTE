using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class ChatService
{
    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string? Recipient { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public static async Task<List<ChatMessage>> GetMessagesAsync(DateTime since, int userId)
    {
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_chat_fetch_messages.sql");
        try
        {
            var parameters = new Dictionary<string, object?>
            {
                ["@since"] = since,
                ["@uid"] = userId
            };
            var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath), parameters);
            var messages = new List<ChatMessage>();
            foreach (var row in rows)
            {
                messages.Add(new ChatMessage
                {
                    Sender = Convert.ToString(row["sender"]) ?? string.Empty,
                    Recipient = row["recipient"] as string,
                    Message = Convert.ToString(row["message"]) ?? string.Empty,
                    SentAt = Convert.ToDateTime(row["sent_at"])
                });
            }
            Debug.Log($"Fetched {messages.Count} chat messages");
            return messages;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to fetch chat messages: {ex.Message}");
            return new List<ChatMessage>();
        }
    }
}
