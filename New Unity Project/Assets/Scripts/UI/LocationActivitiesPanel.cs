using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Drives the location activity buttons, loading availability from the database
/// and managing highlight states.
/// </summary>
public class LocationActivitiesPanel : MonoBehaviour
{
    [Serializable]
    private class ActivityButton
    {
        public LocationActivityType activityType;
        public Button button = null!;
        [NonSerialized] public UnityAction? cachedHandler;
    }

    private static readonly Color BaseColor = ParseColor("#FF4949", new Color(1f, 0.286f, 0.286f, 1f));
    private static readonly Color ActiveColor = new(1f, 0.92f, 0.016f, 1f);

    [Header("Location Context")]
    [SerializeField]
    private string locationId = string.Empty;

    [Tooltip("Hide buttons that are unavailable instead of leaving them disabled.")]
    [SerializeField]
    private bool hideUnavailableButtons = true;

    [Tooltip("Seconds that the Search button remains highlighted before returning to red.")]
    [SerializeField]
    private float searchHighlightDuration = 0.35f;

    [Header("Buttons")]
    [SerializeField]
    private List<ActivityButton> activityButtons = new();

    [Header("Debug")]
    [Tooltip("When enabled, periodically refreshes availability while the panel is active.")]
    [SerializeField]
    private bool debugAutoRefresh = false;

    [SerializeField]
    [Min(0.5f)]
    private float debugRefreshIntervalSeconds = 3f;

    private readonly LocationActivityService _service = new();
    private readonly Dictionary<LocationActivityType, ActivityButton> _lookup = new();

    private ActivityButton? _activeSelection;
    private CancellationTokenSource? _refreshCts;
    private Coroutine? _searchRoutine;

    /// <summary>
    /// Currently selected activity (excluding Search).
    /// </summary>
    public LocationActivityType? ActiveActivity => _activeSelection?.activityType;

    /// <summary>
    /// Raised whenever the active activity selection changes. A <c>null</c> value indicates that
    /// no contextual activity is currently selected (for example, after clearing the selection or
    /// clicking the Search option).
    /// </summary>
    public event Action<LocationActivityType?> ActivitySelectionChanged;

    private void Awake()
    {
        _lookup.Clear();
        foreach (var entry in activityButtons)
        {
            if (entry?.button == null)
            {
                continue;
            }

            if (_lookup.ContainsKey(entry.activityType))
            {
                Debug.LogWarning($"Duplicate activity mapping for {entry.activityType} detected on {name}. Only the first mapping will be used.");
                continue;
            }

            _lookup.Add(entry.activityType, entry);
            ConfigureButtonColors(entry.button, BaseColor);

            entry.cachedHandler = () => HandleButtonClicked(entry.activityType);
            entry.button.onClick.AddListener(entry.cachedHandler);
        }
    }

    private void OnEnable()
    {
        RequestRefresh();
        StartDebugAutoRefresh();
    }

    private void OnDisable()
    {
        CancelPendingRefresh();
        StopDebugAutoRefresh();
        ClearSelection();
    }

    private void OnValidate()
    {
        debugRefreshIntervalSeconds = Mathf.Max(0.5f, debugRefreshIntervalSeconds);

        if (Application.isPlaying && isActiveAndEnabled)
        {
            StartDebugAutoRefresh();
        }
    }

    private void OnDestroy()
    {
        foreach (var entry in activityButtons)
        {
            if (entry?.button != null && entry.cachedHandler != null)
            {
                entry.button.onClick.RemoveListener(entry.cachedHandler);
            }
        }
    }

    /// <summary>
    /// Assign a new location identifier and refresh the availability data.
    /// </summary>
    public void SetLocation(string newLocationId)
    {
        newLocationId ??= string.Empty;
        bool locationChanged = !string.Equals(locationId, newLocationId, StringComparison.OrdinalIgnoreCase);

        if (locationChanged)
        {
            locationId = newLocationId;
        }

        _ = RefreshAvailabilityAsync();
    }

    /// <summary>
    /// Clears any active selection and resets button colors to their base state.
    /// </summary>
    public void ClearSelection()
    {
        _activeSelection = null;
        StopSearchRoutine();

        foreach (var entry in activityButtons)
        {
            if (entry?.button == null)
            {
                continue;
            }

            ConfigureButtonColors(entry.button, BaseColor);
        }

        NotifyActivitySelectionChanged(null);
    }

    /// <summary>
    /// Force a refresh of the availability data using the configured location identifier.
    /// </summary>
    public async Task RefreshAvailabilityAsync()
    {
        CancelPendingRefresh();

        if (string.IsNullOrWhiteSpace(locationId))
        {
            Debug.LogWarning($"{nameof(LocationActivitiesPanel)} on {name} cannot refresh without a location identifier.");
            ApplyAvailability(new LocationActivityAvailability());
            return;
        }

        _refreshCts = new CancellationTokenSource();
        var token = _refreshCts.Token;

        try
        {
            var availability = await _service.GetAvailabilityAsync(locationId, token).ConfigureAwait(true);
            ApplyAvailability(availability);
        }
        catch (OperationCanceledException)
        {
            // Intentionally ignored when the panel is disabled or destroyed.
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load location activities for '{locationId}': {ex.Message}");
            ApplyAvailability(new LocationActivityAvailability());
        }
    }

    private void RequestRefresh()
    {
        _ = RefreshAvailabilityAsync();
    }

    private void StartDebugAutoRefresh()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (!debugAutoRefresh)
        {
            CancelInvoke(nameof(RequestRefresh));
            return;
        }

        float interval = Mathf.Max(0.5f, debugRefreshIntervalSeconds);
        CancelInvoke(nameof(RequestRefresh));
        InvokeRepeating(nameof(RequestRefresh), interval, interval);
    }

    private void StopDebugAutoRefresh()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        CancelInvoke(nameof(RequestRefresh));
    }

    private void CancelPendingRefresh()
    {
        if (_refreshCts == null)
        {
            return;
        }

        if (!_refreshCts.IsCancellationRequested)
        {
            _refreshCts.Cancel();
        }

        _refreshCts.Dispose();
        _refreshCts = null;
    }

    private void ApplyAvailability(LocationActivityAvailability availability)
    {
        bool selectionCleared = false;

        foreach (var entry in activityButtons)
        {
            if (entry?.button == null)
            {
                continue;
            }

            bool isEnabled = availability.IsEnabled(entry.activityType);

            entry.button.interactable = isEnabled;

            if (hideUnavailableButtons)
            {
                entry.button.gameObject.SetActive(isEnabled);
            }

            if (!isEnabled && _activeSelection == entry)
            {
                _activeSelection = null;
                selectionCleared = true;
            }

            if (!isEnabled)
            {
                ConfigureButtonColors(entry.button, BaseColor);
            }
        }

        if (_activeSelection != null && _activeSelection.button != null)
        {
            ConfigureButtonColors(_activeSelection.button, ActiveColor);
        }
        else
        {
            ResetNonSearchButtonsToBase();
        }

        if (selectionCleared)
        {
            NotifyActivitySelectionChanged(null);
        }
    }

    private void HandleButtonClicked(LocationActivityType activityType)
    {
        if (!_lookup.TryGetValue(activityType, out var entry) || entry.button == null)
        {
            return;
        }

        if (activityType == LocationActivityType.SearchForEnemies)
        {
            HandleSearchButton(entry);
            return;
        }

        _activeSelection = entry;
        ResetNonSearchButtonsToBase();
        ConfigureButtonColors(entry.button, ActiveColor);
        NotifyActivitySelectionChanged(activityType);
    }

    private void HandleSearchButton(ActivityButton entry)
    {
        ResetNonSearchButtonsToBase();

        StopSearchRoutine();

        ConfigureButtonColors(entry.button, ActiveColor);
        _searchRoutine = StartCoroutine(ResetSearchAfterDelay(entry));
        NotifyActivitySelectionChanged(null);
    }

    private IEnumerator ResetSearchAfterDelay(ActivityButton entry)
    {
        yield return new WaitForSeconds(Mathf.Max(0.05f, searchHighlightDuration));

        if (entry.button != null)
        {
            ConfigureButtonColors(entry.button, BaseColor);
        }

        if (_activeSelection != null && _activeSelection.button != null)
        {
            ConfigureButtonColors(_activeSelection.button, ActiveColor);
        }

        _searchRoutine = null;
    }

    private void ResetNonSearchButtonsToBase()
    {
        foreach (var entry in activityButtons)
        {
            if (entry?.button == null)
            {
                continue;
            }

            if (entry.activityType == LocationActivityType.SearchForEnemies)
            {
                continue;
            }

            ConfigureButtonColors(entry.button, BaseColor);
        }
    }

    private void NotifyActivitySelectionChanged(LocationActivityType? activityType)
    {
        ActivitySelectionChanged?.Invoke(activityType);
    }

    private static void ConfigureButtonColors(Button button, Color color)
    {
        if (button == null)
        {
            return;
        }

        if (button.targetGraphic != null)
        {
            button.targetGraphic.color = color;
        }

        var colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.disabledColor = new Color(color.r * 0.6f, color.g * 0.6f, color.b * 0.6f, color.a);
        colors.colorMultiplier = 1f;
        button.colors = colors;
    }

    private void StopSearchRoutine()
    {
        if (_searchRoutine == null)
        {
            return;
        }

        StopCoroutine(_searchRoutine);
        _searchRoutine = null;
    }

    private static Color ParseColor(string hex, Color fallback)
    {
        return ColorUtility.TryParseHtmlString(hex, out var parsed) ? parsed : fallback;
    }
}
