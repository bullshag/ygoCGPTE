using UnityEngine;
using UnityEngine.SceneManagement;
using WinFormsApp2;

/// <summary>
/// Simplified Unity wrapper for battle interactions.
/// </summary>
public class BattlePanel : MonoBehaviour
{
    public void StartBattle()
    {
        // Placeholder: launch a battle using BattleForm logic
    }

    public void BackToMain() => SceneManager.LoadScene("MainRPG");
}
