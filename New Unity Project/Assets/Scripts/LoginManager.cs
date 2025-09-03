using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WinFormsApp2;

public class LoginManager : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;
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
            DatabaseConfig.DebugMode = debugServerToggle != null && debugServerToggle.isOn;
            DatabaseConfig.UseKimServer = kimServerToggle != null && kimServerToggle.isOn;

            string hashed = HashPassword(password);
            string sql = "SELECT id, nickname FROM accounts WHERE username = @username AND password_hash = @passwordHash;";
            var parameters = new Dictionary<string, object?>
            {
                ["@username"] = username,
                ["@passwordHash"] = hashed
            };

            var results = await DatabaseClientUnity.QueryAsync(sql, parameters);
            if (results.Count > 0)
            {
                var row = results[0];
                int userId = Convert.ToInt32(row["id"]);
                await DatabaseClientUnity.ExecuteAsync(
                    "UPDATE accounts SET last_seen = NOW() WHERE id = @id",
                    new Dictionary<string, object?> { ["@id"] = userId });
                InventoryServiceUnity.Load(userId);
                SceneManager.LoadScene("RPG");
            }
        }
    }

    private string HashPassword(string password)
    {
        using (var sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            var builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    private void OnCreateAccountClicked()
    {
        SceneManager.LoadScene("Register");
    }
}
