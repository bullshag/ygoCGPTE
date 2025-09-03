using UnityEngine;
using WinFormsApp2;

/// <summary>
/// Unity UI wrapper for TavernForm interactions.
/// </summary>
public class TavernPanel : MonoBehaviour
{
    public void OnRecruit()
    {
        // Placeholder for recruit logic using TavernService
        TavernService.NotifyPartyHired(0);
    }

    public void OnJoinParty()
    {
        // Placeholder: open join party flow
    }

    public void BackToMain() => MainRPGNavigation.OpenMain();
}
