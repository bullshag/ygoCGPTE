using UnityEngine;
using WinFormsApp2;

/// <summary>
/// Unity UI wrapper for arena interactions.
/// </summary>
public class ArenaPanel : MonoBehaviour
{
    public void OnDeposit()
    {
        // Placeholder: deposit/withdraw team logic using InventoryServiceUnity
    }

    public void OnChallenge()
    {
        // Placeholder: challenge logic that would invoke BattleForm
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
