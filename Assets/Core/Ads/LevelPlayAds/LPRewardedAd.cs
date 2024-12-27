using System;
namespace Shine.Ads
{
    public class LPRewardedAd : Ads.IRewardedAd
    {
        public Action onceRewarded = () => { };
        public Action onceRewardFailed = () => { };

        string adUnitId;
        public LPRewardedAd(string adUnitId)
        {
            this.adUnitId = adUnitId;

            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        }

        private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
        {
            this.onceRewarded();
            this.onceRewarded = () => { };
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
        {
            this.onceRewardFailed();
            this.onceRewardFailed = () => { };
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdUnavailable()
        {
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdAvailable(IronSourceAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }

        private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo info)
        {
            UnityEngine.Debug.Log("");
        }


        public void ShowRewardedAd(Action<bool> callback)
        {
            bool available = IronSource.Agent.isRewardedVideoAvailable();
            onceRewarded += () =>
            {
                callback.Invoke(true);
            };
            onceRewardFailed += () =>
            {
                callback.Invoke(false);
            };

            if (!available)
            {
                UnityEngine.Debug.Log("Don't have video ad");
                callback.Invoke(false);
                return;
            }

            IronSource.Agent.showRewardedVideo();
        }
    }
}
