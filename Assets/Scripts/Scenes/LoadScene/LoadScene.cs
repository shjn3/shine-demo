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
        Core.Init();
    }
#endif

    void LoadAsync()
    {
#if !UNITY_EDITOR
        Addressables.LoadAssetAsync<GameObject>("ShineCore").Completed += LoadScene.OnLoadDone;
#endif
    }
}
