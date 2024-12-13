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
        canvas.worldCamera = Camera.main;
        background.GetComponent<RectTransform>().sizeDelta = new Vector2(320, GameSceneConfig.GetBannerHeight());
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
