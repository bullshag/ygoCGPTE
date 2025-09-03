using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WinFormsApp2;

public class RegisterManager : MonoBehaviour
{
    public InputField usernameField;
    public InputField nicknameField;
    public InputField passwordField;
    public InputField confirmPasswordField;
    public Toggle debugServerToggle;
    public Toggle kimServerToggle;
    public Button registerButton;

    private void Start()
    {
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private void OnDestroy()
    {
        if (registerButton != null)
            registerButton.onClick.RemoveListener(OnRegisterClicked);
    }

    private void OnRegisterClicked()
    {
        string user = usernameField != null ? usernameField.text : string.Empty;
        string nick = nicknameField != null ? nicknameField.text : string.Empty;
        string pass = passwordField != null ? passwordField.text : string.Empty;
        string confirm = confirmPasswordField != null ? confirmPasswordField.text : string.Empty;

        if (user.Contains(" ") || pass.Contains(" ") || nick.Contains(" "))
        {
            Debug.Log("No spaces allowed in username, nickname or password");
            return;
        }
        if (user.Length < 3 || user.Length > 12 || pass.Length < 3 || pass.Length > 12 || nick.Length < 3 || nick.Length > 12)
        {
            Debug.Log("Username, nickname and password must be 3-12 characters");
            return;
        }
        if (pass != confirm)
        {
            Debug.Log("Passwords do not match");
            return;
        }

        DatabaseConfig.DebugMode = debugServerToggle != null && debugServerToggle.isOn;
        DatabaseConfig.UseKimServer = kimServerToggle != null && kimServerToggle.isOn;

        using var conn = new MySqlConnection(DatabaseConfig.ConnectionString);
        conn.Open();

        using var checkUser = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Username=@u", conn);
        checkUser.Parameters.AddWithValue("@u", user);
        int existsUser = Convert.ToInt32(checkUser.ExecuteScalar());
        if (existsUser > 0)
        {
            Debug.Log("Username already exists");
            return;
        }

        using var checkNick = new MySqlCommand("SELECT COUNT(1) FROM Users WHERE Nickname=@n", conn);
        checkNick.Parameters.AddWithValue("@n", nick);
        int existsNick = Convert.ToInt32(checkNick.ExecuteScalar());
        if (existsNick > 0)
        {
            Debug.Log("Nickname already exists");
            return;
        }

        string insertSql = File.ReadAllText(Path.Combine(Application.dataPath, "../create_user.sql"));
        using var insert = new MySqlCommand(insertSql, conn);
        insert.Parameters.AddWithValue("@u", user);
        insert.Parameters.AddWithValue("@n", nick);
        insert.Parameters.AddWithValue("@p", HashPassword(pass));
        insert.ExecuteNonQuery();
        long newId = insert.LastInsertedId;

        using (var ensureNode = new MySqlCommand("INSERT IGNORE INTO nodes (id, name) VALUES (@node, @name)", conn))
        {
            ensureNode.Parameters.AddWithValue("@node", "nodeRiverVillage");
            ensureNode.Parameters.AddWithValue("@name", "River Village");
            ensureNode.ExecuteNonQuery();
        }

        string travelSql = File.ReadAllText(Path.Combine(Application.dataPath, "../init_travel_state.sql"));
        using var initTravel = new MySqlCommand(travelSql, conn);
        initTravel.Parameters.AddWithValue("@a", newId);
        initTravel.Parameters.AddWithValue("@node", "nodeRiverVillage");
        initTravel.ExecuteNonQuery();

        Debug.Log("Account created");
        SceneManager.LoadScene("Login");
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
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
