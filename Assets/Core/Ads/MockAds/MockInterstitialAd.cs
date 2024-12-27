using System;
namespace Shine.Ads
{
    public class MockInterstitialAd : Ads.IInterstitialAd
    {
        //
        public void ShowInterstitialAd(Action<bool> callback)
        {
            UnityEngine.Debug.Log("call");
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
}