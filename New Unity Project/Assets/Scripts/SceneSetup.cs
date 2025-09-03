using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ensures that every scene has a Main Camera and EventSystem. Any Canvas
/// components present in the scene will reference the main camera when
/// required.
/// </summary>
public static class SceneSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSceneObjects()
    {
        EnsureCamera();
        EnsureEventSystem();
        ConfigureCanvases();
    }

    private static void EnsureCamera()
    {
        if (Camera.main != null)
            return;

        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.AddComponent<Camera>();
        camGO.AddComponent<AudioListener>();
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
            return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static void ConfigureCanvases()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        var canvases = Object.FindObjectsOfType<Canvas>();
        if (canvases.Length == 0)
        {
            var canvasGO = GameObject.Find("Canvas") ?? new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera;
            canvases = new[] { canvas };
        }

        foreach (var canvas in canvases)
        {
            if ((canvas.renderMode == RenderMode.ScreenSpaceCamera ||
                 canvas.renderMode == RenderMode.WorldSpace) &&
                canvas.worldCamera == null)
            {
                canvas.worldCamera = mainCamera;
            }
        }
    }
}

