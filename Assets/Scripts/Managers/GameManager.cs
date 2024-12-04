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
}
