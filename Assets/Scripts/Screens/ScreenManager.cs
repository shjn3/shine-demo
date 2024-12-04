using System.Collections;
using System.Collections.Generic;
using ShineCore;
using UnityEngine;

public class ScreenKey
{
    public const string LEVEL_COMPLETED_SCREEN = "LevelCompletedScreen";
    public const string LEVEL_FAILED_SCREEN = "LevelFailedScreen";
    public const string SETTINGS_SCREEN = "SettingsScreen";


}
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    Stack<BaseScreen> currentScreens = new();
    Dictionary<string, BaseScreen> screensDictionary = new();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }

        instance = this;

        foreach (var screen in GetComponentsInChildren<BaseScreen>(true))
        {
            screen.gameObject.SetActive(false);
            screensDictionary.Add(screen.screenName, screen);
        }
    }

    public static void OpenScreen(string name)
    {
        if (instance == null)
        {
            return;
        }
        var screen = GetScreen(name);
        if (screen == null)
        {
            return;

        }
        screen.GetComponent<Canvas>().worldCamera = Camera.main;
        screen.gameObject.SetActive(true);
        instance.currentScreens.Push(screen);
        screen.Open();
        return;

    }

    public static void CloseScreen(string name)
    {
        if (instance == null) return;

        var screen = GetScreen(name);
        if (screen == null || instance.currentScreens.Count == 0)
        {
            return;
        }
        instance.currentScreens.Pop().Close();
        screen.gameObject.SetActive(false);
        return;
    }

    public static BaseScreen GetScreen(string name)
    {
        if (instance == null)
        {
            return null;
        }
        if (!instance.screensDictionary.ContainsKey(name))
        {
            Debug.Log("Screen doesn't exist! - " + name);
            return null;
        }

        return instance.screensDictionary[name];
    }


    public static T GetScreen<T>() where T : BaseScreen
    {
        if (instance == null)
        {
            return null;
        }
        foreach (var screen in instance.screensDictionary.Values)
        {
            if (screen is T t)
            {
                return t;
            }

        }
        return null;
    }
}
