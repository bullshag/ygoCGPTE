using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityClient;

/// <summary>
/// Handles recruiting and tavern interactions via server calls.
/// Mirrors TavernForm operations like fetching candidates and hiring.
/// </summary>
public class TavernManager : MonoBehaviour
{
    private string BaseUrl => DatabaseConfigUnity.ApiBaseUrl;

    /// <summary>
    /// Fetch recruit candidates for the account.
    /// </summary>
    public async Task<List<Recruit>> GetCandidatesAsync(int accountId)
    {
        using var req = UnityWebRequest.Get($"{BaseUrl}/tavern/candidates?accountId={accountId}");
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
            return new List<Recruit>();
        return JsonUtility.FromJson<RecruitList>(req.downloadHandler.text).candidates;
    }

    /// <summary>
    /// Hire a specific recruit.
    /// </summary>
    public async Task<bool> HireAsync(int accountId, int recruitId)
    {
        var form = new WWWForm();
        form.AddField("accountId", accountId);
        form.AddField("recruitId", recruitId);
        using var req = UnityWebRequest.Post($"{BaseUrl}/tavern/hire", form);
        await req.SendWebRequest();
        return req.result == UnityWebRequest.Result.Success;
    }

    [System.Serializable]
    public class Recruit
    {
        public int id;
        public string name;
        public int level;
    }

    [System.Serializable]
    private class RecruitList
    {
        public List<Recruit> candidates;
    }
}
