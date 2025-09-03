using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Retrieves player mail and interacts with the server-side mailbox.
/// </summary>
public class MailboxManager : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://localhost:5001";

    /// <summary>
    /// Get unread mail for the given account.
    /// </summary>
    public async Task<List<MailMessage>> GetMailAsync(int accountId)
    {
        using var req = UnityWebRequest.Get($"{baseUrl}/mail/unread?accountId={accountId}");
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
            return new List<MailMessage>();
        return JsonUtility.FromJson<MailList>(req.downloadHandler.text).mail;
    }

    /// <summary>
    /// Mark a message as read.
    /// </summary>
    public async Task<bool> MarkReadAsync(int messageId)
    {
        var form = new WWWForm();
        form.AddField("messageId", messageId);
        using var req = UnityWebRequest.Post($"{baseUrl}/mail/markRead", form);
        await req.SendWebRequest();
        return req.result == UnityWebRequest.Result.Success;
    }

    [System.Serializable]
    public class MailMessage
    {
        public int id;
        public string subject;
        public string body;
    }

    [System.Serializable]
    private class MailList
    {
        public List<MailMessage> mail;
    }
}
