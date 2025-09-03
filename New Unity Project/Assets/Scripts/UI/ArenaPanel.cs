using System.Linq;
using UnityEngine;
using WinFormsApp2;

/// <summary>
/// Unity UI wrapper for arena interactions.
/// </summary>
public class ArenaPanel : MonoBehaviour
{
    private bool _deposited;

    public void OnDeposit()
    {
        // Simple deposit/withdraw logic using the shared inventory service.
        // Depositing will consume an "Arena Coin" if present, withdrawing will add one back.
        InventoryServiceUnity.Load(1); // ensure inventory is loaded
        if (_deposited)
        {
            // Withdraw: return a coin to the player's inventory.
            InventoryServiceUnity.AddItem(new ArenaCoin());
            Debug.Log("Team withdrawn from arena.");
            _deposited = false;
        }
        else
        {
            var coin = InventoryServiceUnity.Items.FirstOrDefault(i => i.Item.Name == "Arena Coin");
            if (coin == null)
            {
                Debug.LogWarning("No Arena Coin available to deposit.");
                return;
            }
            InventoryServiceUnity.RemoveItem(coin.Item);
            Debug.Log("Team deposited to arena.");
            _deposited = true;
        }
    }

    public async void OnChallenge()
    {
        // Challenge the arena using the ArenaManager and report the outcome to the player.
        var mgr = FindObjectOfType<ArenaManager>();
        if (mgr == null)
        {
            Debug.LogError("ArenaManager not found.");
            return;
        }

        string? result = null;
        try
        {
            result = await mgr.ChallengeAsync(1);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Challenge failed: {ex.Message}");
        }

        if (string.IsNullOrEmpty(result))
            Debug.LogError("No result returned from arena challenge.");
        else
            Debug.Log($"Battle result: {result}");

        BackToMain();
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
