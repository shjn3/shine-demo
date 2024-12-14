
using System;
using UnityEditor;
using UnityEngine;

public class AdsManager
{
    public static AdsManager instance;
    public Ads.AdsConfig adsConfig;
    public Ads ads;
    bool enable = false;
    private AdsManagerListener listener;

    public AdsManager(Ads.AdsConfig config)
    {
        enable = true;
        adsConfig = config;
        ads = adsConfig switch
        {
            LevelPlayAdsConfig => new LevelPlayAds((LevelPlayAdsConfig)config),
            _ => new MockAds(config),
        };


        CreateListener();
    }

    private void CreateListener()
    {
        var go = new GameObject("AdsManagerListener");
        GameObject.DontDestroyOnLoad(go);
        listener = go.AddComponent<AdsManagerListener>();
        listener.onApplicationPauseCallback += OnApplicationPause;
    }

    public static void Init(Ads.AdsConfig config)
    {
        Debug.Log("ads Init");
        instance = new AdsManager(config);
    }

    public static void ShowInterstitialAd(Action<bool> callback)
    {
        if (!HasAds())
        {
            ShowNotifyScreen("Load Interstitial ad failed!", 3);
            callback.Invoke(false);
            return;
        }

        instance.ads.ShowInterstitialAd(callback);
    }

    public static void ShowRewardedAd(Action<bool> callback)
    {
        Debug.Log("Instance: " + instance);
        if (!HasAds())
        {
            ShowNotifyScreen("Load rewarded failed!", 3);
            callback.Invoke(false);
            return;
        }

        instance.ads.ShowRewardedAd(callback);
    }

    void OnApplicationPause(bool isPaused)
    {
        ads?.OnApplicationPause(isPaused);
    }

    public static void ShowNotifyScreen(string message, float duration = -1, bool isShowLoading = false)
    {
        NotifyScreen screen = ScreenManager.GetScreen<NotifyScreen>();
        if (screen == null)
        {
            Debug.Log("screen null");
            return;
        }
        screen.PassData(message, duration, isShowLoading);
        ScreenManager.OpenScreen(screen.screenName);
    }

    private static bool HasAds()
    {
        return !(instance == null || instance.ads == null || !instance.enable);
    }
}
