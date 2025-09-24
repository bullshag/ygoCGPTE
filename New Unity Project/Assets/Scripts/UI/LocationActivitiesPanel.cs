using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the city location activities panel and its associated content views.
/// Handles button highlighting, panel visibility, and keyboard/gamepad shortcuts.
/// </summary>
public class LocationActivitiesPanel : MonoBehaviour
{
    private static readonly Color ActiveColor = new(1f, 0.92f, 0.016f, 1f); // Bright yellow
    private static readonly Color InactiveColor = new(0.6f, 0.0f, 0.0f, 1f); // Deep red

    [Serializable]
    private class LocationView
    {
        [Tooltip("Display name for debugging purposes only.")]
        public string displayName = string.Empty;
        public Button button = null!;
        public GameObject contentRoot = null!;
        [NonSerialized] public UnityAction cachedAction = null!;
    }

    private const string ButtonSuffix = "Btn";
    private const string BackgroundSuffix = "Background";

    [Header("Panel Root")]
    [SerializeField]
    private GameObject panelRoot = null!;

    [Header("Location Views")]
    [SerializeField]
    private List<LocationView> locations = new();

    [Header("Auto Discovery")]
    [SerializeField]
    [Tooltip("Automatically discover buttons/content under the configured container when explicit locations are not provided.")]
    private bool autoDiscoverLocations = true;

    [SerializeField]
    [Tooltip("Optional override for the container that holds the activity buttons (defaults to searching for 'infoFrame').")]
    private Transform buttonContainerOverride = null!;

    [Header("Visuals")]
    [SerializeField]
    private bool useButtonColorHighlights = true;

    [Header("Placeholders")]
    [SerializeField]
    [Tooltip("Format string used when auto-generating placeholder content for locations without bespoke panels.")]
    private string placeholderMessageFormat = "{0} content coming soon.";

    private LocationView activeView;
    private bool isOpen;

    private void Awake()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }

        if (autoDiscoverLocations)
        {
            DiscoverLocationsFromHierarchy();
        }

        var templateView = locations.FirstOrDefault(l => l.contentRoot != null);
        foreach (var location in locations)
        {
            if (location.button != null)
            {
                location.cachedAction = () => HandleLocationClicked(location);
                location.button.onClick.AddListener(location.cachedAction);
            }

            EnsureContentRoot(location, templateView?.contentRoot);
            location.contentRoot?.SetActive(false);
        }

        panelRoot.SetActive(false);
        isOpen = false;
    }

    private void OnDestroy()
    {
        foreach (var location in locations)
        {
            if (location.button != null)
            {
                if (location.cachedAction != null)
                {
                    location.button.onClick.RemoveListener(location.cachedAction);
                }
            }
        }
    }

    private void Update()
    {
        if (!isOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            Close();
        }
    }

    /// <summary>
    /// Displays the panel and highlights the previously selected location or the first available entry.
    /// </summary>
    public void Open()
    {
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(true);
        isOpen = true;

        if (activeView == null && locations.Count > 0)
        {
            SetActiveView(locations[0]);
        }
        else
        {
            ApplyVisualStates();
            ShowOnlyActiveContent();
        }

        if (EventSystem.current != null && activeView != null && activeView.button != null)
        {
            EventSystem.current.SetSelectedGameObject(activeView.button.gameObject);
        }
    }

    /// <summary>
    /// Hides the panel and resets AreaTooltip so the player can reopen it without leaving the trigger volume.
    /// </summary>
    public void Close()
    {
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(false);
        isOpen = false;
        HideAllContent();

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        var lastTooltip = AreaTooltip.LastActivatedTooltip;
        lastTooltip?.UnlockInteraction();
    }

    /// <summary>
    /// Makes the Tavern content visible.
    /// </summary>
    public void ShowTavern() => SelectLocationByName("Tavern");

    /// <summary>
    /// Makes the Shop content visible.
    /// </summary>
    public void ShowShop() => SelectLocationByName("Shop");

    /// <summary>
    /// Makes the Temple content visible.
    /// </summary>
    public void ShowTemple() => SelectLocationByName("Temple");

    /// <summary>
    /// Makes the Academy content visible.
    /// </summary>
    public void ShowAcademy() => SelectLocationByName("Academy");

    /// <summary>
    /// Makes the Arena content visible.
    /// </summary>
    public void ShowArena() => SelectLocationByName("Arena");

    /// <summary>
    /// Makes the Graveyard content visible.
    /// </summary>
    public void ShowGraveyard() => SelectLocationByName("Graveyard");

    private void HandleLocationClicked(LocationView view)
    {
        SetActiveView(view);
    }

    private void SelectLocationByName(string name)
    {
        var view = locations.Find(l => string.Equals(l.displayName, name, StringComparison.OrdinalIgnoreCase));
        if (view != null)
        {
            SetActiveView(view);
        }
    }

    private void DiscoverLocationsFromHierarchy()
    {
        if (locations.Any(l => l.button != null))
        {
            return;
        }

        var container = ResolveButtonContainer();
        if (container == null)
        {
            Debug.LogWarning($"[{nameof(LocationActivitiesPanel)}] Unable to find button container for auto discovery.", this);
            return;
        }

        locations.Clear();

        foreach (var button in container.GetComponentsInChildren<Button>(true))
        {
            if (button == null)
            {
                continue;
            }

            var baseName = ExtractBaseName(button.gameObject.name);
            var displayName = ExtractDisplayName(button, baseName);

            if (IsSearchForEnemiesButton(baseName, displayName))
            {
                WireSearchForEnemiesButton(button);
                continue;
            }

            if (string.IsNullOrEmpty(displayName))
            {
                continue;
            }

            var content = FindContentRoot(displayName, baseName);
            locations.Add(new LocationView
            {
                displayName = displayName,
                button = button,
                contentRoot = content
            });
        }
    }

    private Transform ResolveButtonContainer()
    {
        if (buttonContainerOverride != null)
        {
            return buttonContainerOverride;
        }

        if (panelRoot != null)
        {
            var found = FindChildRecursive(panelRoot.transform, "infoFrame");
            if (found != null)
            {
                buttonContainerOverride = found;
                return found;
            }
        }

        var infoFrame = GameObject.Find("infoFrame");
        if (infoFrame != null)
        {
            buttonContainerOverride = infoFrame.transform;
            return buttonContainerOverride;
        }

        return null;
    }

    private static string ExtractBaseName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return string.Empty;
        }

        return objectName.EndsWith(ButtonSuffix, StringComparison.OrdinalIgnoreCase)
            ? objectName[..^ButtonSuffix.Length]
            : objectName;
    }

    private static string ExtractDisplayName(Button button, string fallback)
    {
        var label = button.GetComponentInChildren<TMP_Text>();
        if (label != null && !string.IsNullOrWhiteSpace(label.text))
        {
            return label.text.Trim();
        }

        if (string.IsNullOrEmpty(fallback))
        {
            return string.Empty;
        }

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fallback.Replace('_', ' '));
    }

    private static bool IsSearchForEnemiesButton(string baseName, string displayName)
    {
        if (baseName.Equals("enemies", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return displayName.IndexOf("Enemies", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void WireSearchForEnemiesButton(Button button)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Close();
            MainRPGNavigation.OpenBattle();
        });
    }

    private GameObject FindContentRoot(string displayName, string baseName)
    {
        foreach (var candidate in BuildContentNameCandidates(displayName, baseName))
        {
            var found = TryFindContentObject(candidate);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private IEnumerable<string> BuildContentNameCandidates(string displayName, string baseName)
    {
        if (!string.IsNullOrEmpty(baseName))
        {
            yield return baseName + BackgroundSuffix;
            yield return baseName + "Panel";
        }

        if (!string.IsNullOrEmpty(displayName))
        {
            foreach (var variant in BuildNameVariants(displayName))
            {
                yield return variant + BackgroundSuffix;
                yield return variant + "Panel";
            }
        }
    }

    private static IEnumerable<string> BuildNameVariants(string source)
    {
        var trimmed = source.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            yield break;
        }

        var parts = trimmed.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            yield break;
        }

        var camel = string.Concat(parts.Select((part, index) => index == 0
            ? part.ToLowerInvariant()
            : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(part.ToLowerInvariant())));

        var pascal = string.Concat(parts.Select(part => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(part.ToLowerInvariant())));
        var lower = string.Concat(parts).ToLowerInvariant();

        yield return camel;
        if (!string.Equals(camel, pascal, StringComparison.Ordinal))
        {
            yield return pascal;
        }
        if (!string.Equals(lower, camel, StringComparison.Ordinal) && !string.Equals(lower, pascal, StringComparison.Ordinal))
        {
            yield return lower;
        }
    }

    private GameObject TryFindContentObject(string name)
    {
        if (panelRoot != null)
        {
            var found = FindChildRecursive(panelRoot.transform, name);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        if (panelRoot != null && panelRoot.transform.parent != null)
        {
            var found = FindChildRecursive(panelRoot.transform.parent, name);
            if (found != null)
            {
                return found.gameObject;
            }
        }

        var sceneObject = GameObject.Find(name);
        return sceneObject != null ? sceneObject : null;
    }

    private static Transform FindChildRecursive(Transform root, string name)
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root)
        {
            if (string.Equals(child.name, name, StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }

            var descendant = FindChildRecursive(child, name);
            if (descendant != null)
            {
                return descendant;
            }
        }

        return null;
    }

    private void SetActiveView(LocationView view)
    {
        if (view == null)
        {
            return;
        }

        activeView = view;
        ShowOnlyActiveContent();
        ApplyVisualStates();

        if (EventSystem.current != null && view.button != null)
        {
            EventSystem.current.SetSelectedGameObject(view.button.gameObject);
        }
    }

    private void ShowOnlyActiveContent()
    {
        foreach (var location in locations)
        {
            if (location.contentRoot != null)
            {
                location.contentRoot.SetActive(location == activeView);
            }
        }
    }

    private void HideAllContent()
    {
        foreach (var location in locations)
        {
            if (location.contentRoot != null)
            {
                location.contentRoot.SetActive(false);
            }
        }
    }

    private void ApplyVisualStates()
    {
        if (!useButtonColorHighlights)
        {
            return;
        }

        foreach (var location in locations)
        {
            if (location.button == null)
            {
                continue;
            }

            var targetGraphic = location.button.targetGraphic;
            if (targetGraphic == null)
            {
                continue;
            }

            targetGraphic.color = location == activeView ? ActiveColor : InactiveColor;
        }
    }

    private void EnsureContentRoot(LocationView location, GameObject templateRoot)
    {
        if (location.contentRoot != null)
        {
            return;
        }

        var placeholder = CreatePlaceholderContent(location.displayName, templateRoot);
        location.contentRoot = placeholder;
    }

    private GameObject CreatePlaceholderContent(string displayName, GameObject templateRoot)
    {
        var parent = templateRoot != null ? templateRoot.transform.parent : panelRoot.transform;
        var placeholder = new GameObject($"{displayName}Placeholder", typeof(RectTransform));
        var placeholderRect = placeholder.GetComponent<RectTransform>();
        placeholderRect.SetParent(parent, false);

        ApplyTemplateLayout(templateRoot, placeholderRect);

        var label = BuildPlaceholderLabel(placeholderRect, templateRoot, displayName);
        label.text = string.Format(placeholderMessageFormat, displayName);

        placeholder.SetActive(false);
        return placeholder;
    }

    private static void ApplyTemplateLayout(GameObject templateRoot, RectTransform target)
    {
        if (templateRoot != null && templateRoot.TryGetComponent(out RectTransform templateRect))
        {
            target.anchorMin = templateRect.anchorMin;
            target.anchorMax = templateRect.anchorMax;
            target.anchoredPosition = templateRect.anchoredPosition;
            target.sizeDelta = templateRect.sizeDelta;
            target.pivot = templateRect.pivot;
        }
        else
        {
            target.anchorMin = new Vector2(0.5f, 0.5f);
            target.anchorMax = new Vector2(0.5f, 0.5f);
            target.anchoredPosition = Vector2.zero;
            target.sizeDelta = new Vector2(600f, 400f);
            target.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private static TextMeshProUGUI BuildPlaceholderLabel(RectTransform parent, GameObject templateRoot, string displayName)
    {
        var labelGO = new GameObject($"{displayName}Label", typeof(RectTransform), typeof(CanvasRenderer));
        var labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.SetParent(parent, false);
        labelRect.anchorMin = new Vector2(0.1f, 0.1f);
        labelRect.anchorMax = new Vector2(0.9f, 0.9f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.alignment = TextAlignmentOptions.Center;
        label.enableWordWrapping = true;
        label.fontSize = 28f;

        if (templateRoot != null)
        {
            var templateLabel = templateRoot.GetComponentInChildren<TextMeshProUGUI>(true);
            if (templateLabel != null)
            {
                label.font = templateLabel.font;
                label.fontSize = templateLabel.fontSize;
                label.fontStyle = templateLabel.fontStyle;
                label.color = templateLabel.color;
            }
        }

        return label;
    }
}
