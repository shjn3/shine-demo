using System.Collections;
using System.Linq;
using ShineCore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
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
            Sleeper.instance.WaitForSeconds(0.15f).Then(() =>
            {
                SceneManager.LoadScene(sceneName);
                instance.transitionAnim.Play("FadeOut");

                Sleeper.instance.WaitForSeconds(0.15f).Then(() =>
                {
                    resolve();
                });
            });
        });
    }
}