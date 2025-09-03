using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WinFormsApp2;

/// <summary>
/// Unity UI wrapper for viewing mail messages.
/// </summary>
public class MailboxPanel : MonoBehaviour
{
    private List<MailItem> _mail = new();

    void Start()
    {
        LoadMail();
    }

    public void Refresh() => LoadMail();

    private void LoadMail()
    {
        // Placeholder uses account id 0; actual game would supply the logged in id.
        _mail = MailService.GetUnread(0);
    }

    public void BackToMain() => SceneManager.LoadScene("MainRPG");
}
