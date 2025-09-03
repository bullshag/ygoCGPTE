using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using WinFormsApp2;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
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
        passwordField.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordField = CreateInputField(canvas.transform, "Confirm Password", new Vector2(0, -30));
        confirmPasswordField.contentType = TMP_InputField.ContentType.Password;
        debugServerToggle = CreateToggle(canvas.transform, "Debug Server", new Vector2(-80, -70));
        kimServerToggle = CreateToggle(canvas.transform, "Kim Server", new Vector2(80, -70));
        registerButton = CreateButton(canvas.transform, "Register", new Vector2(0, -120));
    }

    private TMP_InputField CreateInputField(Transform parent, string placeholder, Vector2 position)
    {
        var go = new GameObject(placeholder + "Input", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_InputField));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(200, 30);
        rt.anchoredPosition = position;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        var text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = "";
        text.color = Color.black;

        var placeholderGO = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        var placeholderRT = placeholderGO.GetComponent<RectTransform>();
        placeholderRT.SetParent(go.transform);
        placeholderRT.anchorMin = new Vector2(0, 0);
        placeholderRT.anchorMax = new Vector2(1, 1);
        placeholderRT.offsetMin = Vector2.zero;
        placeholderRT.offsetMax = Vector2.zero;
        var placeholderText = placeholderGO.GetComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f);

        var input = go.GetComponent<TMP_InputField>();
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

        var textGO = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchoredPosition = new Vector2(10, 0);
        var text = textGO.GetComponent<TextMeshProUGUI>();
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

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.SetParent(go.transform);
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        var text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;

        return go.GetComponent<Button>();
    }

    private async void OnRegisterClicked()

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

        string passwordHash = HashPassword(pass);
        var parameters = new Dictionary<string, object?>
        {
            ["@username"] = user,
            ["@nickname"] = nick,
            ["@passwordHash"] = passwordHash
        };
        Debug.Log($"Register params - username: {user}, nickname: {nick}, hash: {passwordHash}");

        string sqlPath = Path.Combine(AppContext.BaseDirectory, "unity_register_user.sql");
        try
        {
            int rows = await DatabaseClientUnity.ExecuteAsync(File.ReadAllText(sqlPath), parameters);
            Debug.Log($"Insert result: {rows}");
            if (rows > 0)
            {
                Debug.Log("Account created");
                SceneManager.LoadScene("Login");
            }
            else
            {
                Debug.Log("No account created");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Registration failed: {ex.Message}");
        }
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
