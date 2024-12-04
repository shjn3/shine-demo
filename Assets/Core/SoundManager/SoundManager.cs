using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundKey
{
    BOUND = 0,
    BUTTON_CLICK = 1,
    CONFETTI = 2,
    HIGHLIGHT = 3,
    UN_HIGHLIGHT = 4,
    YOU_LOSE = 5,
    YOU_WIN = 6
}

// [Serializable]
// public struct SoundList
// {
//     public string name;
//     public AudioClip[] clips;
// }

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    private AudioClip[] clips;
    public static SoundManager instance;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public static void Play(SoundKey key, float volume = 1)
    {
        instance?.audioSource.PlayOneShot(instance.clips[(int)key], volume);
    }

    public static void Pause()
    {
        if (instance != null)
        {
            instance.audioSource.volume = 0;
        }
    }

    public static void Continue()
    {
        if (instance != null)
        {
            instance.audioSource.volume = 1;
        }
    }
    // #if UNITY_EDITOR
    //     private void OnEnable()
    //     {
    //         string[] names = Enum.GetNames(typeof(SoundKey));
    //         Array.Resize(ref soundList, names.Length);
    //         for (int i = 0; i < names.Length; i++)
    //         {
    //             soundList[i].name = names[i];
    //         }
    //     }

    // #endif
}
