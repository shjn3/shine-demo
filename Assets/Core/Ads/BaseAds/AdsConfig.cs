public abstract partial class Ads
{
    public class AdsConfig
    {
        public struct AdsUnit
        {
            public string banner;
            public string interstitial;
            public string rewarded;
        }
        public AdsUnit androidAdsUnit;
        public AdsUnit iosAdsUnit;
        public string appKey;
    }
}