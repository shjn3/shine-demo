using System;
using UnityEngine;

public class AdsManagerListener : MonoBehaviour
{
    public Action<bool> onApplicationPauseCallback = (v) => { };

    void OnApplicationPause(bool isPaused)
    {
        onApplicationPauseCallback.Invoke(isPaused);
    }
}