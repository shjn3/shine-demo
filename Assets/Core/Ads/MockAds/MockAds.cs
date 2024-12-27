using System;

public class MockAds : Ads
{
    //
    public MockAds(AdsConfig config) : base(config)
    {
        Init();
    }

    public override void Init()
    {
        bannerAd = new MockBannerAd();
        interstitialAd = new MockInterstitialAd();
        rewardedAd = new MockRewardedAd();
    }

    public override void ShowRewardedAd(Action<bool> callback)
    {
        rewardedAd.ShowRewardedAd(callback);
    }

    public override void ShowInterstitialAd(Action<bool> callback)
    {
        interstitialAd.ShowInterstitialAd(callback);
    }
}