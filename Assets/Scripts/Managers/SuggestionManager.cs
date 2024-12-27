using System;
using System.Linq;
using ShineCore;
using UnityEngine;
public struct Suggestion
{
    public Vector3 fromWorldPosition;
    public Vector3 toWorldPosition;
}

public class SuggestionManager
{
    private GameManager gameManager;
    private Guide guide;
    public SuggestionManager()
    {
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }


    public void SetGuide(Guide guide)
    {
        this.guide = guide;
    }

    private Suggestion GetSuggestion()
    {
        return new Suggestion()
        {
            fromWorldPosition = gameManager.gamePlay.tubes[(int)solution.moves[0].x].GetLastBallWorldPosition(),
            toWorldPosition = gameManager.gamePlay.tubes[(int)solution.moves[0].y].GetLastBallWorldPosition(),
        };
    }

    GameSolution solution;
    public bool IsCanShowHint()
    {
        if (solution.moves == null || solution.moves.Length == 0)
        {
            return false;
        }
        return true;
    }

    public void FindSolution()
    {
        solution = GameLogic.Solve(gameManager.gamePlay.GetGameStateData());
    }

    public void ShowHint(Action onCompletedCallback, Action<bool> onStartCallback)
    {
        FindSolution();
        if (!IsCanShowHint() || guide == null)
        {
            AdsManager.ShowNotifyScreen("Can't find any solution!", 5);
            onStartCallback.Invoke(false);
            onCompletedCallback.Invoke();
            return;
        }

        onStartCallback.Invoke(true);
        guide.Show(GetSuggestion()).Then(onCompletedCallback);
    }

    public void HintHint()
    {
        guide?.Hide();
    }


}