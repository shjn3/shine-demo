using System;
namespace Shine.Ads
{
    public abstract class Ads
    {
        public AdsConfig adsConfig;
        protected IBannerAd bannerAd;
        protected IInterstitialAd interstitialAd;
        protected IRewardedAd rewardedAd;

        public Ads(AdsConfig config)
        {
            adsConfig = config;
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

        public abstract void ShowRewardedAd(Action<bool> callback);

        public abstract void ShowInterstitialAd(Action<bool> callback);

        public virtual void HideBannerAd()
        {
            //
        }

        public virtual void ShowBannerAd()
        {
            //
        }

        public virtual void DestroyBannerAd()
        {
            //
        }

    }
}
