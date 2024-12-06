using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Awake()
    {
        LoadAsync();
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
        Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += OnLoadDone;
    }
#endif

    void LoadAsync()
    {
#if !UNITY_EDITOR
        Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += LoadScene.OnLoadDone;
#endif
    }

    private static void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Result != null)
        {
            Instantiate(obj.Result);
        }

        if (SceneManager.GetActiveScene().name == "LoadScene")
        {
            SceneTransition.Transition("GameScene");

        }
    }
}
