using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class ChatService
{
    private const string BaseUrl = "http://localhost:8080";

    [System.Serializable]
    private class ChatResponse
    {
        public List<string> messages;
    }

    public static async Task<List<string>> FetchMessagesAsync()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/chat/messages");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to fetch chat messages: {request.error}");
            return new List<string>();
        }
        var json = request.downloadHandler.text;
        ChatResponse resp = JsonUtility.FromJson<ChatResponse>(json);
        return resp != null && resp.messages != null ? resp.messages : new List<string>();
    }
}
