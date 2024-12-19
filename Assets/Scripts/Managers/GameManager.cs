using System.Collections.Generic;
using UnityEngine;
using System;
using ShineCore;

public class GameManager : MonoBehaviour
{
    public GameScene gameScene;
    public GamePlay gamePlay;
    // public InputManager inputManager = new InputManager();
    public LevelPlugin levelPlugin = new LevelPlugin();
    private SuggestionManager suggestionManager = new SuggestionManager();
    void Awake()
    {
        Init();
    }
    public void Init()
    {
        suggestionManager.SetGameManager(this);
        suggestionManager.SetGuide(gameScene.guide);
    }

    public void Update()
    {
        inputManager?.Update();
    }

    public void Retry()
    {
        gamePlay.Retry();
    }

    public void UpdateRatioScale(float remainingTopAreaHeight = GameSceneConfig.HEIGHT / 2, float remainingBottomAreaHeight = GameSceneConfig.HEIGHT / 2)
    {
        float ratio = Math.Min(1, (Screen.width * 1f / Screen.height) / (GameSceneConfig.WIDTH / GameSceneConfig.HEIGHT));
        float scaleTop = (remainingTopAreaHeight - gamePlay.transform.localPosition.y) / (GamePlayConfig.TUBE_GAP_VERTICAL + TubeConfig.HEIGHT / 2f + BallConfig.HEIGHT * 2 + TubeConfig.TOP_HEIGHT);

        float scaleBottom = (remainingBottomAreaHeight + gamePlay.transform.localPosition.y) / (GamePlayConfig.TUBE_GAP_VERTICAL + TubeConfig.HEIGHT / 2f + TubeConfig.BOTTOM_HEIGHT + BallConfig.HEIGHT);


        float scaleRatio = Mathf.Max(0, Mathf.Min(scaleTop, scaleBottom));
        gamePlay.UpdateRatio(scaleRatio * ratio);
    }

    public void ShowHint(Action onCompletedCallback, Action<bool> onStartCallback)
    {
        suggestionManager.ShowHint(onCompletedCallback, onStartCallback);
    }
}
