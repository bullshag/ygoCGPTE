using UnityEngine;

public class locationWindowHandler : MonoBehaviour
{
    private void OnDisable()
    {
        UnlockTooltipInteraction();
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

        UnlockTooltipInteraction();
        gameObject.SetActive(false);
    }

    private static void UnlockTooltipInteraction()
    {
        AreaTooltip.LastActivatedTooltip?.UnlockInteraction();
    }
}
