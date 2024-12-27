using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Shine.Ads
{
    public class MockBannerAd : Ads.IBannerAd
    {
        public Image bannerImage;
        public MockBannerAd()
        {
#if UNITY_EDITOR
            var a = AssetDatabase.FindAssets("MockBanner");
            if (a != null && a.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(a[0]);
                GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                GameObject.Instantiate(go);
                bannerImage = go.GetComponentInChildren<Image>();
            }
#endif
        }
        public void DestroyBannerAd()
        {
            UnityEngine.Debug.Log("");
        }

        public float GetHeight()
        {
            if (bannerImage == null)
            {
                return 0;
            }
            UnityEngine.Debug.Log("");
            return bannerImage.rectTransform.sizeDelta.y;
        }

        public void HideBannerAd()
        {
            UnityEngine.Debug.Log("");
        }

        public void ShowBannerAd()
        {
            UnityEngine.Debug.Log("");
        }
    }
}