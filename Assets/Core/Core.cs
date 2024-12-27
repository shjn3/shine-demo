using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Core : MonoBehaviour
{
    public static Core instance;
    public static Configs configs;
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
        configs = JsonConvert.DeserializeObject<Configs>(configAsset.text);

        Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj) =>
        {
            if (obj.Result != null) Instantiate(obj.Result);
            if (SceneManager.GetActiveScene().name == "LoadScene") SceneTransition.Transition("GameScene");
        };


        AdsManager.Init(configs.levelPlayAdsConfig);
    }
}