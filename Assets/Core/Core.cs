using Newtonsoft.Json;
using UnityEngine;

public class Core : MonoBehaviour
{
    public static Core instance;
    public Configs configs;
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

        Init();
    }

    public void Init()
    {
        InitConfigs();
        adsManager.Init(configs.mockAdsConfig);
    }

    public static void InitConfigs()
    {
        TextAsset configAsset = Resources.Load<TextAsset>("configs/config.default");
        instance.configs = JsonConvert.DeserializeObject<Configs>(configAsset.text);
    }
}