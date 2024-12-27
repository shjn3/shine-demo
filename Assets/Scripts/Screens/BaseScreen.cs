using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using ShineCore;
using System;

public class BaseScreen : MonoBehaviour
{
    public string screenName = "";
    [HideInInspector]
    public Image background;

    public Action onceOpen = () => { };
    public Action onceClose = () => { };



    protected virtual void Awake()
    {
        background = transform.Find("BackgroundScreen").GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        RunFadeInBackground();
        OnOpen();
        onceOpen.Invoke();
        onceOpen = () => { };
    }

    public void Close()
    {
        RunFadeOutBackground();
        OnClose();
        onceClose.Invoke();
        onceClose = () => { };
    }

    private Tween fakeInBackgroundAnimation;
    private Tween fakeOutBackgroundAnimation;

    protected virtual Promise RunFadeInBackground()
    {
        if (background == null)
        {
            return new Promise();
        }
        return new Promise(resolve =>
   {
       Color color = background.color;
       background.color = new Color(color.r, color.g, color.b, 0);
       if (fakeInBackgroundAnimation != null)
       {
           fakeInBackgroundAnimation.Complete();
       }
       fakeInBackgroundAnimation = background.DOColor(new Color(color.r, color.g, color.b, 0.8f), 0.3f).OnComplete(() =>
                {
                    resolve();
                });
   });
    }

    protected virtual Promise RunFadeOutBackground()
    {
        if (background == null)
        {
            return new Promise();
        }
        return new Promise(resolve =>
        {
            Color color = background.color;
            background.color = new Color(color.r, color.g, color.b, 0.8f);
            if (fakeOutBackgroundAnimation != null)
            {
                fakeOutBackgroundAnimation.Complete();
            }
            fakeOutBackgroundAnimation = background.DOColor(new Color(color.r, color.g, color.b, 0), 0.3f).OnComplete(() =>
            {
                resolve();
            });
        });
    }

    public virtual void OnOpen()
    {
        //
    }

    public virtual void OnClose()
    {
        //
    }
}
