using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Navigation controller for the main RPG interface.  Buttons in the main scene
/// call these methods to load the matching sub-scenes.
/// </summary>
public class MainRPGNavigation : MonoBehaviour
{
    public void OpenShop() => SceneManager.LoadScene("Shop");
    public void OpenTavern() => SceneManager.LoadScene("Tavern");
    public void OpenArena() => SceneManager.LoadScene("Arena");
    public void OpenMailbox() => SceneManager.LoadScene("Mailbox");
    public void OpenBattle() => SceneManager.LoadScene("Battle");
}
