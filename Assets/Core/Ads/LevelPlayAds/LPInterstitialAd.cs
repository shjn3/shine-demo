using System;
using System.Diagnostics;
using com.unity3d.mediation;

public class LPInterstitialAd : Ads.IInterstitialAd
{
    public Action onceAdDisplaceFailed = () => { };
    public Action onceAdDisplaced = () => { };

    LevelPlayInterstitialAd interstitialAd;
    string adUnitId;
    public LPInterstitialAd(string adUnitId)
    {
        this.adUnitId = adUnitId;
    }

    private void LoadInterstitialAd()
    {
        interstitialAd = new LevelPlayInterstitialAd(adUnitId);
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
        interstitialAd.LoadAd();
    }

    private void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo info)
    {
        UnityEngine.Debug.Log("");
    }

    private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        UnityEngine.Debug.Log("");
    }

    private void InterstitialOnAdClickedEvent(LevelPlayAdInfo info)
    {
        UnityEngine.Debug.Log("");
    }

    private void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError error)
    {
        UnityEngine.Debug.Log("");
        onceAdDisplaceFailed.Invoke();
        onceAdDisplaceFailed = () => { };
    }

    private void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo info)
    {
        UnityEngine.Debug.Log("");
        onceAdDisplaced.Invoke();
        onceAdDisplaced = () => { };
    }

    private void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        UnityEngine.Debug.Log("Interstitial ad load failed!");
        onceLoadedAd = () => { };
    }

    private void InterstitialOnAdLoadedEvent(LevelPlayAdInfo info)
    {
        UnityEngine.Debug.Log("Interstitial ad loaded!");
        onceLoadedAd();
        onceLoadedAd = () => { };
    }
    public Action onceLoadedAd = () => { };
    public void ShowInterstitialAd(Action<bool> callback)
    {
        onceAdDisplaced = () => { };
        onceAdDisplaced += () => { callback.Invoke(true); };
        onceAdDisplaceFailed = () => { };
        onceAdDisplaceFailed += () => { callback.Invoke(false); };

        if (interstitialAd == null || !interstitialAd.IsAdReady())
        {
            onceLoadedAd += () =>
            {
                if (interstitialAd.IsAdReady())
                {
                    interstitialAd.ShowAd();
                }
                else
                {
                    callback.Invoke(false);
                }
            };
            LoadInterstitialAd();
            return;
        }

        interstitialAd.ShowAd();
    }
}