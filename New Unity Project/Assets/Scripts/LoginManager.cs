using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using WinFormsApp2;

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
            DatabaseConfig.DebugMode = debugServerToggle != null && debugServerToggle.isOn;
            DatabaseConfig.UseKimServer = kimServerToggle != null && kimServerToggle.isOn;

            string hashed = HashPassword(password);
            var request = new LoginRequest { username = username, passwordHash = hashed };
            string json = JsonUtility.ToJson(request);
            using var req = new UnityWebRequest($"{DatabaseConfig.ApiBaseUrl}/login", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            await SendRequest(req);
            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);
                if (resp.success)
                {
                    InventoryServiceUnity.Load(resp.userId);
                    SceneManager.LoadScene("RPG");
                }
            }
        }
    }

    private static async System.Threading.Tasks.Task SendRequest(UnityWebRequest req)
    {
        var op = req.SendWebRequest();
        while (!op.isDone)
            await System.Threading.Tasks.Task.Yield();
    }

    [System.Serializable]
    private class LoginRequest
    {
        public string username = string.Empty;
        public string passwordHash = string.Empty;
    }

    [System.Serializable]
    private class LoginResponse
    {
        public bool success;
        public int userId;
        public string nickname = string.Empty;
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
