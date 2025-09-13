using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Button okButton;

    public void Show(string message, Action onOk = null, Action onYes = null, Action onNo = null)
    {
        if (messageText != null)
            messageText.text = message;

        bool showYesNo = onYes != null || onNo != null;

        if (yesButton != null)
        {
            yesButton.gameObject.SetActive(showYesNo);
            yesButton.onClick.RemoveAllListeners();
            if (onYes != null)
                yesButton.onClick.AddListener(() => { onYes(); Destroy(gameObject); });
        }

        if (noButton != null)
        {
            noButton.gameObject.SetActive(showYesNo);
            noButton.onClick.RemoveAllListeners();
            if (onNo != null)
                noButton.onClick.AddListener(() => { onNo(); Destroy(gameObject); });
        }

        if (okButton != null)
        {
            okButton.gameObject.SetActive(!showYesNo);
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(() => { onOk?.Invoke(); Destroy(gameObject); });
        }
    }
}
