using UnityEngine;
using ShineCore;
using System.Collections;
using System;

public class Sleeper : MonoBehaviour
{
    [HideInInspector]
    public static Sleeper instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        DontDestroyOnLoad(this);
    }

    public static Promise WaitForSeconds(float seconds)
    {
        if (instance == null)
        {
            return new Promise();
        }

        return new Promise(resolve =>
        {
            instance.StartCoroutine(instance.SleepAsync(seconds, resolve));
        });
    }

    public Promise WaitForSeconds(float seconds, Action callback)
    {
        return new Promise(resolve =>
        {
            StartCoroutine(SleepAsync(seconds, () =>
            {
                resolve();
                callback.Invoke();
            }));
        });
    }

    private IEnumerator SleepAsync(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}