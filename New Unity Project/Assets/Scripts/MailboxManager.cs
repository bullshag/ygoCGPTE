using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Retrieves player mail and interacts with the server-side mailbox.
/// </summary>
public class MailboxManager : MonoBehaviour
{
    /// <summary>
    /// Get unread mail for the given account.
    /// </summary>
    public async Task<List<MailMessage>> GetMailAsync(int accountId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_mailbox_unread.sql");
            var rows = await DatabaseClientUnity.QueryAsync(File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@accountId"] = accountId });
            var messages = new List<MailMessage>();
            foreach (var row in rows)
            {
                messages.Add(new MailMessage
                {
                    id = Convert.ToInt32(row["id"]),
                    senderId = Convert.ToInt32(row["sender_id"]),
                    subject = Convert.ToString(row["subject"]) ?? string.Empty,
                    body = Convert.ToString(row["body"]) ?? string.Empty
                });
            }
            Debug.Log($"Retrieved {messages.Count} messages for account {accountId}");
            return messages;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error retrieving mail for account {accountId}: {ex.Message}");
            return new List<MailMessage>();
        }
    }

    /// <summary>
    /// Mark a message as read.
    /// </summary>
    public async Task<bool> MarkReadAsync(int messageId)
    {
        try
        {
            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_mailbox_mark_read.sql");
            int rows = await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@messageId"] = messageId });
            Debug.Log($"Marked message {messageId} as read");
            return rows > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error marking message {messageId} as read: {ex.Message}");
            return false;
        }
    }

    [System.Serializable]
    public class MailMessage
    {
        public int id;
        public int senderId;
        public string subject;
        public string body;
    }

    [System.Serializable]
    private class MailList
    {
        public List<MailMessage> mail;
    }
}
