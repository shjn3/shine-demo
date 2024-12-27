using UnityEngine;

public class GameSceneConfig
{
    public const float WIDTH = 375;
    public const float HEIGHT = 667;

    public static float GetBannerHeight(float height = 50)
    {
        return Mathf.RoundToInt(height * Screen.dpi / 160f) * (GameSceneConfig.HEIGHT / Screen.height);
    }
}