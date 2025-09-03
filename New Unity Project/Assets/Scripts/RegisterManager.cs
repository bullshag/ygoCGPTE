using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.EventSystems;
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
        if (usernameField == null || nicknameField == null || passwordField == null ||
            confirmPasswordField == null || debugServerToggle == null || kimServerToggle == null ||
            registerButton == null)
        {
            CreateDefaultUI();
        }

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private void OnDestroy()
    {
        if (registerButton != null)
            registerButton.onClick.RemoveListener(OnRegisterClicked);
    }

    private void CreateDefaultUI()
    {
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        if (FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        usernameField = CreateInputField(canvas.transform, "Username", new Vector2(0, 90));
        nicknameField = CreateInputField(canvas.transform, "Nickname", new Vector2(0, 50));
        passwordField = CreateInputField(canvas.transform, "Password", new Vector2(0, 10));
        passwordField.contentType = InputField.ContentType.Password;
        confirmPasswordField = CreateInputField(canvas.transform, "Confirm Password", new Vector2(0, -30));
        confirmPasswordField.contentType = InputField.ContentType.Password;
        debugServerToggle = CreateToggle(canvas.transform, "Debug Server", new Vector2(-80, -70));
        kimServerToggle = CreateToggle(canvas.transform, "Kim Server", new Vector2(80, -70));
        registerButton = CreateButton(canvas.transform, "Register", new Vector2(0, -120));
    }

    private InputField CreateInputField(Transform parent, string placeholder, Vector2 position)
    {
        var go = new GameObject(placeholder + "Input", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(200, 30);
        rt.anchoredPosition = position;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        var text = textGO.GetComponent<Text>();
        text.text = "";
        text.color = Color.black;

        var placeholderGO = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        var placeholderRT = placeholderGO.GetComponent<RectTransform>();
        placeholderRT.SetParent(go.transform);
        placeholderRT.anchorMin = new Vector2(0, 0);
        placeholderRT.anchorMax = new Vector2(1, 1);
        placeholderRT.offsetMin = Vector2.zero;
        placeholderRT.offsetMax = Vector2.zero;
        var placeholderText = placeholderGO.GetComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f);

        var input = go.GetComponent<InputField>();
        input.textComponent = text;
        input.placeholder = placeholderText;
        return input;
    }

    private Toggle CreateToggle(Transform parent, string label, Vector2 position)
    {
        var go = new GameObject(label + "Toggle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Toggle));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(160, 20);
        rt.anchoredPosition = position;

        var bg = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        var bgRT = bg.GetComponent<RectTransform>();
        bgRT.SetParent(go.transform);
        bgRT.sizeDelta = new Vector2(20, 20);
        bgRT.anchoredPosition = new Vector2(-70, 0);
        var checkmark = new GameObject("Checkmark", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        var cmRT = checkmark.GetComponent<RectTransform>();
        cmRT.SetParent(bg.transform);
        cmRT.sizeDelta = new Vector2(20, 20);

        var textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchoredPosition = new Vector2(10, 0);
        var text = textGO.GetComponent<Text>();
        text.text = label;
        text.color = Color.black;

        var toggle = go.GetComponent<Toggle>();
        toggle.graphic = checkmark.GetComponent<Image>();
        toggle.targetGraphic = bg.GetComponent<Image>();
        return toggle;
    }

    private Button CreateButton(Transform parent, string label, Vector2 position)
    {
        var go = new GameObject(label + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(120, 30);
        rt.anchoredPosition = position;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        var text = textGO.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;

        return go.GetComponent<Button>();
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
