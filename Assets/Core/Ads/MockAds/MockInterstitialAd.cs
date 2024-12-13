using System;
using System.Diagnostics;

public class MockInterstitialAd : Ads.IInterstitialAd
{
    //
    public void ShowInterstitialAd(Action<bool> callback)
    {
        AdsScreen screen = ScreenManager.GetScreen<AdsScreen>();
        screen.onceClose += () =>
        {
            callback.Invoke(true);
        };
        screen.PassData("Interstitial Ad");
        ScreenManager.OpenScreen(screen.screenName);
        UnityEngine.Debug.Log("");
    }
}