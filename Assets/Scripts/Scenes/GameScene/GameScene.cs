using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public GameManager gameManager;
    public GameSceneFooter footer;
    public GameSceneHeader header;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager.UpdateRatioScale(GameSceneConfig.HEIGHT / 2 - header.GetHeight(), GameSceneConfig.HEIGHT / 2 - footer.GetHeight());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
