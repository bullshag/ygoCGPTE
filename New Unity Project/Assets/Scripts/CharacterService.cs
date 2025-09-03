using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class CharacterService
{
    private const string BaseUrl = "http://localhost:8080";

    [System.Serializable]
    private class PartyResponse
    {
        public List<CharacterData> party;
    }

    [System.Serializable]
    private class GoldResponse
    {
        public int gold;
    }

    public static async Task<List<CharacterData>> GetPartyMembersAsync()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/party");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to fetch party members: {request.error}");
            return new List<CharacterData>();
        }
        var json = request.downloadHandler.text;
        PartyResponse resp = JsonUtility.FromJson<PartyResponse>(json);
        return resp != null && resp.party != null ? resp.party : new List<CharacterData>();
    }

    public static async Task<int> GetGoldAsync()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/gold");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to fetch gold: {request.error}");
            return 0;
        }
        var json = request.downloadHandler.text;
        GoldResponse resp = JsonUtility.FromJson<GoldResponse>(json);
        return resp != null ? resp.gold : 0;
    }
}
