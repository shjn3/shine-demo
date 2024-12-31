using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Shine
{
    using Ads;
    using FireB;
    using Utils;
    public class Core : MonoBehaviour
    {
        public static Core instance;
        public static Config.Config configs;
        public AdsManager adsManager;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }


        }
        public static void Init()
        {
            ShineFireBase.Init();
            GameObject go = new GameObject("Sleeper");
            go.AddComponent<Sleeper>();
            TextAsset configAsset = Resources.Load<TextAsset>("configs/config.default");
            configs = JsonConvert.DeserializeObject<Config.Config>(configAsset.text);
            AdsManager.Init(configs.mockAdsConfig);
            FPSManager.Init();
        }
    }
}