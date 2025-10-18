using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class locationWindowHandler : MonoBehaviour
{
    [Serializable]
    private sealed class ActivityWindow
    {
        public LocationActivityType activityType;
        public GameObject window = null!;
    }

    [Tooltip("Optional explicit reference to the GraphicRaycaster responsible for this window.")]
    [SerializeField]
    private GraphicRaycaster graphicRaycaster;

    [Tooltip("Optional explicit reference to the location activities panel shown within this window.")]
    [SerializeField]
    private LocationActivitiesPanel locationActivitiesPanel;

    [Header("Activity Windows")]
    [Tooltip("List of contextual windows that should toggle when corresponding activities are selected.")]
    [SerializeField]
    private List<ActivityWindow> activityWindows = new();

    private readonly List<RaycastResult> raycastResults = new();
    private readonly Dictionary<LocationActivityType, GameObject> windowLookup = new();
    private Button closeButton;
    private bool isSubscribedToSelection;

    private void Awake()
    {
        CacheCloseButton();
        CacheGraphicRaycaster();
        CacheLocationActivitiesPanel();
        BuildWindowLookup();
        HideAllActivityWindows();
    }

    private void OnEnable()
    {
        CacheLocationActivitiesPanel();
        BuildWindowLookup();
        HideAllActivityWindows();
    }

    private void OnDisable()
    {
        UnlockTooltipInteraction();
        CacheLocationActivitiesPanel();
        locationActivitiesPanel?.ClearSelection();
        HideAllActivityWindows();
    }

    private void OnDestroy()
    {
        UnsubscribeFromSelectionEvents();
    }

    public void hideLocationActivityWindow()
    {
        HidePanel();
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryHandleRaycastClose();
        }

        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.Backspace) ||
            Input.GetKeyDown(KeyCode.JoystickButton1) ||
            Input.GetButtonDown("Cancel"))
        {
            HidePanel();
        }
    }

    private void HidePanel()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        CacheLocationActivitiesPanel();
        locationActivitiesPanel?.ClearSelection();
        UnlockTooltipInteraction();
        gameObject.SetActive(false);
    }

    private static void UnlockTooltipInteraction()
    {
        AreaTooltip.LastActivatedTooltip?.UnlockInteraction();
    }

    private void TryHandleRaycastClose()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        CacheCloseButton();
        if (closeButton == null)
        {
            return;
        }

        CacheGraphicRaycaster();
        var raycaster = graphicRaycaster;
        if (raycaster == null)
        {
            return;
        }

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        raycastResults.Clear();
        raycaster.Raycast(pointerData, raycastResults);

        for (var i = 0; i < raycastResults.Count; i++)
        {
            var result = raycastResults[i];
            if (IsCloseButtonHit(result.gameObject))
            {
                HidePanel();
                return;
            }
        }
    }

    private bool IsCloseButtonHit(GameObject hitObject)
    {
        if (hitObject == null || closeButton == null)
        {
            return false;
        }

        return hitObject == closeButton.gameObject || hitObject.transform.IsChildOf(closeButton.transform);
    }

    private void CacheCloseButton()
    {
        if (closeButton != null)
        {
            return;
        }

        foreach (var button in GetComponentsInChildren<Button>(true))
        {
            var persistentCallCount = button.onClick.GetPersistentEventCount();
            for (var i = 0; i < persistentCallCount; i++)
            {
                if (button.onClick.GetPersistentTarget(i) == this &&
                    button.onClick.GetPersistentMethodName(i) == nameof(hideLocationActivityWindow))
                {
                    closeButton = button;
                    return;
                }
            }
        }
    }

    private void CacheGraphicRaycaster()
    {
        if (graphicRaycaster != null)
        {
            return;
        }

        graphicRaycaster = GetComponentInParent<GraphicRaycaster>(true);
    }

    private void CacheLocationActivitiesPanel()
    {
        if (locationActivitiesPanel == null)
        {
            locationActivitiesPanel = GetComponentInChildren<LocationActivitiesPanel>(true);
        }

        if (locationActivitiesPanel != null && !isSubscribedToSelection)
        {
            locationActivitiesPanel.ActivitySelectionChanged += HandleActivitySelectionChanged;
            isSubscribedToSelection = true;
        }
    }

    private void BuildWindowLookup()
    {
        windowLookup.Clear();

        if (activityWindows == null)
        {
            return;
        }

        foreach (var entry in activityWindows)
        {
            if (entry == null || entry.window == null)
            {
                continue;
            }

            if (windowLookup.ContainsKey(entry.activityType))
            {
                Debug.LogWarning($"Duplicate activity window mapping for {entry.activityType} on {name}. Only the first mapping will be used.");
                continue;
            }

            windowLookup.Add(entry.activityType, entry.window);
        }
    }

    private void HideAllActivityWindows()
    {
        if (activityWindows == null)
        {
            return;
        }

        foreach (var entry in activityWindows)
        {
            if (entry?.window == null)
            {
                continue;
            }

            if (entry.window.activeSelf)
            {
                entry.window.SetActive(false);
            }
        }
    }

    private void HandleActivitySelectionChanged(LocationActivityType? activityType)
    {
        HideAllActivityWindows();

        if (!activityType.HasValue)
        {
            return;
        }

        if (windowLookup.TryGetValue(activityType.Value, out var window) && window != null)
        {
            window.SetActive(true);
        }
    }

    private void UnsubscribeFromSelectionEvents()
    {
        if (locationActivitiesPanel != null && isSubscribedToSelection)
        {
            locationActivitiesPanel.ActivitySelectionChanged -= HandleActivitySelectionChanged;
            isSubscribedToSelection = false;
        }
    }

}
