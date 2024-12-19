using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;

using UnityEngine;

public class ShineFireBaseRemoteConfig
{

    private bool _enable = false;
    public bool enable
    {
        get
        {
            return _enable;
        }
    }
    const string PREFIX = "firebase.remoteConfig";
    const string UPDATE_AT = "updateAt";
    public TimeSpan cacheExpired = TimeSpan.FromHours(12);
    public Task Init()
    {
        var dateTime = DataStorage.GetDateTime(PREFIX + UPDATE_AT);
        if (DateTime.UtcNow - dateTime >= cacheExpired)
        {
            return FetchDataAsync().ContinueWithOnMainThread(task =>
            {
                _enable = true;
            });
        }

        Dictionary<string, object> defaults =
         new(){
           { "test", "v1"}
         };

        return FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
            .ContinueWithOnMainThread(task =>
            {
                _enable = true;
            });
    }


    public Task FetchDataAsync()
    {
        return FirebaseRemoteConfig.DefaultInstance.FetchAsync(cacheExpired).ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        DataStorage.SetDateTime(PREFIX + UPDATE_AT, DateTime.UtcNow);
        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(task =>
          {
              Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
          });
    }


    // Handle real-time Remote Config events.
    void ConfigUpdateListenerEventHandler(object sender, Firebase.RemoteConfig.ConfigUpdateEventArgs args)
    {
        if (args.Error != RemoteConfigError.None)
        {
            Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
            return;
        }

        Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(
            task =>
            {
                Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                               info.FetchTime));
            });
    }

    public void EnableAutoFetch()
    {
        Debug.Log("Enabling auto-fetch:");
        FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
            += ConfigUpdateListenerEventHandler;
    }

    public void DisableAutoFetch()
    {
        Debug.Log("Disabling auto-fetch:");
        FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener
            -= ConfigUpdateListenerEventHandler;
    }

    public ConfigValue GetValue(string key)
    {
        return FirebaseRemoteConfig.DefaultInstance.GetValue(key);
    }
}