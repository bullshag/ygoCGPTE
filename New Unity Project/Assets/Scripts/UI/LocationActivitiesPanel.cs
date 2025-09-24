using System;
using System.Collections.Generic;
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

    [Header("Panel Root")]
    [SerializeField]
    private GameObject panelRoot = null!;

    [Header("Location Views")]
    [SerializeField]
    private List<LocationView> locations = new();

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
