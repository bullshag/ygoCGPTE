using UnityEngine.SceneManagement;

/// <summary>
/// Navigation controller for the main RPG interface.  Buttons in any scene
/// call these methods to load the matching sub-scenes.
/// </summary>
public static class MainRPGNavigation
{
    public static void OpenMain() => SceneManager.LoadScene("MainRPG");
    public static void OpenShop() => SceneManager.LoadScene("Shop");
    public static void OpenTavern() => SceneManager.LoadScene("Tavern");
    public static void OpenArena() => SceneManager.LoadScene("Arena");
    public static void OpenMailbox() => SceneManager.LoadScene("Mailbox");
    public static void OpenBattle() => SceneManager.LoadScene("Battle");
}
