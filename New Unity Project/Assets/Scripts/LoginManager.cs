using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
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
        if (usernameField == null || passwordField == null ||
            debugServerToggle == null || kimServerToggle == null ||
            loginButton == null || createAccountButton == null)
        {
            CreateDefaultUI();
        }

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);
        if (createAccountButton != null)
            createAccountButton.onClick.AddListener(OnCreateAccountClicked);
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

        usernameField = CreateInputField(canvas.transform, "Username", new Vector2(0, 60));
        passwordField = CreateInputField(canvas.transform, "Password", new Vector2(0, 0));
        debugServerToggle = CreateToggle(canvas.transform, "Debug Server", new Vector2(-80, -60));
        kimServerToggle = CreateToggle(canvas.transform, "Kim Server", new Vector2(80, -60));
        loginButton = CreateButton(canvas.transform, "Login", new Vector2(-60, -120));
        createAccountButton = CreateButton(canvas.transform, "Create Account", new Vector2(60, -120));
    }

    private InputField CreateInputField(Transform parent, string placeholder, Vector2 position)
    {
        var go = new GameObject(placeholder + "Input", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent);
        rt.sizeDelta = new Vector2(160, 30);
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
