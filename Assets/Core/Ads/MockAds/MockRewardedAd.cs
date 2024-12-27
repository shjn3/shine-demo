using System;
using System.Diagnostics;

public class MockRewardedAd : Ads.IRewardedAd
{
    public void ShowRewardedAd(Action<bool> callback)
    {
        AdsScreen screen = ScreenManager.GetScreen<AdsScreen>();
        screen.onceClose += () =>
        {
            callback.Invoke(true);
        };
        screen.PassData("Rewarded Ad");
        ScreenManager.OpenScreen(screen.screenName);
        UnityEngine.Debug.Log("");
    }
}