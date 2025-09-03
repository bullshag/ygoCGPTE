using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles arena battle flow against the server.
/// </summary>
public class ArenaManager : MonoBehaviour
{
    [SerializeField] private string baseUrl = "https://localhost:5001";

    /// <summary>
    /// Initiates a battle and returns the server provided result payload.
    /// </summary>
    public async Task<string> ChallengeAsync(int accountId)
    {
        var form = new WWWForm();
        form.AddField("accountId", accountId);
        using var req = UnityWebRequest.Post($"{baseUrl}/arena/challenge", form);
        await req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
            return null;
        return req.downloadHandler.text;
    }
}
