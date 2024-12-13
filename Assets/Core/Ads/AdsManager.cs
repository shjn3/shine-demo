
using System;
using UnityEditor;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;
    public Ads.AdsConfig adsConfig;
    public Ads ads;
    bool enable = false;

    public void Init(Ads.AdsConfig config)
    {
        if (instance != null)
        {
            return;
        }
        instance = this;


        enable = true;
        adsConfig = config;
        ads = adsConfig switch
        {
            LevelPlayAdsConfig => new LevelPlayAds((LevelPlayAdsConfig)config),
            _ => new MockAds(config),
        };
        ads.bannerAd.ShowBannerAd();
    }

    public void HideBannerAd()
    {
        if (!enable) return;
        ads.bannerAd.HideBannerAd();
    }

    public void ShowBannerAd()
    {
        if (!enable) return;
        ads.bannerAd.ShowBannerAd();
    }

    public void DestroyBannerAd()
    {
        if (!enable) return;
        ads.bannerAd.DestroyBannerAd();

    }

    public float GetBannerHeight()
    {
        return ads.bannerAd.GetHeight();
    }

    public static void ShowInterstitialAd(Action<bool> callback)
    {
        if (instance == null)
        {
            ShowNotify("Load Interstitial ad failed!", 3);
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
            ShowNotify("Load rewarded failed!", 3);
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

    static void ShowNotify(string message, float duration = -1, bool isShowLoading = false)
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
