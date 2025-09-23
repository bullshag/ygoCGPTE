using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

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

    private LocationView activeView;
    private bool isOpen;

    private void Awake()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }

        foreach (var location in locations)
        {
            if (location.button != null)
            {
                location.cachedAction = () => HandleLocationClicked(location);
                location.button.onClick.AddListener(location.cachedAction);
            }

            if (location.contentRoot != null)
            {
                location.contentRoot.SetActive(false);
            }
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
        foreach (var location in locations)
        {
            if (location.button == null)
            {
                continue;
            }

            var targetColor = location == activeView ? ActiveColor : InactiveColor;

            var targetGraphic = location.button.targetGraphic;
            if (targetGraphic != null)
            {
                targetGraphic.color = targetColor;
            }

            var colors = location.button.colors;
            colors.normalColor = targetColor;
            colors.highlightedColor = targetColor;
            colors.selectedColor = targetColor;
            colors.pressedColor = targetColor;
            colors.disabledColor = new Color(targetColor.r, targetColor.g, targetColor.b, colors.disabledColor.a);
            location.button.colors = colors;
        }
    }
}
