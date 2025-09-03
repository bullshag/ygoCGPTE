using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityClient;

/// <summary>
/// Handles arena battle flow against the server.
/// </summary>
public class ArenaManager : MonoBehaviour
{
    private string BaseUrl => DatabaseConfigUnity.ApiBaseUrl;

    /// <summary>
    /// Initiates a battle and returns the server provided result payload.
    /// </summary>
    public async Task<string> ChallengeAsync(int accountId)
    {
        var form = new WWWForm();
        form.AddField("accountId", accountId);
        using var req = UnityWebRequest.Post($"{BaseUrl}/arena/challenge", form);
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
            return null;
        return req.downloadHandler.text;
    }
}
