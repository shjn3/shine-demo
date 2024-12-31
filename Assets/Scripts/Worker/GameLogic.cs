using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public struct GameSolution
{
    public Vector2[] moves;
    public int searchCount;
}

public class DFSState
{
    public GameState gameState;
    public Vector2 move;
    public DFSState parentState;
}

public class GameLogic
{
    public static GameSolution Solve(GameStateData data)
    {
        var gameSate = new GameState(data);
        DFSState state = new DFSState()
        {
            gameState = gameSate,
            move = Vector2.zero,
            parentState = null
        };

        MaxHeap<DFSState> maxHeap = new();
        maxHeap.Add(new MaxNode<DFSState>(state, 0));
        Vector2[] moves = DFSSolve(state, maxHeap);
        GameSolution gameSolution = new();
        int count = 0;
        while ((moves == null || moves?.Length == 0) && !maxHeap.IsEmpty() && count < 2000)
        {
            state = maxHeap.ExtractMax();
            moves = DFSSolve(state, maxHeap);
            count++;
        }

        gameSolution.moves = moves;
        gameSolution.searchCount = count;
        return gameSolution;
    }

    public static Vector2[] DFSSolve(DFSState dfsState, MaxHeap<DFSState> maxHeap)
    {
        if (dfsState.gameState.IsWin())
        {
            List<Vector2> moves = new(){
                dfsState.move
            };

            DFSState parent = dfsState.parentState;
            while (parent != null)
            {
                moves.Add(parent.move);
                parent = parent.parentState;
            }

            moves.RemoveAt(moves.Count - 1);
            moves.Reverse();
            return moves.ToArray();
        }

        if (dfsState.gameState.IsLose()) return null;

        GameSuccessor[] successors = dfsState.gameState.GetAllGameSuccessors();
        for (int i = 0; i < successors.Length; i++)
        {
            GameState newGameState = new(successors[i].data);
            DFSState newDfsState = new()
            {
                gameState = newGameState,
                move = new Vector2(successors[i].fromIdx, successors[i].toIdx),
                parentState = dfsState,
            };
            maxHeap.Add(new MaxNode<DFSState>(newDfsState, newDfsState.gameState.GetScore()));
        }

        return null;
    }
}