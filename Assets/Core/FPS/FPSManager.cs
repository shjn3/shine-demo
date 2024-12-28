using UnityEngine;

public class FPSManager
{
    public static void Init()
    {
        GameObject go = new GameObject("FPS Display");
        go.AddComponent<FPSDisplay>();

        SceneTransition.onLoadedScene += () =>
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        };
    }
}