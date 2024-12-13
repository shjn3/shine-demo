public class MockAds : Ads
{
    //
    public MockAds(AdsConfig config) : base(config)
    {

    }

    public override void Init()
    {
        bannerAd = new MockBannerAd();
        interstitialAd = new MockInterstitialAd();
        rewardedAd = new MockRewardedAd();
    }
}