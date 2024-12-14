
using System;
using UnityEditor;
using UnityEngine;

public class AdsManager : MonoBehaviour
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
        ads.bannerAd.ShowBannerAd();

        CreateListener();
    }

    public void CreateListener()
    {
        var go = new GameObject("AdsManagerListener");
        GameObject.DontDestroyOnLoad(go);
        listener = go.AddComponent<AdsManagerListener>();
        listener.onApplicationPauseCallback += OnApplicationPause;
    }

    public static void Init(Ads.AdsConfig config)
    {
        if (instance != null)
        {
            return;
        }
        instance = new AdsManager(config);
    }

    public static void HideBannerAd()
    {
        if (instance == null) return;
        if (!instance.enable) return;
        instance.ads.bannerAd.HideBannerAd();
    }

    public static void ShowBannerAd()
    {
        if (instance == null) return;
        if (!instance.enable) return;
        instance.ads.bannerAd.ShowBannerAd();
    }

    public static void DestroyBannerAd()
    {
        if (instance == null) return;
        if (!instance.enable) return;
        instance.ads.bannerAd.DestroyBannerAd();
    }

    public static void ShowInterstitialAd(Action<bool> callback)
    {
        if (instance == null)
        {
            ShowNotifyScreen("Load Interstitial ad failed!", 3);
            callback.Invoke(false);
            return;
        }

        if (!instance.enable)
        {
            callback.Invoke(false);
            return;
        }

        instance.ads.interstitialAd.ShowInterstitialAd(callback);
    }

    public static void ShowRewardedAd(Action<bool> callback)
    {
        if (instance == null)
        {
            ShowNotifyScreen("Load rewarded failed!", 3);
            callback.Invoke(false);
            return;
        }

        if (!instance.enable)
        {
            callback.Invoke(false);
            return;
        }
        instance.ads.rewardedAd.ShowRewardedAd(callback);
    }

    public void OnApplicationPause(bool isPaused)
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
}
