using System;
using com.unity3d.mediation;

public class LevelPlayAds : Ads
{
    public LevelPlayAds(LevelPlayAdsConfig config) : base(config)
    {
        LevelPlayAdFormat[] legacyAdFormats = new LevelPlayAdFormat[] { LevelPlayAdFormat.REWARDED };
        string userId = "";
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        LevelPlay.Init(adsConfig.appKey, userId, legacyAdFormats);
        IronSource.Agent.setMetaData("is_test_suite", "enable");
        IronSource.Agent.shouldTrackNetworkState(true);
    }

    public override void Init()
    {
        AdsConfig.AdsUnit adsUnit = this.adsConfig.androidAdsUnit;
        bannerAd = new LPBannerAd(adsUnit.banner);
        interstitialAd = new LPInterstitialAd(adsUnit.interstitial);
        rewardedAd = new LPRewardedAd(adsUnit.rewarded);
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        UnityEngine.Debug.Log("Init level play ads Failed!" + error);
    }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {
        UnityEngine.Debug.Log("Init level play ads successfully!");
        Init();
    }

    public override void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }
}