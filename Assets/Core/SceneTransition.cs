

using UnityEngine;
using UnityEngine.SceneManagement;
using Shine.Utils;
using Shine.Promise;
using System;
public class SceneTransition : MonoBehaviour
{
    public static event Action onLoadedScene = () => { };
    public static SceneTransition instance;
    [SerializeField]
    private Canvas canvas;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        canvas.worldCamera = Camera.main;
    }

    [SerializeField]
    private Animator transitionAnim;
    public static Promise Transition(string sceneName, float delay = 0)
    {
        if (instance == null)
        {
            return new Promise();
        }
        return new Promise(resolve =>
        {
            instance.transitionAnim.Play("FadeIn");
            Sleeper.WaitForSeconds(0.2f).Then(() =>
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
                operation.completed += (AsyncOperation) =>
                {
                    if (operation.isDone)
                    {
                        onLoadedScene.Invoke();
                        instance.canvas.worldCamera = Camera.main;
                        instance.transitionAnim.Play("FadeOut");
                        Sleeper.WaitForSeconds(0.15f).Then(() =>
                        {
                            resolve();
                        });
                    }
                };
            });
        });
    }
}
