using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class locationWindowHandler : MonoBehaviour
{
    [Tooltip("Optional explicit reference to the GraphicRaycaster responsible for this window.")]
    [SerializeField]
    private GraphicRaycaster graphicRaycaster;

    [Tooltip("Optional explicit reference to the location activities panel shown within this window.")]
    [SerializeField]
    private LocationActivitiesPanel locationActivitiesPanel;

    private readonly List<RaycastResult> raycastResults = new();
    private Button closeButton;

    private void Awake()
    {
        CacheCloseButton();
        CacheGraphicRaycaster();
        CacheLocationActivitiesPanel();
    }

    private void OnDisable()
    {
        UnlockTooltipInteraction();
        CacheLocationActivitiesPanel();
        locationActivitiesPanel?.ClearSelection();
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
        if (locationActivitiesPanel != null)
        {
            return;
        }

        locationActivitiesPanel = GetComponentInChildren<LocationActivitiesPanel>(true);
    }

}
