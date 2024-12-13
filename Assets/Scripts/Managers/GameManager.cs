using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public InputManager inputManager;
    public GamePlay gamePlay;
    public LevelPlugin levelPlugin = new LevelPlugin();

    public void Retry()
    {
        gamePlay.Retry();
    }

    public bool IsWin()
    {
        return gamePlay.IsWin();
    }

    public bool IsLose()
    {
        return gamePlay.IsLose();
    }

    public void UpdateRatioScale(float remainingTopAreaHeight = GameSceneConfig.HEIGHT / 2, float remainingBottomAreaHeight = GameSceneConfig.HEIGHT / 2)
    {
        float ratio = Math.Min(1, (Screen.width * 1f / Screen.height) / (GameSceneConfig.WIDTH / GameSceneConfig.HEIGHT));
        float scaleTop = (remainingTopAreaHeight - gamePlay.transform.localPosition.y) / (GamePlayConfig.TUBE_GAP_VERTICAL + TubeConfig.HEIGHT / 2f + BallConfig.HEIGHT * 2 + TubeConfig.TOP_HEIGHT);

        float scaleBottom = (remainingBottomAreaHeight + gamePlay.transform.localPosition.y) / (GamePlayConfig.TUBE_GAP_VERTICAL + TubeConfig.HEIGHT / 2f + TubeConfig.BOTTOM_HEIGHT + BallConfig.HEIGHT);


        float scaleRatio = Mathf.Max(0, Mathf.Min(scaleTop, scaleBottom));
        gamePlay.UpdateRatio(scaleRatio * ratio);
    }
}
