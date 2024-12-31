using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;

namespace Shine.FireB
{
    public class ShineFireBase
    {
        private static ShineFireBase instance;
        private FirebaseApp app;
        public bool enable = false;
        private ShineFireBaseRemoteConfig remoteConfig;

        public static void Init()
        {
            instance = new ShineFireBase()
            {
                remoteConfig = new ShineFireBaseRemoteConfig()
            };


            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
               {
                   var dependencyStatus = task.Result;
                   if (dependencyStatus == DependencyStatus.Available)
                   {
                       instance.app = FirebaseApp.DefaultInstance;
                       instance.InitializeFirebase();
                       UnityEngine.Debug.Log("Init firebase success!");
                   }
                   else
                   {
                       UnityEngine.Debug.LogError(System.String.Format(
                         "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                   }
               });
        }

        void InitializeFirebase()
        {
            remoteConfig.Init();
        }

        public void OnDestroy()
        {
            remoteConfig?.DisableAutoFetch();
        }

        public void OnStart()
        {
            //
        }

        public ConfigValue GetRemoteConfigValue(string key)
        {
            return remoteConfig.GetValue(key);
        }
    }
}