using System.Collections;
using System.Linq;
using ShineCore;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
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
            Sleeper.WaitForSeconds(0.15f).Then(() =>
            {
                SceneManager.LoadScene(sceneName);
                instance.transitionAnim.Play("FadeOut");

                Sleeper.WaitForSeconds(0.15f).Then(() =>
                {
                    resolve();
                });
            });
        });
    }
}