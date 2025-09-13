using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityClient;
using WinFormsApp2;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Toggle debugServerToggle;
    public Toggle kimServerToggle;
    public Button loginButton;
    public Button createAccountButton;
    public PopupWindow popupWindowPrefab;

    private void Start()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);
        if (createAccountButton != null)
            createAccountButton.onClick.AddListener(OnCreateAccountClicked);
    }

    private void OnDestroy()
    {
        if (loginButton != null)
            loginButton.onClick.RemoveListener(OnLoginClicked);
        if (createAccountButton != null)
            createAccountButton.onClick.RemoveListener(OnCreateAccountClicked);
    }

    private async void OnLoginClicked()
    {
        string username = usernameField != null ? usernameField.text : string.Empty;
        string password = passwordField != null ? passwordField.text : string.Empty;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowPopup("Please enter username and password.");
            return;
        }

        Debug.Log($"Login attempt for '{username}'");
        DatabaseConfigUnity.DebugMode = debugServerToggle != null && debugServerToggle.isOn;
        DatabaseConfigUnity.UseKimServer = kimServerToggle != null && kimServerToggle.isOn;

        string hashed = password; //HashPassword(password);
        string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_login_select_user.sql");
        Debug.Log("Executing login query");
        try
        {
            var rows = await DatabaseClientUnity.QueryAsync(
                File.ReadAllText(sqlPath),
                new Dictionary<string, object?> { ["@username"] = username, ["@passwordHash"] = hashed });

            if (rows.Count > 0)
            {
                Debug.Log("Login successful");
                int userId = Convert.ToInt32(rows[0]["id"]);
                string updatePath = Path.Combine(Application.dataPath, "sql", "unity_login_update_last_seen.sql");
                await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(updatePath), new Dictionary<string, object?> { ["@id"] = userId });
                await InventoryServiceUnity.LoadAsync(userId);
                SceneManager.LoadScene("RPG");
            }
            else
            {
                Debug.Log("Login failed");
                ShowPopup("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Login error: {ex.Message}");
            ShowPopup($"Login error: {ex.Message}");
        }
    }

    private void ShowPopup(string message)
    {
        if (popupWindowPrefab == null)
        {
            Debug.LogWarning("PopupWindow prefab not assigned.");
            return;
        }

        var canvas = FindObjectOfType<Canvas>();
        var popup = Instantiate(popupWindowPrefab, canvas != null ? canvas.transform : null);
        popup.Show(message);
    }

   // private string HashPassword(string password)
   // {
     //   using (var sha = SHA256.Create())
     //   {
     //       byte[] bytes = Encoding.UTF8.GetBytes(password);
    //        byte[] hash = sha.ComputeHash(bytes);
    //        return Convert.ToBase64String(hash);
   //     }
  //  }
    private void OnCreateAccountClicked()
    {
        SceneManager.LoadScene("Register");
    }
}
