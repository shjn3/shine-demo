using System.Linq;
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

    public bool ShowHint()
    {
        FindSolution();
        if (!IsCanShowHint() || guide == null)
            return false;
        guide?.Show(GetSuggestion());
        return true;
    }

    public void HintHint()
    {
        guide?.Hide();
    }
}