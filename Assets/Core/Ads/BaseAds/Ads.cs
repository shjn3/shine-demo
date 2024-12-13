using System;

public partial class Ads
{
    public AdsConfig adsConfig;
    public IBannerAd bannerAd;
    public IInterstitialAd interstitialAd;
    public IRewardedAd rewardedAd;

    public Ads(AdsConfig config)
    {
        adsConfig = config;
        Init();
    }

    public virtual void Init()
    {
        //
    }

    public interface IBannerAd
    {
        public void HideBannerAd();
        public void ShowBannerAd();
        public void DestroyBannerAd();
        public float GetHeight();
    }

    public interface IInterstitialAd
    {
        public void ShowInterstitialAd(Action<bool> callback);
    }

    public interface IRewardedAd
    {
        public void ShowRewardedAd(Action<bool> callback);
    }

    public virtual void OnApplicationPause(bool isPaused)
    {
        //
    }
}
