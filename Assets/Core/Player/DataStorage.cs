using System;
using UnityEngine;
public class DataStorage
{
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    public static void SetBool(string key, bool value)
    {

        PlayerPrefs.SetInt(key, (value ? 1 : 0));
    }

    public static int GetInt(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static float GetFloat(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1 ? true : false;
    }

    public static string GetString(string key, string defaultValue = "")
    {
        string value = PlayerPrefs.GetString(key);
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }

    public static void SetDateTime(string key, DateTime dateTime)
    {
        SetString(key, dateTime.ToFileTimeUtc().ToString());
    }

    public static DateTime GetDateTime(string key)
    {
        if (long.TryParse(GetString(key), out var v))
        {
            return DateTime.FromFileTime(v);
        }
        return DateTime.MinValue;
    }
}