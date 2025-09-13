using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Simplified Unity wrapper for battle interactions.
/// </summary>
public class BattlePanel : MonoBehaviour
{
    [SerializeField] private ArenaManager arenaManager;
    [SerializeField] private TextMeshProUGUI summaryText;

    [Serializable]
    private class ChallengeResponse
    {
        public bool battleReady;
        public string summary;
    }

    public async void StartBattle()
    {
        if (arenaManager == null)
            arenaManager = FindObjectOfType<ArenaManager>();

        if (arenaManager == null)
        {
            Debug.LogError("ArenaManager not found.");
            return;
        }

        int accountId = PlayerPrefs.GetInt("AccountId", 0);
        var json = await arenaManager.ChallengeAsync(accountId);
        if (string.IsNullOrEmpty(json))
        {
            if (summaryText != null)
                summaryText.text = "Unable to start battle.";
            return;
        }

        ChallengeResponse resp = null;
        try
        {
            resp = JsonUtility.FromJson<ChallengeResponse>(json);
        }
        catch
        {
            // ignore parse errors and handle below
        }

        if (resp != null && resp.battleReady)
        {
            MainRPGNavigation.OpenBattle();
        }
        else
        {
            if (summaryText != null)
                summaryText.text = resp?.summary ?? json;
        }
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
