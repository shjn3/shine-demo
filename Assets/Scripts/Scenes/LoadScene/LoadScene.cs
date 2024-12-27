using UnityEngine;
using Shine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Shine.Promise;

public class LoadScene : MonoBehaviour
{
    public Progress progress;
    void Awake()
    {
        LoadGameAsync();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeInitialized()
    {
        if (SceneManager.GetActiveScene().name == "LoadScene")
        {
            return;
        }
        Core.Init();
        Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj) =>
        {
            if (obj.Result != null) Instantiate(obj.Result);
            if (SceneManager.GetActiveScene().name == "LoadScene") SceneTransition.Transition("GameScene");
        };
    }
#endif

    void LoadGameAsync()
    {
        progress.SetValue(0);
        Core.Init();
        Promise[] promises = new Promise[2];
        promises[0] = progress.RunProgress(1, delay: 0.3f);
        promises[1] = new Promise((resolve) =>
        {
            Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj) =>
                 {
                     if (obj.Result != null) Instantiate(obj.Result);
                     resolve();

                 };
        });
        Promise.All(promises).Then(() =>
        {
            if (SceneManager.GetActiveScene().name == "LoadScene") SceneTransition.Transition("GameScene");
        });
    }
}
