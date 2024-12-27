using System;
using com.unity3d.mediation;

namespace Shine.Ads
{
    public class LPBannerAd : Ads.IBannerAd
    {
        private LevelPlayBannerAd bannerAd;
        public Action OnLoaded = () => { };
        public bool isEnable = false;

        public LPBannerAd(string adUnitId)
        {
            bannerAd = new LevelPlayBannerAd(adUnitId);
            bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
            bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
            bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
            bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
            bannerAd.OnAdClicked += BannerOnAdClickedEvent;
            bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
            bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
            bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
            bannerAd.LoadAd();
        }

        private void BannerOnAdExpandedEvent(LevelPlayAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdCollapsedEvent(LevelPlayAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdClickedEvent(LevelPlayAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError error)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdDisplayedEvent(LevelPlayAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void BannerOnAdLoadFailedEvent(LevelPlayAdError error)
        {
            UnityEngine.Debug.LogError(error);
        }

        private void BannerOnAdLoadedEvent(LevelPlayAdInfo info)
        {
            isEnable = true;
            OnLoaded.Invoke();
            ShowBannerAd();
            UnityEngine.Debug.Log("On Loaded");
        }

        public void HideBannerAd()
        {
            if (!isEnable) return;
            bannerAd.HideAd();
        }

        public void ShowBannerAd()
        {
            if (!isEnable) return;
            bannerAd.ShowAd();
        }

        public void DestroyBannerAd()
        {
            if (!isEnable) return;
            bannerAd.DestroyAd();
        }

        public float GetHeight()
        {
            return bannerAd.GetAdSize().Height;
        }
    }
}