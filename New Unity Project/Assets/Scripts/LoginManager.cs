using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityClient;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public Toggle debugServerToggle;
    public Toggle kimServerToggle;
    public Button loginButton;
    public Button createAccountButton;

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

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            Debug.Log($"Login attempt for '{username}'");
            DatabaseConfigUnity.DebugMode = debugServerToggle != null && debugServerToggle.isOn;
            DatabaseConfigUnity.UseKimServer = kimServerToggle != null && kimServerToggle.isOn;

            string sqlPath = Path.Combine(Application.dataPath, "sql", "unity_login_select_user_plain.sql");
            Debug.Log("Executing login query");
            try
            {
                var rows = await DatabaseClientUnity.QueryAsync(
                    File.ReadAllText(sqlPath),
                    new Dictionary<string, object?> { ["@username"] = username, ["@password"] = password });

                if (rows.Count > 0)
                {
                    Debug.Log("Login successful");
                    int userId = Convert.ToInt32(rows[0]["id"]);
                    string updatePath = Path.Combine(Application.dataPath, "sql", "unity_login_update_last_seen.sql");
                    await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(updatePath), new Dictionary<string, object?> { ["@id"] = userId });
                    InventoryServiceUnity.Load(userId);
                    SceneManager.LoadScene("RPG");
                }
                else
                {
                    Debug.Log("Login failed");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Login error: {ex.Message}");
            }
        }
    }

    private void OnCreateAccountClicked()
    {
        SceneManager.LoadScene("Register");
    }
}
