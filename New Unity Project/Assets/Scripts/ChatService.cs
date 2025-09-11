using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityClient
{
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

        public static async Task<int?> GetUserIdByNicknameAsync(string nickname)
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_chat_get_user_id.sql");
            try
            {
                var rows = await DatabaseClientUnity.QueryAsync(
                    File.ReadAllText(sqlPath),
                    new Dictionary<string, object?> { ["@nick"] = nickname }
                );
                if (rows.Count > 0 && rows[0].TryGetValue("id", out var id))
                {
                    return Convert.ToInt32(id);
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get user id for nickname {nickname}: {ex.Message}");
                return null;
            }
        }

        public static async Task SendMessageAsync(int senderId, int? recipientId, string message)

        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_chat_send_message.sql");
            try
            {
                var parameters = new Dictionary<string, object?>
                {
                    ["@sender"] = senderId,
                    ["@recipient"] = recipientId,
                    ["@message"] = message
                };
                await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath), parameters);
                Debug.Log("Chat message sent");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to send chat message: {ex.Message}");
            }
        }
    }
}

