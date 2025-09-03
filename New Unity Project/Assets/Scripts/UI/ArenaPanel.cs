using UnityEngine;
using UnityEngine.SceneManagement;
using WinFormsApp2;

/// <summary>
/// Unity UI wrapper for arena interactions.
/// </summary>
public class ArenaPanel : MonoBehaviour
{
    public void OnDeposit()
    {
        // Placeholder: deposit/withdraw team logic using InventoryService
    }

    public void OnChallenge()
    {
        // Placeholder: challenge logic that would invoke BattleForm
    }

    public void BackToMain() => SceneManager.LoadScene("MainRPG");
}
