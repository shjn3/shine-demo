using System;
using UnityEngine;
namespace Shine.Ads
{
    public class AdsManagerListener : MonoBehaviour
    {
        public event Action<bool> onApplicationPauseCallback = (v) => { };

        void OnApplicationPause(bool isPaused)
        {
            onApplicationPauseCallback.Invoke(isPaused);
        }
    }
}