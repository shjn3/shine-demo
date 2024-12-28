using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MockBanner : MonoBehaviour
{
    public Image background;
    public Canvas canvas;
    void Awake()
    {
        background.GetComponent<RectTransform>().sizeDelta = new Vector2(320, GameSceneConfig.GetBannerHeight());
        SceneTransition.onLoadedScene += () =>
        {
            canvas.worldCamera = Camera.main;
        };
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
